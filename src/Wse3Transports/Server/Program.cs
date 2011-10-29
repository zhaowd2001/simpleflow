using System;

using Microsoft.Web.Services3;
using Microsoft.Web.Services3.Addressing;
using Microsoft.Web.Services3.Messaging;
using log4net;
using log4net.Config;

namespace Server
{
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        static void Main( string[ ] args )
        {
            // Set up a simple configuration that logs on the console.
            BasicConfigurator.Configure();
            
            log.Info("Entering server.");

            Uri viaUri = null;
            Uri toUri = null;

            if (args.Length >= 1)
            {
                switch (args[0])
                {
                    case "UDP":
                        //UDP Transport
                        viaUri = new Uri("soap.udp://127.0.0.1:6000");
                        toUri = new Uri("soap.udp://weblogs.shockbyte.com.ar/rodolfof/wse/samples/2006/05/SampleReceiver");
                        break;
                    case "SMTP":
                        //SMTP Transport
                        viaUri = new Uri("soap.smtp://soapin@simpleflow.local");
                        toUri = new Uri("soap.smtp://weblogs.shockbyte.com.ar/rodolfof/wse/samples/2006/05/SampleReceiver");
                        break;
                    case "SQL":
                        //SQL Transport
                        viaUri = new Uri("soap.sql://localhost/sqlexpress");
                        toUri = new Uri("soap.sql://weblogs.shockbyte.com.ar/rodolfof/wse/samples/2006/05/SampleReceiver");
                        break;
                    default:
                        Help();
                        return;
                }
            }
            else
            {
                Help();
                return;
            }
            SoapReceivers.Add( new EndpointReference( toUri, viaUri ), typeof( SampleReceiver ) );

            Console.WriteLine( "Listening for messages at " + toUri );
            Console.ReadLine( );
            log.Info("Exiting server.");
        }
        static void Help()
        {
            System.Console.WriteLine("Usage: command line arguments: UDP, SMTP, SQL");
        }
    }
}
