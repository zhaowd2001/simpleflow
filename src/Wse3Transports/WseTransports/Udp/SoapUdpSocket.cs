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
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using Microsoft.Web.Services3;
using Microsoft.Web.Services3.Addressing;
using Microsoft.Web.Services3.Referral;
using Microsoft.Web.Services3.Messaging;

namespace WseTransports.Udp
{
    /// <summary>
    /// A UDP socket
    /// </summary>
    /// <remarks>
    /// Based on the Hervey Wilson work.
    /// </remarks>
    internal sealed class SoapUdpSocket
    {
        sealed class SoapUdpSocketAsyncResult : AsyncResult
        {
            Socket _socket;
            SoapUdpDatagram _datagram;

            public SoapUdpSocketAsyncResult( Socket socket, SoapUdpDatagram datagram, AsyncCallback callback, object state ) : base( callback, state )
            {
                _socket = socket;
                _datagram = datagram;
            }

            public SoapUdpDatagram Datagram
            {
                get
                {
                    return _datagram;
                }
            }

            public void OnReceive( IAsyncResult ar )
            {
                try
                {
                    //
                    // Receive the data and set the completion state of the result.
                    //
                    _datagram.Available = _socket.EndReceiveFrom( ar, ref _datagram.EndPoint );

                    Complete( ar.CompletedSynchronously );
                }
                catch ( ObjectDisposedException )
                {
                    //
                    // The socket has already been closed. In this case we ignore the fact that
                    // the completion has been raised and do not signal back to the application.
                    //
                    SoapUdpTransport.Debug( "SoapUdpSocketAsyncResult Socket Disposed" );
                }
                catch ( Exception ex )
                {
                    SoapUdpTransport.Debug( "SoapUdpSocketAsyncResult Exception =" + ex.Message );
                    SoapUdpTransport.Debug( "SoapUdpSocketAsyncResult StackTrace=" + ex.StackTrace );

                    Complete( ar.CompletedSynchronously, ex );
                }
            }
        }

        Socket _socket = null;
        SoapUdpTransportOptions _options;
        int _refCount = 0;
        IPEndPoint _localEP = null;
        IPAddress _mcastAddress = null;
        ISoapFormatter _formatter = new SoapPlainFormatter( );
        Uri _localVia;


        /// <summary>
        /// Constructor 1.
        /// </summary>
        internal SoapUdpSocket( SoapUdpTransportOptions options )
        {
            if ( options == null )
                throw new ArgumentNullException( "options" );

            _options = options.Clone( ) as SoapUdpTransportOptions;
        }

