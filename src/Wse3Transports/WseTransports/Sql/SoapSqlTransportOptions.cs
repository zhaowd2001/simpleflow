//WSE 3 Version by Rodolfo Finochietti (ml@pboard.com.ar)
using System;
using System.Globalization;

namespace WseTransports.Sql
{
    /// <summary>
    /// Options for the Sql transport.
    /// </summary>
    /// <author>Rodolfo Finochietti (ml@pboard.com.ar)</author>
    /// <remarks>Added to WSE 3 version</remarks>
    public sealed class SoapSqlTransportOptions : ICloneable
    {
        string _connectionString = "Server={0};Database=SoapMessageBox;Trusted_Connection=True;";

        public SoapSqlTransportOptions( ) { }

        public object Clone( )
        {
            SoapSqlTransportOptions clone = new SoapSqlTransportOptions( );

            clone.ConnectionString = _connectionString;

            return clone;
        }

        public string ConnectionString
        {
            get
            {
                return _connectionString;
            }
            set
            {
                _connectionString = value;
            }
        }
    }
}