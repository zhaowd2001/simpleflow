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
using log4net;
using log4net.Config;

namespace WseTransports.Smtp
{
    //In WSE3, an InputChannel is an instance of a conversation between the network and a 
    //specific service endpoint. The endpoint is indicated by the EndpointReference passed
    //to the InputChannel constructor; the transport type is implied by the concrete type of
    //the concrete InputChannel. 

    //This class implements an InputChannel class that services can use to listen 
    //on an SMTP network transport. Note that almost all of the functionality 
    //is inherited from the SoapInputChannel abstract base class.
    public class SoapSmtpInputChannel : SoapInputChannel
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SoapSmtpInputChannel));
        public SoapSmtpInputChannel(EndpointReference epr) : base(epr) { }

        //It's possible to request an InputChannel for which the transport
        //maintains no underlying network connection (e.g. the SoapTcpPassiveInputChannel ). 
        //Channels indicate whether the maintain an underlying network handle by returning
        //either SoapChannelCapabilities.ActivelyListening, or SoapChannelCapabilities.None.
        //This comes in handy for things like TCP or HTTP, where there's an actual network socket lying
        //around somewhere. However, we don't care about this for soap.smtp:// and both the InputChannel and
        //OutputChannel implementation return SoapChannelCapabilities.None
        public new SoapChannelCapabilities Capabilities
        {
            get
            {
                return SoapChannelCapabilities.None;
            }
        }
    }
}