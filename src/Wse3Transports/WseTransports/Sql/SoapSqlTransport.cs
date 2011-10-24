//WSE 3 Version by Rodolfo Finochietti (ml@pboard.com.ar)
using System;
using System.Collections;
using System.Xml;

using Microsoft.Web.Services3;
using Microsoft.Web.Services3.Addressing;
using Microsoft.Web.Services3.Messaging;

namespace WseTransports.Sql
{
    public class SoapSqlTransport : ISoapTransport
    {
        private Hashtable _inputChannels;
        private Hashtable _outputChannels;
        private object _inputChannelLock;
        private object _outputChannelLock;
        private SoapSqlTransportOptions _options = new SoapSqlTransportOptions( );

        public static readonly string UriScheme = "soap.sql";

        public SoapSqlTransport( )
        {
            _inputChannelLock = new object( );
            _outputChannelLock = new object( );
        }

        /// <summary>
        /// Parameterized constructor
        /// </summary>
        public SoapSqlTransport( XmlNodeList configData ) : this( )
        {
            //Set default transport options, override with config settings if needed.
            _options = new SoapSqlTransportOptions( );

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
                            case "connectionString":
                                value = child.GetAttribute( "value" );
                                _options.ConnectionString = value;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        public string SoapBindingTransportUri
        {
            get
            {
                return "http://weblogs.shockbyte.com.ar/rodolfof/wse/transports/2006/05/sql";
            }
        }

        public ISoapInputChannel GetInputChannel( EndpointReference endpoint, SoapChannelCapabilities capabilities )
        {
            //TODO: Endpoint should really be validated.
            ISoapInputChannel channel = null;

            string epDesc = endpoint.TransportAddress.ToString( );

            lock ( _inputChannelLock )
            {
                channel = InputChannels[ epDesc ] as ISoapInputChannel;

                if ( channel == null )
                {
                    channel = new SoapSqlInputChannel( endpoint, _options.ConnectionString );
                    InputChannels[ epDesc ] = channel;
                }
            }

            return channel;
        }

        public ISoapOutputChannel GetOutputChannel( EndpointReference endpoint, SoapChannelCapabilities capabilities )
        {
            //TODO: Endpoint should really be validated.
            ISoapOutputChannel channel = null;

            string epDesc = endpoint.TransportAddress.ToString( );

            lock ( _outputChannelLock )
            {
                channel = OutputChannels[ epDesc ] as ISoapOutputChannel;

                if ( channel == null )
                {
                    channel = new SoapSqlOutputChannel( endpoint, _options.ConnectionString );
                    OutputChannels[ epDesc ] = channel;
                }
            }

            return channel;
        }

        private Hashtable InputChannels
        {
            get
            {
                lock ( _inputChannelLock )
                {
                    if ( _inputChannels == null )
                        _inputChannels = new Hashtable( );
                }

                return _inputChannels;
            }
        }

        private Hashtable OutputChannels
        {
            get
            {
                lock ( _outputChannelLock )
                {
                    if ( _outputChannels == null )
                        _outputChannels = new Hashtable( );
                }

                return _outputChannels;
            }
        }
    }
}