using System;

using Microsoft.Web.Services3;
using Microsoft.Web.Services3.Addressing;
using Microsoft.Web.Services3.Messaging;

namespace Server
{
    class Program
    {
        static void Main( string[ ] args )
        {
            //UDP Transport
            Uri viaUri = new Uri( "soap.udp://127.0.0.1:6000" );
            Uri toUri = new Uri( "soap.udp://weblogs.shockbyte.com.ar/rodolfof/wse/samples/2006/05/SampleReceiver" );

            //SMTP Transport
            //Uri viaUri = new Uri( "soap.smtp://wserequest@pboard.com.ar" );
            //Uri toUri = new Uri( "soap.smtp://weblogs.shockbyte.com.ar/rodolfof/wse/samples/2006/05/SampleReceiver" );

            //SQL Transport
            //Uri viaUri = new Uri( "soap.sql://localhost/Server" );
            //Uri toUri = new Uri( "soap.sql://weblogs.shockbyte.com.ar/rodolfof/wse/samples/2006/05/SampleReceiver" );

            SoapReceivers.Add( new EndpointReference( toUri, viaUri ), typeof( SampleReceiver ) );

            Console.WriteLine( "Listening for messages at " + toUri );
            Console.ReadLine( );
        }
    }
}
