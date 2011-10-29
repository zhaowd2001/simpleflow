//-------------------------------------------------------------------------------------------------
// This work is licensed under the Creative Commons Attribution-NonCommercial-ShareAlike License.
// To view a copy of this license, visit http://creativecommons.org/licenses/by-nc-sa/2.0/ or send
// a letter to Creative Commons, 559 Nathan Abbott Way, Stanford, California 94305, USA.
//
// Copyright Hervey Wilson, 2004.
//
//-------------------------------------------------------------------------------------------------
//WSE 3 Version by Rodolfo Finochietti (ml@pboard.com.ar)
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Timers;
using System.Xml;

using Microsoft.Web.Services3;
using Microsoft.Web.Services3.Addressing;
using Microsoft.Web.Services3.Diagnostics;
using Microsoft.Web.Services3.Messaging;
using Microsoft.Web.Services3.Referral;

using log4net;
using log4net.Config;

namespace WseTransports.Udp
{
    /// <summary>
    /// This class implements the soap.udp transport for WSE 2.0
    /// </summary>
    /// <remarks>
    /// Based on the Hervey Wilson work.
    /// </remarks>
    public sealed class SoapUdpTransport : SoapTransport, ISoapTransport
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SoapUdpTransport));
        /// <summary>
        /// Converts the specified IPEndPoint to its Uri form.
        /// </summary>
        internal static Uri ConvertToUri( IPEndPoint endpoint )
        {
            if ( endpoint.AddressFamily == AddressFamily.InterNetwork )
                return new Uri( String.Format( CultureInfo.InvariantCulture, "{0}://{1}:{2}/", UriScheme, endpoint.Address, endpoint.Port ) );
            else
                return new Uri( String.Format( CultureInfo.InvariantCulture, "{0}://[{1}]:{2}/", UriScheme, endpoint.Address, endpoint.Port ) );
        }

        /// <summary>
        /// Debugging helper.
        /// </summary>
        [System.Diagnostics.ConditionalAttribute( "DEBUG" )]
        internal static void Debug( string text )
        {
            System.Diagnostics.Debug.WriteLine( "soap.udp: " + text );
        }

        /// <summary>
        /// Determines whether the supplied IP Address is a multicast address.
        /// </summary>
        internal static bool IsMulticastAddress( IPAddress address )
        {
            string quadform = address.ToString( );
            string[ ] quads = quadform.Split( new char[ ] { '.' } );

            if ( quads[ 0 ].CompareTo( "223" ) > 0 && quads[ 0 ].CompareTo( "240" ) < 0 )
                return true;
            else
                return false;
        }

        /// <devdoc>
        /// The scheme for this transport
        /// </devdoc>
        public static readonly string UriScheme = "soap.udp";

        /// <devdoc>
        /// The Formatter for the transport
        /// </devdoc>
        ISoapFormatter _formatter = null;
        /// <devdoc>
        /// Host addresses for this machine
        /// </devdoc>
        ArrayList _hostAddresses = new ArrayList( );
        /// <devdoc>
        /// The long host name for this machine
        /// </devdoc>
        string _longHostName = null;
        /// <devdoc>
        /// The short host name for this machine
        /// </devdoc>
        string _shortHostName = null;
        /// <devdoc>
        /// Active sockets
        /// </devdoc>
        Hashtable _sockets = new Hashtable( );
        /// <devdoc>
        /// Transport options
        /// </devdoc>
        SoapUdpTransportOptions _options;

        /// <summary>
        /// Default constructor
        /// </summary>
        public SoapUdpTransport( ) : this( null ) { }

        /// <summary>
        /// Parameterized constructor
        /// </summary>
        public SoapUdpTransport( XmlNodeList configData )
        {
            //
            // Set default transport options, override with config settings if needed.
            //
            _formatter = new SoapPlainFormatter( );
            _options = new SoapUdpTransportOptions( );
            //
            // Process any configuration data
            //
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
                            case "exclusiveAddressUse":
                                //
                                // SO_EXCLUSIVEADDRUSE
                                //
                                value = child.GetAttribute( "enabled" );
                                _options.ExclusiveAddressUse = Convert.ToBoolean( value );
                                break;
                            case "defaultPort":
                                //
                                // Default Port
                                //
                                value = child.GetAttribute( "value" );
                                _options.DefaultPort = Convert.ToInt32( value );
                                break;
                            case "reuseAddress":
                                //
                                // Default Port
                                //
                                value = child.GetAttribute( "enabled" );
                                _options.ReuseAddress = Convert.ToBoolean( value );
                                break;
                            default:
                                //
                                // Ignore rather than fail
                                //
                                break;
                        }
                    }
                }
            }

            //
            // Retrieve the short and full host names for the machine, plus the external
            // network interface addresses.
            //
            IPHostEntry hostEntry = Dns.GetHostEntry( Dns.GetHostName( ) );

            foreach ( IPAddress address in hostEntry.AddressList )
            {
                if ( !IPAddress.IsLoopback( address ) )
                    _hostAddresses.Add( address );
            }

            _longHostName = hostEntry.HostName;

            int dotindex = _longHostName.IndexOf( '.' );

            if ( dotindex != -1 )
                _shortHostName = _longHostName.Substring( 0, dotindex );
            else
                _shortHostName = _longHostName;
            //
            // Registering for AppDomain.ProcessExit allows us to shutdown cleanly.
            //
            AppDomain.CurrentDomain.ProcessExit += new EventHandler( OnProcessExit );
        }

        internal void CloseInputChannel( SoapUdpInputChannel channel )
        {
            lock ( InputChannels.SyncRoot )
            {
                //
                // remove the channel
                //
                if ( InputChannels.Contains( channel ) )
                    InputChannels.Remove( channel );
                //
                // Decrement reference count on the socket for active channels
                //
                lock ( _sockets )
                {
                    IPEndPoint localEP = GetLocalIPEndPoint( channel.LocalEndpoint.TransportAddress );
                    SoapUdpSocket socket = _sockets[ localEP ] as SoapUdpSocket;

                    if ( socket != null )
                    {
                        if ( socket.ReleaseReference( ) == 0 )
                        {
                            socket.Close( );

                            _sockets.Remove( localEP );
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Convert the specified host name to an IPAddress on the local machine.
        /// </summary>
        IPAddress GetLocalIPAddress( string host )
        {
            //
            // Check for localhost first.
            //
            if ( string.Compare( host, "localhost", true, CultureInfo.CurrentCulture ) == 0 && Socket.SupportsIPv4 )
                return IPAddress.Loopback;

            if ( string.Compare( host, "localhost6", true, CultureInfo.CurrentCulture ) == 0 && Socket.OSSupportsIPv6 )
                return IPAddress.IPv6Loopback;
            //
            // Check against short machine name
            //
            if ( string.Compare( host, _shortHostName, true, CultureInfo.CurrentCulture ) == 0 )
                return IPAddress.Any;
            //
            // Check against full name
            //
            if ( string.Compare( host, _longHostName, true, CultureInfo.CurrentCulture ) == 0 )
                return IPAddress.Any;
            //
            // Check against valid addresses for this machine
            //
            IPAddress localAddress;

            try
            {
                localAddress = IPAddress.Parse( host );
            }
            catch
            {
                //
                // IPAddress.Parse failed - the host name is not an IPAddress
                // and therefore cannot be mapped to the local machine.
                //
                return null;
            }
            //
            // Multicast?
            //
            if ( IsMulticastAddress( localAddress ) )
                return localAddress;
            //
            // IPv4 Loopback?
            //
            if ( IPAddress.Loopback.Equals( localAddress ) && Socket.SupportsIPv4 )
                return localAddress;
            //
            // IPv6 Loopback?
            //
            if ( IPAddress.IPv6Loopback.Equals( localAddress ) && Socket.OSSupportsIPv6 )
                return localAddress;
            //
            // IPv4 Any?
            //
            if ( IPAddress.Any.Equals( localAddress ) && Socket.SupportsIPv4 )
                return localAddress;
            //
            // IPv6 Any?
            //
            if ( IPAddress.IPv6Any.Equals( localAddress ) && Socket.OSSupportsIPv6 )
                return localAddress;
            //
            // Configured address?
            //
            if ( _hostAddresses.Contains( localAddress ) )
                return localAddress;
            //
            // Unknown
            //
            return null;
        }

        /// <summary>
        /// Converts the specified URI into into a local IPEndPoint
        /// </summary>
        IPEndPoint GetLocalIPEndPoint( Uri transportAddress )
        {
            //
            // Build the canonical URI string for a connection
            //
            IPAddress address = GetLocalIPAddress( transportAddress.Host );

            if ( address == null )
                return null;

            return new IPEndPoint( address, ( transportAddress.Port == -1 ) ? _options.DefaultPort : transportAddress.Port );
        }

        /// <summary>
        /// </summary>
        void OnProcessExit( object sender, EventArgs args )
        {
            //
            // TODO: Sockets associated with SoapUdpOutputChannels are
            //       not closed here.
            //
            lock ( _sockets )
            {
                foreach ( SoapUdpSocket socket in _sockets.Values )
                    socket.Close( );
            }
        }

        /// <summary>
        /// </summary>
        void OnReceiveComplete( IAsyncResult ar )
        {
            Debug( "OnReceiveComplete" );

            SoapUdpSocket socket = ar.AsyncState as SoapUdpSocket;
            SoapEnvelope message = null;

            try
            {
                //
                // Complete the receiver operation and post another
                //
                message = socket.EndReceiveFrom( ar );

                socket.BeginReceiveFrom( new AsyncCallback( this.OnReceiveComplete ), socket );
            }
            catch ( Exception ex )
            {
                //
                // The EndReceive operation probably failed. This means that the
                // channel is dead and should be closed and then removed from
                // the set of available channels.
                //
                // We do not fault for this scenario.
                //
                Debug( "OnReceiveComplete: " + ex.Message );

                EventLog.WriteError( string.Format( "Soap.udp Message Receive Failure: {0}", new object[ ] { ex.ToString( ) } ) );
            }

            if ( message != null )
            {
                //
                // Dispatch the message. If it cannot be dispatched, SoapTransport.DispatchFailed will
                // be raised. Note that the soap.udp transport does not send faults if the dispatch
                // fails.
                //
                DispatchMessage( message );
            }
        }

        /// <summary>
        /// The transport options.
        /// </summary>
        public SoapUdpTransportOptions Options
        {
            get
            {
                return _options;
            }
        }

        #region ISoapTransport Members

        /// <summary>
        /// </summary>
        public string SoapBindingTransportUri
        {
            get
            {
                return "http://weblogs.shockbyte.com.ar/rodolfof/wse/transports/2006/05/udp";
            }
        }

        /// <summary>
        /// Build an ISoapInputChannel for the specified endpoint.
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="capabilities"></param>
        /// <returns></returns>
        ISoapInputChannel ISoapTransport.GetInputChannel( EndpointReference endpoint, SoapChannelCapabilities capabilities )
        {
            if ( endpoint == null )
                throw new ArgumentNullException( "endpoint" );

            if ( capabilities == SoapChannelCapabilities.ActivelyListening && endpoint.TransportAddress.Scheme != UriScheme )
                throw new ArgumentException( "Invalid Transport Scheme specified" );

            if ( capabilities != SoapChannelCapabilities.None && capabilities != SoapChannelCapabilities.ActivelyListening )
                throw new NotSupportedException( "Unsupported SoapChannelCapabilities Flags" );

            Debug( "GetInputChannel TransportAddress: " + endpoint.TransportAddress.ToString( ) );
            //
            // NOTE: Transport level receiving requires that we map the transport address to
            //       one of the local interfaces for the machine.
            //
            IPEndPoint localEP = GetLocalIPEndPoint( endpoint.TransportAddress );

            if ( localEP == null )
                throw new ArgumentException( "Transport address " + endpoint.TransportAddress.ToString( ) + " could not be mapped to a local network interface." );
            //
            // Lock the InputChannels collection while we register the new channel.
            //
            lock ( InputChannels.SyncRoot )
            {
                //
                // Determine whether the request channel already exists.
                //
                SoapUdpInputChannel channel = InputChannels[ endpoint ] as SoapUdpInputChannel;
                SoapUdpSocket socket;

                if ( channel != null )
                    return channel;
                //
                // Lock the sockets collection while checking for an existing socket
                // or creating a new one.
                //
                lock ( _sockets )
                {
                    socket = _sockets[ localEP ] as SoapUdpSocket;

                    if ( socket == null )
                    {
                        //
                        // There is no suitable socket available. Create a new one,
                        // increment it's reference count and start receiving packets.
                        //
                        socket = new SoapUdpSocket( localEP, _options );
                        _sockets[ localEP ] = socket;

                        socket.AddReference( );
                        socket.BeginReceiveFrom( new AsyncCallback( OnReceiveComplete ), socket );
                    }
                    else
                    {
                        //
                        // Increment the reference count on the listener for unregister behaviour
                        //
                        socket.AddReference( );
                    }
                }
                //
                // Create the channel, then modify the via so that it matches
                // the local endpoint address.
                //
                // TODO: Fix the race condition here where a packet might be
                //       received before the channel is established for 
                //       dispatching.
                //
                channel = new SoapUdpInputChannel( endpoint, socket, this );
                //
                // TODO: Do not do Via for the Any address
                //
                channel.LocalEndpoint.Via = new Via( ConvertToUri( localEP ) );
                //
                // Record the channel
                //
                InputChannels.Add( channel );

                return channel;
            }
        }

        /// <summary>
        /// Builds an ISoapOutputChannel for the specified endpoint.
        /// </summary>
        /// <param name="endpoint">The target endpoint</param>
        /// <param name="capabilities">The channel capabilities</param>
        /// <returns>ISoapOutputChannel</returns>
        ISoapOutputChannel ISoapTransport.GetOutputChannel( EndpointReference endpoint, SoapChannelCapabilities capabilities )
        {
            Debug( "GetOutputChannel TransportAddress: " + endpoint.TransportAddress.ToString( ) );

            if ( endpoint.TransportAddress.Scheme != UriScheme )
                throw new ArgumentException( "The transport scheme for the specified endpoint does not match this transport.", "endpoint" );

            if ( capabilities != SoapChannelCapabilities.None )
                throw new NotSupportedException( "Unsupported SoapChannelCapabilities flags. Use SoapChannelCapabilities.None." );

            return new SoapUdpOutputChannel( endpoint, new SoapUdpSocket( _options ), this );
        }

        #endregion
    }
}