using System;
using System.Threading;

using Microsoft.Web.Services3;
using Microsoft.Web.Services3.Addressing;
using Microsoft.Web.Services3.Messaging;

using log4net;
using log4net.Config;

namespace Client
{
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));
        public static AutoResetEvent responseReceivedEventValue = new AutoResetEvent(false);

        [MTAThread]
        static void Main( string[ ] args )
        {
            // Set up a simple configuration that logs on the console.
            BasicConfigurator.Configure();

            log.Info("Entering client.");
            Program client = null;

            client = new Program( );
            client.Run( args );

            if ( responseReceivedEventValue.WaitOne( new TimeSpan( 0, 5, 0 ), false ) == false )
                Console.WriteLine( "No response was received in five minutes." );

            Console.WriteLine( "" );
            Console.WriteLine( "Press [Enter] to continue..." );
            Console.WriteLine( "" );
            Console.ReadLine( );
            log.Info("Exiting client.");
        }

        public void Run(string[] args)
        {
            SoapEnvelope message = new SoapEnvelope( );

            Uri viaUri = null;
            Uri toUri = null;
            Uri replyUri = null;
            if (args.Length >= 1)
            {
                switch (args[0])
                {
                    case "UDP":
                        viaUri = new Uri("soap.udp://127.0.0.1:6000");
                        toUri = new Uri("soap.udp://weblogs.shockbyte.com.ar/rodolfof/wse/samples/2006/05/SampleReceiver");
                        replyUri = new Uri("soap.udp://127.0.0.1:6001");
                        break;
                    case "SMTP":
                        viaUri = new Uri("soap.smtp://soapin@simpleflow.local");
                        toUri = new Uri("soap.smtp://weblogs.shockbyte.com.ar/rodolfof/wse/samples/2006/05/SampleReceiver");
                        replyUri = new Uri("soap.smtp://soapout@simpleflow.local");
                        break;
                    case "SQL":
                        viaUri = new Uri("soap.sql://localhost/sqlexpress");
                        toUri = new Uri("soap.sql://weblogs.shockbyte.com.ar/rodolfof/wse/samples/2006/05/SampleReceiver");
                        replyUri = new Uri("soap.sql://localhost/Client");
                        break;
                    default:
                        Help();
                        return;
                        break;
                }
            }
            else
            {
                Help();
                return;
            }

            message.Context.Addressing.Action = new Action( "http://weblogs.shockbyte.com.ar/rodolfof/wse/samples/2006/05/SampleServiceRequest" );
            message.Context.Addressing.ReplyTo = new ReplyTo( replyUri );

            SoapSender sender = new SoapSender( new EndpointReference( toUri, viaUri ) );

            SoapReceivers.Add( replyUri, typeof( ReplyReceiver ) );

            sender.Send( message );
            
            Console.WriteLine( "Calling {0}", sender.Destination.Address.Value );
        }

        static void Help()
        {
            System.Console.WriteLine("Usage: command line arguments: UDP, SMTP, SQL");
        }
    }

    public class ReplyReceiver : SoapReceiver
    {
        protected override void Receive( SoapEnvelope message )
        {
            if ( message.Fault != null )
                Console.WriteLine( "A Fault occured: {0}", message.Fault.ToString( ) );
            else
                Console.WriteLine( "Web Service called successfully" );

            Program.responseReceivedEventValue.Set( );
        }
    }
}