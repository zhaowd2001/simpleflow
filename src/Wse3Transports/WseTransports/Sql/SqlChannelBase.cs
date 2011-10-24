//WSE 3 Version by Rodolfo Finochietti (ml@pboard.com.ar)
using System;
using System.Data;
using System.Data.SqlClient;

using Microsoft.Web.Services3;
using Microsoft.Web.Services3.Addressing;
using Microsoft.Web.Services3.Messaging;

namespace WseTransports.Sql
{
    public class SqlChannelBase
    {
        public event EventHandler Closed;

        //TODO: Having these protected is not a great idea, really - revisit.
        protected object _lockObject;
        protected EndpointReference _endpoint;
        protected string _unformatedConnectionString;

        private string _connectionString;
        private bool _isClosed;

        protected SqlChannelBase( EndpointReference endpoint )
        {
            _lockObject = new object( );
            _endpoint = endpoint;
        }

        public SoapChannelCapabilities Capabilities
        {
            get
            {
                return SoapChannelCapabilities.None;
            }
        }

        public void Close( )
        {
            if ( _isClosed )
                throw new InvalidOperationException( "Sql Channel already closed" );

            _isClosed = true;

            if ( Closed != null )
                Closed( this, EventArgs.Empty );
        }

        public bool IsClosed
        {
            get
            {
                return _isClosed;
            }
        }

        protected SqlConnection CreateConnection( )
        {
            SqlConnection con = new SqlConnection( ConnectionString );

            return con;
        }

        protected string ConnectionString
        {
            get
            {
                lock ( _lockObject )
                {
                    if ( _connectionString == null )
                        _connectionString = SqlUtility.EndpointToConnectionString( _endpoint, _unformatedConnectionString );
                    return _connectionString;
                }
            }
        }

        protected void CheckClosed( )
        {
            if ( _isClosed )
            {
                throw new InvalidOperationException( "Sql Channel is closed" );
            }
        }
    }
}