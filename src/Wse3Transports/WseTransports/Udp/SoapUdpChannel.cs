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
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Threading;

using Microsoft.Web.Services3;
using Microsoft.Web.Services3.Addressing;
using Microsoft.Web.Services3.Messaging;

using log4net;
using log4net.Config;

namespace WseTransports.Udp
{
    /// <summary>
    /// SoapUdpInputChannel.
    /// </summary>
    /// <remarks>
    /// Based on the Hervey Wilson work.
    /// </remarks>
    public sealed class SoapUdpInputChannel : SoapInputChannel, ISoapOutputChannel
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SoapUdpInputChannel));

        EndpointReference _remoteEndpoint;
        SoapUdpSocket _socket;
        SoapUdpTransport _transport;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="endpoint">The local endpoint for the channel.</param>
        /// <param name="socket">The socket for the channel.</param>
        /// <param name="transport">The transport for the channel.</param>
        internal SoapUdpInputChannel( EndpointReference endpoint, SoapUdpSocket socket, SoapUdpTransport transport )
            : base( endpoint )
        {
            Debug.Assert( socket != null );
            Debug.Assert( transport != null );

            _remoteEndpoint = null;
            _socket = socket;
            _transport = transport;
        }

        #region ISoapChannel Overrides

        /// <summary>
        /// </summary>
        public new SoapChannelCapabilities Capabilities
        {
            get
            {
                return SoapChannelCapabilities.ActivelyListening;
            }
        }

        /// <summary>
        /// </summary>
        public override void Close( )
        {
            if ( !this.IsClosed )
            {
                base.Close( );
                //
                // Tell the transport that this channel is closed.
                //
                _transport.CloseInputChannel( this );
            }
        }

        #endregion

        #region ISoapOutputChannel Implementation

        /// <summary>
        /// </summary>
        public IAsyncResult BeginSend( SoapEnvelope message, AsyncCallback callback, object state )
        {
            throw new NotImplementedException( );
        }

        /// <summary>
        /// </summary>
        public void EndSend( IAsyncResult result )
        {
            throw new NotImplementedException( );
        }

        /// <summary>
        /// </summary>
        public EndpointReference RemoteEndpoint
        {
            get
            {
                return _remoteEndpoint;
            }
            set
            {
                _remoteEndpoint = value;
            }
        }

        /// <summary>
        /// </summary>
        public void Send( SoapEnvelope message )
        {
            if ( message == null )
                throw new ArgumentNullException( "message" );

            _socket.SendTo( message, RemoteEndpoint );
        }

        #endregion
    }

    /// <summary>
    /// SoapUdpOutputChannel.
    /// </summary>
    public sealed class SoapUdpOutputChannel : SoapOutputChannel
    {
        SoapUdpSocket _socket;
        SoapUdpTransport _transport;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="endpoint">The remote endpoint for the channel.</param>
        /// <param name="socket">The socket for the channel.</param>
        /// <param name="transport">The transport for the channel.</param>
        internal SoapUdpOutputChannel( EndpointReference endpoint, SoapUdpSocket socket, SoapUdpTransport transport ) : base( endpoint )
        {
            Debug.Assert( socket != null );
            Debug.Assert( transport != null );

            _socket = socket;
            _transport = transport;
        }

        /// <summary>
        /// Returns the capabilities of the channel.
        /// </summary>
        public new SoapChannelCapabilities Capabilities
        {
            get
            {
                return SoapChannelCapabilities.None;
            }
        }

        #region ISoapOutputChannel Overrides

        /// <summary>
        /// Sends the specified message to the RemoteEndpoint.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        public override void Send( SoapEnvelope message )
        {
            if ( message == null )
                throw new ArgumentNullException( "message" );

            _socket.SendTo( message, RemoteEndpoint );
        }

        #endregion
    }
}