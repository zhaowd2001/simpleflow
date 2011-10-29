using System;
using System.Collections.Generic;

using Microsoft.Web.Services3;
using Microsoft.Web.Services3.Addressing;
using Microsoft.Web.Services3.Messaging;

using log4net;
using log4net.Config;

namespace Server
{
    public class SampleReceiver : SoapReceiver
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SampleReceiver));

        protected override void Receive(SoapEnvelope message)
        {
            try
            {
                doReceive(message);
            }
            catch (Exception e)
            {
                log.Error(e);
            }
        }

        protected void doReceive(SoapEnvelope message)
        {
            log.Info( "Request -- " + message.Envelope.InnerXml );

            SoapSender sender = new SoapSender( message.Context.Addressing.ReplyTo.Address.Value );
            SoapEnvelope responseMessage = new SoapEnvelope( );

            responseMessage.Context.Addressing.Action = new Action( "http://weblogs.shockbyte.com.ar/rodolfof/wse/samples/2006/05/SampleServiceRequest#Response" );

            log.Info("Response -- " + responseMessage.Envelope.InnerXml);
            sender.Send( responseMessage );
        }
    }
}