        /// <summary>
        /// Constructor 2.
        /// </summary>
        internal SoapUdpSocket( IPEndPoint localEP, SoapUdpTransportOptions options )
        {
            if ( localEP == null )
                throw new ArgumentNullException( "endpoint" );

            if ( options == null )
                throw new ArgumentNullException( "options" );

            _options = options.Clone( ) as SoapUdpTransportOptions;

            Bind( localEP );
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~SoapUdpSocket( )
        {
            //
            // Ensure we are stopped
            //
            Close( );
        }

        /// <summary>
        /// </summary>
        internal void AddReference( )
        {
            Interlocked.Increment( ref _refCount );
        }

        /// <summary>
        /// </summary>
        internal int ReleaseReference( )
        {
            return Interlocked.Decrement( ref _refCount );
        }

        /// <summary>
        /// Attempts to receive a datagram. This call blocks until the
        /// receive completes.
        /// </summary>
        internal IAsyncResult BeginReceiveFrom( AsyncCallback callback, object state )
        {
            SoapUdpTransport.Debug( "SoapUdpSocket[" + _localEP.ToString( ) + "] BeginReceive" );

            SoapUdpDatagram datagram = new SoapUdpDatagram( );
            SoapUdpSocketAsyncResult asyncResult = new SoapUdpSocketAsyncResult( _socket, datagram, callback, state );

            _socket.BeginReceiveFrom( datagram.Data, 0, datagram.Data.Length, 0, ref datagram.EndPoint, new AsyncCallback( asyncResult.OnReceive ), null );

            return asyncResult;
        }

        /// <summary>
        /// Bind to the specified endpoint.
        /// </summary>
        void Bind( IPEndPoint localEP )
        {
            if ( null == localEP )
                throw new ArgumentNullException( "localEP" );

            if ( null != _socket )
                throw new InvalidOperationException( "Socket is already bound" );

            IPAddress mcastAddress = null;

            if ( SoapUdpTransport.IsMulticastAddress( localEP.Address ) )
            {
                mcastAddress = localEP.Address;
                _localVia = SoapUdpTransport.ConvertToUri( localEP );

                if ( localEP.AddressFamily == AddressFamily.InterNetwork )
                    _localEP = new IPEndPoint( IPAddress.Any, localEP.Port );
                else
                    _localEP = new IPEndPoint( IPAddress.IPv6Any, localEP.Port );
            }
            else
            {
                _localVia = SoapUdpTransport.ConvertToUri( localEP );
                _localEP = localEP;
            }
            //
            // Create the socket and bind it
            //                
            _socket = new Socket( _localEP.AddressFamily, SocketType.Dgram, ProtocolType.Udp );
            //
            // Set SO_REUSEADDR on the socket
            //
            if ( _options.ReuseAddress )
                _socket.SetSocketOption( SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1 );

            try
            {
                //
                // Normal address
                //
                _socket.Bind( _localEP );                                     // throws SocketException

                MulticastAddress = mcastAddress;
            }
            catch
            {
                _socket.Close( );

                _socket = null;
                _localEP = null;

                throw;
            }
        }

        /// <summary>
        /// Stops the server.
        /// </summary>
        internal void Close( )
        {
            if ( _socket != null )
            {
                SoapUdpTransport.Debug( "Close SoapUdpSocket at " + _localEP.ToString( ) );

                Socket socket = _socket;
                //
                // Ensure no new calls
                //
                _socket = null;

                socket.Shutdown( SocketShutdown.Both );

                socket.Close( );
            }
        }

        /// <summary>
        /// Removes the listener from the specified multicast group.
        /// </summary>
        internal void DropMulticastGroup( IPAddress multicastgroup )
        {
            Debug.Assert( null != _socket );
            Debug.Assert( SoapUdpTransport.IsMulticastAddress( multicastgroup ) );

            if ( null == _socket )
                throw new InvalidOperationException( "_socketet not yet bound" );

            if ( !SoapUdpTransport.IsMulticastAddress( multicastgroup ) )
                throw new InvalidOperationException( "invalid multicast group address" );

            _socket.SetSocketOption( SocketOptionLevel.IP, SocketOptionName.DropMembership, new MulticastOption( multicastgroup ) );
        }

        /// <summary>
        /// Ends an asynchronous receive operation.
        /// </summary>
        internal SoapEnvelope EndReceiveFrom( IAsyncResult ar )
        {
            try
            {
                SoapUdpTransport.Debug( "SoapUdpSocket[" + _localEP.ToString( ) + "] EndReceive" );

                SoapUdpSocketAsyncResult asyncResult = ( SoapUdpSocketAsyncResult )ar;
                SoapEnvelope message;

                AsyncResult.End( asyncResult );

                SoapUdpTransport.Debug( "SoapUdpSocket[" + _localEP.ToString( ) + "] EndReceive " + asyncResult.Datagram.Available + " bytes" );

                MemoryStream stream = new MemoryStream( asyncResult.Datagram.Data, 0, asyncResult.Datagram.Available );

                message = _formatter.Deserialize( stream );
                //
                // Build Addressing defaults using the local Via and remote Via. Setting the 
                // via's correctly is a security measure that prevents dispatching across
                // channels that are registered on different interfaces.
                //
                message.Context.Addressing.SetRequestHeaders( _localVia, SoapUdpTransport.ConvertToUri( asyncResult.Datagram.Source ) );

                return message;
            }
            catch ( Exception ex )
            {
                SoapUdpTransport.Debug( "SoapUdpSocket[" + _localEP.ToString( ) + "] Exception =" + ex.Message );
                SoapUdpTransport.Debug( "SoapUdpSocket[" + _localEP.ToString( ) + "] StackTrace=" + ex.StackTrace );

                throw;
            }
        }

        /// <summary>
        /// Adds the listener to the specified multicast group.
        /// </summary>
        internal void JoinMulticastGroup( IPAddress mcastAddress )
        {
            Debug.Assert( null != _socket );
            Debug.Assert( SoapUdpTransport.IsMulticastAddress( mcastAddress ) );

            if ( null == _socket )
                throw new InvalidOperationException( "_socketet not yet bound" );

            if ( !SoapUdpTransport.IsMulticastAddress( mcastAddress ) )
                throw new InvalidOperationException( "invalid multicast group address" );

            _mcastAddress = mcastAddress;

            _socket.SetSocketOption( SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption( mcastAddress ) );
        }

        /// <summary>
        /// Returns the local endpoint for the server.
        /// </summary>
        internal IPEndPoint LocalEndPoint
        {
            get
            {
                Debug.Assert( null != _socket );

                if ( null == _socket )
                    throw new InvalidOperationException( );

                return ( IPEndPoint )_socket.LocalEndPoint;
            }
        }

        /// <summary>
        /// Returns the current multicast group address or sets a new one.  
        /// </summary>
        internal IPAddress MulticastAddress
        {
            get
            {
                return _mcastAddress;
            }
            set
            {
                //
                // Leave the current group
                //
                if ( _mcastAddress != null )
                    DropMulticastGroup( _mcastAddress );
                //
                // Join a new group
                //
                if ( value != null )
                    JoinMulticastGroup( value );
            }
        }

        /// <summary>
        /// Send does not catch any exceptions - the caller must handle exceptions.
        /// </summary>
        internal void SendTo( SoapEnvelope envelope, EndpointReference destination )
        {
            if ( _localEP != null )
                SoapUdpTransport.Debug( "SoapUdpSocket[" + _localEP.ToString( ) + "] Send" );
            else
                SoapUdpTransport.Debug( "SoapUdpSocket[Unbound] Send" );
            //
            // IPv6: We need to process each of the addresses return from
            //       DNS when trying to connect. Use of AddressList[0] is
            //       bad form.
            //
            string host = destination.TransportAddress.Host;
            int port = ( destination.TransportAddress.Port == -1 ) ? _options.DefaultPort : destination.TransportAddress.Port;

            IPAddress remoteAddress = null;
            IPAddress[ ] remoteAddresses = null;

            try
            {
                if ( string.Compare( host, "localhost", true, CultureInfo.CurrentCulture ) == 0 && Socket.SupportsIPv4 )
                    remoteAddress = IPAddress.Loopback;
                else if ( string.Compare( host, "localhost6", true, CultureInfo.CurrentCulture ) == 0 && Socket.OSSupportsIPv6 )
                    remoteAddress = IPAddress.IPv6Loopback;
                else
                    remoteAddress = IPAddress.Parse( host );

                remoteAddresses = new IPAddress[ 1 ] { remoteAddress };
            }
            catch
            {
                //
                // Intentionally empty - hostname is not a plain IP address.
                // Use DNS to resolve the name.
                //
                remoteAddresses = Dns.GetHostEntry( host ).AddressList;
            }
            //
            // Now form the packet to send.
            //
            MemoryStream stream = new MemoryStream( );

            _formatter.Serialize( envelope, stream );

            System.Diagnostics.Debug.Assert( stream.Length <= SoapUdpDatagram.MaxDataSize );

            if ( stream.Length > SoapUdpDatagram.MaxDataSize )
                throw new ArgumentException( "Message is too large" );

            if ( _localEP != null )
                SoapUdpTransport.Debug( "SoapUdpSocket[" + _localEP.ToString( ) + "] Packet size is " + stream.Length.ToString( ) );
            else
                SoapUdpTransport.Debug( "SoapUdpSocket[Unbound] Packet size is " + stream.Length.ToString( ) );

            for ( int i = 0; i < remoteAddresses.Length; i++ )
            {
                //
                // Set the address family appropriately, create the socket and
                // try to connect.
                //
                remoteAddress = remoteAddresses[ i ];

                if ( _socket == null )
                {
                    //
                    // Unbound case: create a temporary socket to send the data on.
                    //
                    Socket socket = new Socket( remoteAddress.AddressFamily, SocketType.Dgram, ProtocolType.Udp );
                    socket.SendTo( stream.GetBuffer( ), 0, ( int )stream.Length, SocketFlags.None, new IPEndPoint( remoteAddress, port ) );
                    socket.Close( );

                    break;
                }
                else if ( remoteAddress.AddressFamily == _socket.AddressFamily )
                {
                    //
                    // Bound case: use the existing socket to send the data on.
                    //
                    _socket.SendTo( stream.GetBuffer( ), 0, ( int )stream.Length, SocketFlags.None, new IPEndPoint( remoteAddress, port ) );

                    break;
                }
            }
        }

        /// <summary>
        /// Sets the Multicast Time to Live for the _socketet.
        /// </summary>
        internal void SetMulticastTtl( int ttl )
        {
            Debug.Assert( ttl > 0 && ttl < 33, "Time to Live out of range" );
            Debug.Assert( _socket != null, "Socket not initialized" );
            Debug.Assert( _mcastAddress != null, "Not part of multicast group" );

            if ( ttl < 1 || ttl > 32 )
                throw new ArgumentException( "Time to Live is out of range (0..32)" );

            if ( _socket == null || _mcastAddress == null )
                throw new InvalidOperationException( "Socket not initialized or not part of multicast group" );
            //
            // Set the multicast TTL for any outgoing traffic.
            //
            _socket.SetSocketOption( SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, ttl );
        }
    }
}