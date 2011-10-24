//WSE 3 Version by Rodolfo Finochietti (ml@pboard.com.ar)
using System;

using Microsoft.Web.Services3;
using Microsoft.Web.Services3.Addressing;
using Microsoft.Web.Services3.Messaging;

namespace WseTransports.Sql
{
    public class SoapSqlInputChannel : SqlChannelBase, ISoapInputChannel
    {
        private SqlMessageReader _reader;

        internal SoapSqlInputChannel( EndpointReference endpoint, string unformatedConnectionString ) : base( endpoint )
        {
            _endpoint = endpoint;
            _unformatedConnectionString = unformatedConnectionString;
            _reader = SqlMessageReader.GetReaderForServer( endpoint.TransportAddress.Host, unformatedConnectionString );
        }

        public IAsyncResult BeginReceive( System.AsyncCallback callback, object state )
        {
            return _reader.BeginMessageReceive( _endpoint.TransportAddress.PathAndQuery, callback, state );
        }
        
        public EndpointReference LocalEndpoint
        {
            get
            {
                return _endpoint;
            }
        }
        
        public SoapEnvelope Receive( )
        {
            IAsyncResult result = BeginReceive( null, null );

            result.AsyncWaitHandle.WaitOne( );

            return EndReceive( result );
        }
 
        public SoapEnvelope EndReceive( IAsyncResult result )
        {
            return _reader.EndMessageReceive( result );
        }
    }
}