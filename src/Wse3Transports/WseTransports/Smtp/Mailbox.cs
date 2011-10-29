/*
 * Author: Steve Maine
 * Email: stevem@hyperthink.net
 * Web: http://hyperthink.net/blog
 *
 * This code may be redistributed and modified at will, as long as any derivative works
 * credit the original author.
 * 
 * No warranties expressed or implied.
 * 
 * This class makes use of Pawel Lesnikowski's free POP3 library for C#.
 * You can find this code on the web at http://lesnikowski.fm.interia.pl/Mail/mail.html.
 * Thanks, Pawel!
 */
//WSE 3 Version by Rodolfo Finochietti (ml@pboard.com.ar)
using System;
using System.Configuration;
using System.IO;
using System.Text;
using System.Threading;

using Microsoft.Web.Services3;
using Microsoft.Web.Services3.Addressing;
using Microsoft.Web.Services3.Diagnostics;
using Microsoft.Web.Services3.Messaging;
using Microsoft.Web.Services3.Referral;

using log4net;
using log4net.Config;

using Lesnikowski.Mail;
using Lesnikowski.Client;

namespace WseTransports.Smtp
{
    //The Mailbox class represents the "network resource" used by the SoapSmtp transport.
    //It is analogous in function to a TcpConnection.
    public class Mailbox
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Mailbox));
        private string _address;
        private Uri _endpointUri;
        private bool _listening;

        public SoapSmtpTransportOptions Options;

        //Creates a new Mailbox using the specified soap.smtp:// uri
        public Mailbox( Uri endpoint, SoapSmtpTransportOptions options )
        {
            this._address = SoapSmtpUri.AddressFromUri( endpoint );
            this._endpointUri = endpoint;
            
            this.Options = options;
        }

        //The POP3 address of this Mailbox
        public string Address
        {
            get
            {
                return _address;
            }
        }

        //The soap.msmq:// URI of this mailbox
        public Uri EndpointUri
        {
            get
            {
                return _endpointUri;
            }
        }

        //Returns true if this Mailbox is already polling the server for messages
        public bool Listening
        {
            get
            {
                return _listening;
            }
        }

        //Starts a new asynchronous receive operation by creating a new MailClientAsyncResult
        public IAsyncResult BeginRecieve( AsyncCallback callback )
        {
            this._listening = true;
            return new MailClientAsyncResult( this, callback );
        }

        //This will get called by whomever called BeginReceive on this Mailbox.
        //This is the part where we deserialize the incoming MailMessages into SoapEnvelopes.
        public SoapEnvelope[ ] EndReceive( IAsyncResult result )
        {
            Mailbox.MailClientAsyncResult ar = result as Mailbox.MailClientAsyncResult;

            if ( null == ar )
                throw new ArgumentException( "AsyncResult not obtained from Mailbox.BeginReceive()", "result" );

            //This line is also very important, as it's the second half of the exception marshalling story. Any
            //exceptions that occured during the async operation will be rethrown here.
            AsyncResult.End( result );

            //We need a UTF8 encoding because we're going to be mucking about
            //with MemoryStreams
            UTF8Encoding encoding = new UTF8Encoding( );
            SoapEnvelope[ ] envelopes;

            envelopes = new SoapEnvelope[ ar.Messages.Length ];

            //Iterate over each of 
            for ( int i = 0; i < envelopes.Length; i++ )
            {
                try
                {
                    SimpleMailMessage m = ar.Messages[ i ];

                    if ( m != null )
                    {
                        dump(i,m);
                        //Wrap a MemoryStream around the message body and deserialize it using
                        //the SoapPlainFormatter (we don't care about DIME attachments here).
                        ISoapFormatter formatter = new SoapPlainFormatter( );
                        MemoryStream stream = new MemoryStream( encoding.GetBytes( m.TextDataString ) );

                        try
                        {
                            envelopes[ i ] = formatter.Deserialize( stream );
                        }
                        catch
                        {
                            //swallow any exceptions that happened during deserialization -- a non-SOAP message
                            //shouldn't crash us
                        }

                        //We need to set up some addressing headers here. This attaches an implicit "Via" to the
                        //message, indicating it came in on this transport.
                        if ( envelopes[ i ] != null )
                        {
                            SoapEnvelope envelope = envelopes[ i ];

                            AddressingHeaders headers = envelope.Context.Addressing;
                            Uri remoteEndpoint;
                            Uri localEndpoint = this.EndpointUri;

                            if ( m.From[ 0 ] != null && m.From[ 0 ].Address != String.Empty )
                                remoteEndpoint = SoapSmtpUri.UriFromAddress( m.From[ 0 ].Address );
                            else
                                remoteEndpoint = localEndpoint;

                            //AddressingHeaders.SetRequestHeaders() does most of the hard work for us.
                            //Note that if you don't have MessagingConfiguration.EnableRedirectedResponses set to
                            //true in configuration, all replies will get routed to localEndpoint (which won't do much
                            //other than cause DispatchFailed events)
                            headers.SetRequestHeaders( new Via( localEndpoint ), new Via( remoteEndpoint ) );
                        }
                    }
                }
                catch ( Exception e )
                {
                    EventLog.WriteError( e.Message + "\nCould not deserialize message: " + ar.Messages[ i ].TextDataString );
                }
            }

            return envelopes;
        }

        public void Send(SoapEnvelope e, Uri destination)
        {
            try
            {
                doSend(e, destination);
            }
            catch (Exception e1)
            {
                log.Error(e1);
            }
        }

        //Writes a SoapEnvelope to a UTF8-encoded MemoryStream and mails it via SMTP.
        public void doSend( SoapEnvelope e, Uri destination )
        {
            ISoapFormatter formatter = new SoapPlainFormatter( );
            UTF8Encoding encoding = new UTF8Encoding( );
            MemoryStream stream = new MemoryStream( );
            formatter.Serialize( e, stream );

            string smtpServer = Options.SmtpServer;
            string to = SoapSmtpUri.AddressFromUri( destination );
            string from = this.Address;
            string subject = ( null == e.Context.Addressing.Action ) ? "soap.smtp" : e.Context.Addressing.Action.ToString( );
            log.Debug(string.Format("send:to -- {0}" , to));
            log.Debug(string.Format("send:subject -- {0}", subject));

            System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage( from, to );
            message.Subject = subject;

            message.Body = encoding.GetString( stream.GetBuffer( ) );

            System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient( smtpServer );

            client.Send( message );
        }

        #region MailClientAsyncResult

        //The MailClientAsyncResult represents an asynchronous receive operation. Mailbox.BeginReceive()
        //will return a new instance of this class. 
        private class MailClientAsyncResult : AsyncResult
        {
            //The mail messages that were pulled down from the server
            //during this receive operation.
            public SimpleMailMessage[ ] Messages;

            //Creates a new MailClientAsyncResult and begins an asynchronous receive operation.
            //When this operation completes, it will call the specified AsyncCallback, carrying the specified
            //Mailbox instance as its AsyncState.

            //Note that this class derives from Microsoft.Web.Services3.AsyncResult, which gives us 
            //a lot of useful features.
            public MailClientAsyncResult( Mailbox box, AsyncCallback callback )
                : base( callback, box )
            {
                //Initiate the asynchronous operation using the ThreadPool. Sooner or later,
                //this will result in the Receive() method being called.
                ThreadPool.QueueUserWorkItem( new WaitCallback( this.Receive ) );
            }

            private void Receive(object state)
            {
                try
                {
                    doReceive(state);
                }
                catch (Exception e)
                {
                    log.Error(e);
                }
            }

            //Receives messages from the Mailbox. Note that the state parameter is ignored; we're already
            //carrying our state in the AsyncState property of the base class. However, Receive() has to 
            //take a single parameter of type Object in order to be a valid WaitCallback delegate.
            private void doReceive( object state )
            {
                Pop3 pop3 = null;
                bool bCaughtException = false;

                //This part does the work of connecting to the POP3 account, logging in, and
                //checking for new messages. If any messages were found, they will be downloaded
                //and stored in this.Messages
                try
                {
                    Mailbox box = this.AsyncState as Mailbox;

                    string server = box.Options.MailServer;
                    string username = box.Address;
                    string password = box.Options.MailServerPassword;

                    pop3 = new Pop3( );
                    pop3.User = username;
                    pop3.Password = password;

                    pop3.Connect( server );

                    if ( pop3.HasTimeStamp )
                        pop3.APOPLogin( );
                    else
                        pop3.Login( );

                    pop3.GetAccountStat( );
                    this.Messages = new SimpleMailMessage[ pop3.MessageCount ];

                    log.Debug(string.Format("pop3 check -- {0} - mail count:{1}", username, pop3.MessageCount));

                    //If we don't have any messages, go to sleep for a little while and try again.
                    //We'll keep doing this sleep/retry loop until a message shows up. That way, SoapTransport
                    //never has to pay any attention to us until we actually have work for it to do.
                    if ( pop3.MessageCount == 0 )
                    {
                        pop3.Close( );
                        pop3 = null;
                        SleepAndRetry( box.Options.RetrySeconds );
                        return;
                    }

                    for ( int i = 1; i <= pop3.MessageCount; i++ )
                    {
                        try
                        {
                            string message = pop3.GetMessage( i );
                            this.Messages[ i - 1 ] = SimpleMailMessage.Parse( message );
                        }
                        finally
                        {
                            pop3.DeleteMessage( i );
                        }

                    }

                }
                catch ( Exception e )
                {
                    //This part's very important. Since we're running on a ThreadPool thread right now, any exceptions
                    //thrown on this thread will be swallowed by the ThreadPool. If an exception happens, we need to
                    //somehow marshal it back to the thread that initiated the async operation and rethrow it there.
                    //Forutnately, the AsyncResult base class lets us do that. We'll catch the exception here and
                    //pass it to Complete(). When the originating thread calls AsyncResult.End() on us, the AsyncResult base
                    //class will rethrow the exception on the right thread so it can be handled by the application.

                    bCaughtException = true;
                    base.Complete( false, e );
                }
                finally
                {
                    if ( pop3 != null )
                        pop3.Close( true );
                }

                if ( !bCaughtException )
                    base.Complete( false );
            }

            //If this method gets called, it means there we're any messages on the server that need to be downloaded.
            //So, we'll sleep for a little while and then try again.
            private void SleepAndRetry( string timeout )
            {
                int retry;

                if ( null == timeout )
                    retry = 30;
                else
                    retry = Int32.Parse( timeout );

                Thread.Sleep( new TimeSpan( 0, 0, 0, retry, 0 ) );
                ThreadPool.QueueUserWorkItem( new WaitCallback( this.Receive ) );
            }
        }

        static void dump(int index, SimpleMailMessage m)
        {
            log.Debug(string.Format("recv[{0}]:to -- {1}", index, m.To));
            log.Debug(string.Format("recv[{0}]:subject -- {1}", index, m.Subject));
        }
        #endregion
    }
}