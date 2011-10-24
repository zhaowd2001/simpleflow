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
    /// Class that represents a UDP datagram.
    /// </summary>
    /// <remarks>
    /// Based on the Hervey Wilson work.
    /// </remarks>
    public sealed class SoapUdpDatagram
    {
        public const int MaxDataSize = 8192;

        /// <summary>
        /// Number of bytes of data that are available.
        /// </summary>
        internal int Available;
        /// <summary>
        /// The data in the datagram.
        /// </summary>
        internal byte[ ] Data;
        /// <summary>
        /// EndPoint object, used with Socket.EndReceiveFrom to map
        /// between EndPoint and IPEndPoint types.
        /// </summary>
        internal EndPoint EndPoint;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SoapUdpDatagram( )
        {
            Available = 0;
            Data = new byte[ MaxDataSize ];
            EndPoint = new IPEndPoint( 0, 0 );
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public SoapUdpDatagram( byte[ ] data, IPEndPoint endpoint )
        {
            if ( data.Length > MaxDataSize )
                throw new ArgumentException( "MaxDataSize exceeded" );

            Available = 0;
            Data = data;
            EndPoint = endpoint;
        }

        /// <summary>
        /// The source / destination endpoint. When sending a datagram
        /// this property contains the target endpoint. When receiving
        /// a datagram, this property contains the source endpoint.
        /// </summary>
        public IPEndPoint Source
        {
            get
            {
                return ( IPEndPoint )EndPoint;
            }
            set
            {
                EndPoint = value;
            }
        }
    }
}