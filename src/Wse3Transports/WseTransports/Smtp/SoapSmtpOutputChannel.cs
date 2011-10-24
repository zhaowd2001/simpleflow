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

using Microsoft.Web.Services3.Addressing;
using Microsoft.Web.Services3.Messaging;

namespace WseTransports.Smtp
{
    //In WSE3, an OutputChannel is an instance of a conversation between a service endpoint and its underlying
    //network transport. This class implements an OutputChannel for the soap.smtp network transport. Notice
    //almost all of the functionality is either inherited from the SoapOutputChannel base class or delegated 
    //to the transport.
    public class SoapSmtpOutputChannel : SoapOutputChannel
    {
        //A back-pointer to the transport that created this OutputChannel.
        private SoapSmtpTransport transport;

        //Create a new output channel that can send messages to the provided Endpoint, using
        //the provided instance of SoapSmtpTransport.
        public SoapSmtpOutputChannel( EndpointReference epr, SoapSmtpTransport transport ) : base( epr )
        {
            this.transport = transport;
        }

        //This is pretty much irrelevant for soap.smtp -- see SoapSmtpInputChannel.Capabilities
        //for more information.
        public new SoapChannelCapabilities Capabilities
        {
            get
            {
                return SoapChannelCapabilities.None;
            }
        }

        //The only method that we are required to override. Higher-level classes like SoapSender/SoapClient will
        //end up calling this method to send messages over the soap.smtp:// transport. Since the transport instance
        //owns all of the handles to the underlying network, we just forward this call to the transport
        //and let it do its work. This keeps the channel model very clean, and keeps all interaction with the network
        //inside of the SoapSmtpTransport class, where it belongs.
        public override void Send( Microsoft.Web.Services3.SoapEnvelope message )
        {
            this.transport.Send( message, message.Context.Addressing.Destination );
        }
    }
}