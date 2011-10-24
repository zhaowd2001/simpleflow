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
using System.Globalization;

namespace WseTransports.Udp
{
    /// <summary>
    /// Options for the UDP transport.
    /// </summary>
    public sealed class SoapUdpTransportOptions : ICloneable
    {
        int _defaultPort = 8081;
        bool _exclusiveUse = true;
        bool _reuse = false;

        /// <summary>
        /// To Be Provided.
        /// </summary>
        public SoapUdpTransportOptions( ) { }

        /// <summary>
        /// To Be Provided.
        /// </summary>
        public object Clone( )
        {
            SoapUdpTransportOptions clone = new SoapUdpTransportOptions( );

            clone.DefaultPort = _defaultPort;
            clone.ExclusiveAddressUse = _exclusiveUse;
            clone.ReuseAddress = _reuse;

            return clone;
        }

        /// <summary>
        /// To Be Provided.
        /// </summary>
        public int DefaultPort
        {
            get
            {
                return _defaultPort;
            }
            set
            {
                if ( value <= 0 )
                    throw new ArgumentOutOfRangeException( "DefaultPort" );

                _defaultPort = value;
            }
        }

        /// <summary>
        /// To Be Provided.
        /// </summary>
        public bool ExclusiveAddressUse
        {
            get
            {
                return _exclusiveUse;
            }
            set
            {
                _exclusiveUse = value;

                if ( _exclusiveUse )
                    _reuse = false;
            }
        }

        /// <summary>
        /// To Be Provided.
        /// </summary>
        public bool ReuseAddress
        {
            get
            {
                return _reuse;
            }
            set
            {
                _reuse = value;

                if ( _reuse )
                    _exclusiveUse = false;
            }
        }
    }
}