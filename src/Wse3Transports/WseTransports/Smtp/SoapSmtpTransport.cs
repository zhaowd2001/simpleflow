/*
 * Author: Steve Maine
 * Email: stevem@hyperthink.net
 * Web: http://hyperthink.net/blog
 *
 * This work is licensed under the Creative Commons Attribution License. 
 * To view a copy of this license, visit http://creativecommons.org/licenses/by/2.0/ 
 * or send a letter to Creative Commons, 559 Nathan Abbott Way, Stanford, California 94305, USA.
 * 
 * No warranties expressed or implied.
 */
//WSE 3 Version by Rodolfo Finochietti (ml@pboard.com.ar)
using System;
using System.Collections;
using System.Xml;

using Microsoft.Web.Services3;
using Microsoft.Web.Services3.Addressing;
using Microsoft.Web.Services3.Diagnostics;
using Microsoft.Web.Services3.Messaging;
using log4net;
using log4net.Config;

namespace WseTransports.Smtp
{
    //In WSE3, the Transport is responsible for encaspulating the details of the underlying network.
    //It serves an a bridge between the wire and the rest of the messaging architecture, and is concerned
    //with transferring messages to and from the wire in some sort of transport-specific way. By encapsulating
    //the gory details of network communication inside the transport class, the rest of the WSE messaging architecture
    //can be made transport-agnostic. 

    //The Transport has a few major responsibilities:
    // - Create and return Input/Output channels when requested.
    // - Maintain whatever network handles are needed to allow those Channels to communicate
    // - Maintain a collection of all actively listening InputChannels waiting for a message
    //   to arrive from the network
    // - Receive messages from the network and dispatch them to the appropriate InputChannel
    // - Send messages to the network, when requested to do so by and OutputChannel

