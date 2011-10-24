using System;
using System.Collections.Generic;

using Microsoft.Web.Services3;
using Microsoft.Web.Services3.Addressing;
using Microsoft.Web.Services3.Messaging;

namespace Server
{
    public class SampleReceiver : SoapReceiver
    {
        protected override void Receive( SoapEnvelope message )
        {
            Console.WriteLine( "Request received." );

            SoapSender sender = new SoapSender( message.Context.Addressing.ReplyTo.Address.Value );
            SoapEnvelope responseMessage = new SoapEnvelope( );

            responseMessage.Context.Addressing.Action = new Action( "http://weblogs.shockbyte.com.ar/rodolfof/wse/samples/2006/05/SampleServiceRequest#Response" );
 
            sender.Send( responseMessage );
        }
    }
}