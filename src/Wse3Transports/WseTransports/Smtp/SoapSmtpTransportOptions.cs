using System;
using System.Globalization;

namespace WseTransports.Smtp
{
    /// <summary>
    /// Options for the Smtp transport.
    /// </summary>
    /// <author>Rodolfo Finochietti (ml@pboard.com.ar)</author>
    /// <remarks>Added to WSE 3 version</remarks>
    public sealed class SoapSmtpTransportOptions : ICloneable
    {
        string _mailServer = "localhost";
        string _mailServerPassword = String.Empty;
        string _smtpServer = "localhost";
        string _retrySeconds = "30";

        public SoapSmtpTransportOptions( ) { }

        public object Clone( )
        {
            SoapSmtpTransportOptions clone = new SoapSmtpTransportOptions( );

            clone.MailServer = _mailServer;
            clone.MailServerPassword = _mailServerPassword;
            clone.SmtpServer = _smtpServer;

            return clone;
        }

        public string MailServer
        {
            get
            {
                return _mailServer;
            }
            set
            {
                _mailServer = value;
            }
        }

        public string MailServerPassword
        {
            get
            {
                return _mailServerPassword;
            }
            set
            {
                _mailServerPassword = value;
            }
        }

        public string SmtpServer
        {
            get
            {
                return _smtpServer;
            }
            set
            {
                _smtpServer = value;
            }
        }

        public string RetrySeconds
        {
            get
            {
                return _retrySeconds;
            }
            set
            {
                _retrySeconds = value;
            }
        }
    }
}