    //This class implements a transport for sending and receiving messages over SMTP.
    public class SoapSmtpTransport : SoapTransport, ISoapTransport
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SoapSmtpTransport));

        //This hashtable stores handles to active network connections. The actual type of these
        //connections vary by transport; the SoapTcpTransport has a collection of TcpConnections, and a SoapMsmq
        //transport might have a collection of MessageQueue object. In the case of SoapSmtp, the "network connection"
        //is an abstraction of a POP3 mailbox.
        private Hashtable _mailboxes;

        /// <devdoc>
        /// Transport options
        /// </devdoc>
        SoapSmtpTransportOptions _options;

        public SoapSmtpTransport( )
        {
            _mailboxes = new Hashtable( );
        }

        /// <summary>
        /// Parameterized constructor
        /// </summary>
        public SoapSmtpTransport( XmlNodeList configData ) : this( )
        {
            //Set default transport options, override with config settings if needed.
            _options = new SoapSmtpTransportOptions( );

            //Process any configuration data
            if ( configData != null )
            {
                string value;

                foreach ( XmlNode node in configData )
                {
                    XmlElement child = node as XmlElement;

                    if ( child != null )
                    {
                        switch ( child.LocalName )
                        {
                            case "mailServer":
                                value = child.GetAttribute( "value" );
                                _options.MailServer = value;
                                break;
                            case "mailServerPassword":
                                value = child.GetAttribute( "value" );
                                _options.MailServerPassword = value;
                                break;
                            case "smtpServer":
                                value = child.GetAttribute( "value" );
                                _options.SmtpServer = value;
                                break;
                            case "retrySeconds":
                                value = child.GetAttribute( "value" );
                                _options.RetrySeconds = value;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
        
        #region ISoapTransport Members

        /// <summary>
        /// </summary>
        public string SoapBindingTransportUri
        {
            get
            {
                return "http://weblogs.shockbyte.com.ar/rodolfof/wse/transports/2006/05/smtp";
            }
        }
        
        //Creates a new InputChannel for the specified endpoint. As part of this operation, the transport
        //must create a new Mailbox and begin polling that Mailbox for new messages.
        public ISoapInputChannel GetInputChannel( EndpointReference endpoint, Microsoft.Web.Services3.Messaging.SoapChannelCapabilities capabilities )
        {
            //The newly created InputChannel
            SoapSmtpInputChannel channel;

            if ( null == endpoint )
                throw new ArgumentNullException( "destination", "Cannot open input channel on null endpoint" );

            //It's vitally important to lock the InputChannels collection. Otherwise, we get a race condition
            //if a message arrives between the call to m.BeginReceive() and InputChannels.Add(). However, 
            //SoapTransport.DispatchMessage() will block trying to take the lock on InputChannels, 
            //so as long as we lock here we're OK.
            lock ( this.InputChannels.SyncRoot )
            {
                //Create a new Mailbox on the transport address of the endpoint. We use endpoint.TransportAddress here
                //to look for a Via on the endpoint if one exists. If we just used endpoint.Address, we'd be unable
                //to use soap.smtp endpoints as intermediaries.
                Mailbox m = FindOrCreateMailbox( endpoint.TransportAddress );

                //Create the new input channel
                channel = new SoapSmtpInputChannel( endpoint );

                //If the mailbox isn't already listening, start polling for messages.
                if ( !m.Listening )
                    m.BeginRecieve( new AsyncCallback( this.OnReceiveComplete ) );

                //Finally, add the new channel to our collection of InputChannels (which we inherited from the SoapTransport base
                //class). This will allow us to dispatch messages addressed to this channel via DispatchMessage().
                this.InputChannels.Add( channel );
            }

            //Return the channel so that the higher-level messaging components can do what they need to on it.
            //Usually, they will take the returned InputChannel and call BeginReceive() on it, so that when messages
            //arrive on the channel they can pick them off and dispatch them to the appropriate SoapReceiver for processing.
            return channel;
        }

        //Creating an OutputChannel is a lot easier than creating an InputChannel, since we don't have to
        //store the channel or the Mailbox. We just do some sanity checks on the input and then return
        //a new SoapSmtpOutputChannel.
        public ISoapOutputChannel GetOutputChannel( EndpointReference endpoint, Microsoft.Web.Services3.Messaging.SoapChannelCapabilities capabilities )
        {
            if ( capabilities != SoapChannelCapabilities.None )
                throw new NotSupportedException( "Unsupported channel capabilities" );

            if ( endpoint == null )
                throw new ArgumentNullException( "endpoint", "Cannot open output channel for null endpoint" );

            return new SoapSmtpOutputChannel( endpoint, this );
        }
        
        #endregion

        //Called by SoapSmtpOutputChannel.Send(). Implementing
        //this on the transport instead of the channel keeps all references to
        //transport-specific network resources inside of the transport class, and keeps
        //the channels clean.
        internal void Send( SoapEnvelope e, EndpointReference destination )
        {
            //Create a new mailbox based on the TransportAddress of the endpoint. Again,
            //we use TransportAddress instead of Address to allow soap.smtp:// endpoints to
            //act as intermediaries.
            Mailbox box = new Mailbox( destination.TransportAddress, _options );

            //Send the message via SMTP.
            box.Send( e, destination.TransportAddress );
        }

        //This is the callback method that gets invoked whenever a set of messages arrive from a mailbox.
        //This callback is set up by GetInputChannel().
        internal void OnReceiveComplete( IAsyncResult result )
        {
            //We carry around the mailbox as AsyncState
            Mailbox box = result.AsyncState as Mailbox;
            SoapEnvelope[ ] envelopes = null;
            SoapEnvelope envelope;

            try
            {
                //Terminate the ansyc receive operation and get back any SoapEnvelopes that
                //have arrived. Note that the Mailbox class is implemented in such a way as to
                //guarantee that this array will always be populated -- Mailbox.BeginReceive does not
                //terminate until at least one message has arrived.
                envelopes = box.EndReceive( result );
            }
            catch ( AsynchronousOperationException e )
            {
                //If anything happened to the mail server, we have a problem
                if ( e.InnerException is Lesnikowski.Client.ServerException )
                    throw;
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                //Any other exceptions that occured on the async thread will be caught here. 
                string errorMessage = "Receive failed from: " + box.Address + "\n" + e.InnerException.Message;
                EventLog.WriteError( errorMessage);
                log.Error(errorMessage);
                //Start the mailbox listening again. 
                box.BeginRecieve( new AsyncCallback( this.OnReceiveComplete ) );
            }

            //Just in case we did catch an exception...
            if ( null == envelopes )
                return;

            //Iterate over all the received messages and dispatch them to their waiting InputChannels
            for ( int i = 0; i < envelopes.Length; i++ )
            {
                envelope = envelopes[ i ];

                //If we have an envelope that couldn't be dispatched, and that envelope doesn't have a Fault
                //and it's not a response, send an "endpoint not found" fault back to the client.
                //Note the call to base.DispatchMessage(), which does most of the heavy lifting around message dispatch.

                //This conditional was obtained by looking at the implementation of the SoapTcpTransport
                if ( ( ( ( envelope != null ) && !base.DispatchMessage( envelope ) ) && ( envelope.Fault == null ) ) &&
                    ( ( envelope.Context.Addressing.RelatesTo == null ) || ( envelope.Context.Addressing.RelatesTo.RelationshipType != WSAddressing.AttributeValues.Reply ) ) )
                {
                    //If we get here, we have an undeliverable message. We need to send a fault back to the originator.
                    try
                    {
                        SoapEnvelope faultMessage = base.GetFaultMessage( envelope,
                            new AddressingFault( AddressingFault.DestinationUnreachableMessage,
                            AddressingFault.DestinationUnreachableCode ) );

                        //We can't send faults back to the anonymous role
                        if ( !faultMessage.Context.Addressing.Destination.TransportAddress.Equals( WSAddressing.AnonymousRole ) )
                        {
                            //The faultTo address may not be a soap.smtp:// endpoint. Call StaticGetOutputChannel
                            //to get an OutputChannel for the faultTo address.
                            ISoapOutputChannel outputChannel = SoapTransport.StaticGetOutputChannel( faultMessage.Context.Addressing.Destination );
                            outputChannel.Send( faultMessage );
                        }
                    }
                    catch ( Exception e )
                    {
                        //If anything bad happened during the sending of said fault, log the 
                        //exception and get on with life
                        EventLog.WriteError( String.Format( "soap.msmq Fault Send Failed: {0}", e ) );
                    }
                }
            }
        }

        //Creates a new mailbox and stores it in the mailboxes collection if one
        //does not already exist for the specified URI.
        internal Mailbox FindOrCreateMailbox( Uri uri )
        {
            lock ( _mailboxes )
            {
                if ( !_mailboxes.Contains( uri ) )
                {
                    Mailbox box = new Mailbox( uri, _options );
                    _mailboxes.Add( uri, box );
                }

                return ( Mailbox )_mailboxes[ uri ];
            }
        }
    }
}