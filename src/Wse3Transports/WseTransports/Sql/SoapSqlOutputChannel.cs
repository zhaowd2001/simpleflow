//WSE 3 Version by Rodolfo Finochietti (ml@pboard.com.ar)
using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;

using Microsoft.Web.Services3;
using Microsoft.Web.Services3.Addressing;
using Microsoft.Web.Services3.Messaging;

namespace WseTransports.Sql
{
    public class SoapSqlOutputChannel : SqlChannelBase, ISoapOutputChannel
    {
        private SendMessageDelegateType _sendDelegate;

        private const int LARGE_MESSAGE = 2048;
        private const string SMALL_MESSAGE_SP = "InsertSmallMessage";
        private const string LARGE_MESSAGE_SP = "InsertLargeMessage";
        private const string MSG_PARAM_NAME = "@Message";
        private const string EP_PARAM_NAME = "@Endpoint";

        private delegate void SendMessageDelegateType( SoapEnvelope envelope );

        internal SoapSqlOutputChannel( EndpointReference endpoint, string unformatedConnectionString ) : base( endpoint )
        {
            _endpoint = endpoint;
            _unformatedConnectionString = unformatedConnectionString;
            _lockObject = new object( );
        }

        public void Send( SoapEnvelope message )
        {
            CheckClosed( );

            SendMessage( message );
        }

        public void EndSend( IAsyncResult result )
        {
            CheckClosed( );

            SendMessageDelegate.EndInvoke( result );
        }

        public EndpointReference RemoteEndpoint
        {
            get
            {
                return _endpoint;
            }
        }

        public IAsyncResult BeginSend( SoapEnvelope message, AsyncCallback callback, object state )
        {
            CheckClosed( );

            return SendMessageDelegate.BeginInvoke( message, callback, state );
        }

        private SqlParameter CreateMessageParameter( string msgBody, bool smallMessage )
        {
            SqlParameter p = new SqlParameter( );

            p.ParameterName = MSG_PARAM_NAME;
            p.SqlDbType = smallMessage ? SqlDbType.NVarChar : SqlDbType.NText;
            p.Value = msgBody;

            return p;
        }

        private SqlParameter CreateEndpointParameter( )
        {
            SqlParameter p = new SqlParameter( );

            p.ParameterName = EP_PARAM_NAME;
            p.SqlDbType = SqlDbType.NVarChar;
            p.Value = _endpoint.TransportAddress.PathAndQuery;

            return p;
        }

        private SqlCommand CreateCommand( SqlConnection connection, SoapEnvelope message )
        {
            string msgBody = message.DocumentElement.OuterXml;

            bool small = msgBody.Length < LARGE_MESSAGE;

            SqlCommand com = new SqlCommand( small ? SMALL_MESSAGE_SP : LARGE_MESSAGE_SP, connection );

            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Add( CreateEndpointParameter( ) );
            com.Parameters.Add( CreateMessageParameter( msgBody, small ) );

            return com;
        }
        
        private void SendMessage( SoapEnvelope message )
        {
            using ( SqlConnection con = CreateConnection( ) )
            {
                con.Open( );

                using ( SqlCommand com = CreateCommand( con, message ) )
                {
                    com.ExecuteNonQuery( );
                }
                con.Close( );
            }
        }

        private SendMessageDelegateType SendMessageDelegate
        {
            get
            {
                lock ( _lockObject )
                {
                    if ( _sendDelegate == null )
                        _sendDelegate = new SendMessageDelegateType( SendMessage );

                    return _sendDelegate;
                }
            }
        }
    }
}