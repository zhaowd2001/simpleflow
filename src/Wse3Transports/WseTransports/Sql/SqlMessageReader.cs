//WSE 3 Version by Rodolfo Finochietti (ml@pboard.com.ar)
using System;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Threading;

using Microsoft.Web.Services3;
using Microsoft.Web.Services3.Addressing;

namespace WseTransports.Sql
{
    public class SqlMessageReader
    {
        private const int INTERVAL = 250;
        private static readonly string STORED_PROC = "GetMessages";
        private static readonly string MESSAGEBOX_TABLE = "MessageBox";
        private static readonly string SMALL_MESSAGE_COLUMN = "SmallSoapMessage";
        private static readonly string LARGE_MESSAGE_COLUMN = "LargeSoapMessage";
        private static readonly string ENDPOINT_COLUMN = "Endpoint";
        private static readonly string ENDPOINTS_PARAMETER = "@Endpoints";

        private static Hashtable _endpointReaders;

        private Timer _timer;
        private string _server;
        private string _connectionString;
        private string _unformatedConnectionString;
        private EndpointCollection _endpoints;

        #region SqlMessageAyncResult

        private class SqlMessageAsyncResult : IAsyncResult
        {
            private bool _completed;
            private AsyncCallback _callback;
            private AutoResetEvent _event;
            private object _state;
            private SoapEnvelope _envelope;

            public SqlMessageAsyncResult( AsyncCallback callback, object state )
            {
                _state = state;
                _callback = callback;
                _event = new AutoResetEvent( false );
            }

            public object AsyncState
            {
                get
                {
                    return _state;
                }
            }

            public bool CompletedSynchronously
            {
                get
                {
                    return false;
                }
            }

            public System.Threading.WaitHandle AsyncWaitHandle
            {
                get
                {
                    return _event;
                }
            }
            
            public bool IsCompleted
            {
                get
                {
                    return _completed;
                }
            }
            
            public SoapEnvelope Envelope
            {
                get
                {
                    return _envelope;
                }
                set
                {
                    _envelope = value;
                }
            }

            public bool Completed
            {
                set
                {
                    _completed = value;

                    if ( _completed )
                        _event.Set( );
                }
            }

            public AsyncCallback Callback
            {
                get
                {
                    return _callback;
                }
            }
        }

        #endregion

        #region EndpointCollection

        private class EndpointCollection
        {
            private Hashtable _endpoints;

            private class EndpointEntry
            {
                public Queue OutstandingRequests;
                public Queue OutstandingEnvelopes;

                public EndpointEntry( )
                {
                    OutstandingRequests = new Queue( );
                    OutstandingEnvelopes = new Queue( );
                }
            }

            public EndpointCollection( )
            {
                _endpoints = new Hashtable( );
            }
            
            public void AddRequest( string endpoint, SqlMessageAsyncResult result )
            {
                lock ( _endpoints )
                {
                    if ( _endpoints[ endpoint ] == null )
                        _endpoints[ endpoint ] = new EndpointEntry( );

                    EndpointEntry e = ( EndpointEntry )_endpoints[ endpoint ];

                    e.OutstandingRequests.Enqueue( result );
                }
            }

            public void DispatchMessages( )
            {
                lock ( _endpoints )
                {
                    foreach ( string key in _endpoints.Keys )
                        DispatchMessages( key );
                }
            }

            public void DispatchMessages( string endpoint )
            {
                lock ( _endpoints )
                {
                    EndpointEntry e = ( EndpointEntry )_endpoints[ endpoint ];

                    while ( ( e.OutstandingEnvelopes.Count > 0 ) && ( e.OutstandingRequests.Count > 0 ) )
                    {
                        SqlMessageAsyncResult result = ( SqlMessageAsyncResult )e.OutstandingRequests.Dequeue( );

                        result.Envelope = ( SoapEnvelope )e.OutstandingEnvelopes.Dequeue( );

                        result.Completed = true;

                        if ( result.Callback != null )
                            ThreadPool.QueueUserWorkItem( new WaitCallback( ThreadPoolCallback ), result );
                    }
                }
            }

            private void ThreadPoolCallback( object o )
            {
                SqlMessageAsyncResult result = ( SqlMessageAsyncResult )o;

                result.Callback( result );
            }

            public void AddMessage( string endpoint, SoapEnvelope message )
            {
                lock ( _endpoints )
                {
                    EndpointEntry e = ( EndpointEntry )_endpoints[ endpoint ];

                    e.OutstandingEnvelopes.Enqueue( message );
                }
            }

            public string[ ] GetEndpointsWithOutstandingMessageRequests( )
            {
                string[ ] endpoints = null;

                lock ( _endpoints )
                {
                    if ( _endpoints.Count > 0 )
                    {
                        endpoints = new string[ _endpoints.Count ];

                        _endpoints.Keys.CopyTo( endpoints, 0 );
                    }
                }
                return endpoints;
            }
        }

        #endregion

        private SqlMessageReader( string server, string unformatedConnectionString )
        {
            _server = server;
            _unformatedConnectionString = unformatedConnectionString;
            _endpoints = new EndpointCollection( );
        }

        public IAsyncResult BeginMessageReceive( string endpoint, AsyncCallback callback, object state )
        {
            SqlMessageAsyncResult result = new SqlMessageAsyncResult(callback, state );

            _endpoints.AddRequest( endpoint, result );

            if ( _timer == null )
                _timer = new Timer( new TimerCallback( OnTimerTick ), null, 0, INTERVAL );

            _endpoints.DispatchMessages( endpoint );

            return result;
        }

        public SoapEnvelope EndMessageReceive( IAsyncResult result )
        {
            SqlMessageAsyncResult sqlResult = ( SqlMessageAsyncResult )result;

            return sqlResult.Envelope;
        }
        
        private void OnTimerTick( object state )
        {
            DataSet ds = GetLatestMessagesForEndpoints( );

            if ( ( ds != null ) && ( ds.Tables.Count > 0 ) && ( ds.Tables[ MESSAGEBOX_TABLE ].Rows.Count > 0 ) )
            {
                foreach ( DataRow r in ds.Tables[ MESSAGEBOX_TABLE ].Rows )
                {
                    SoapEnvelope env = new SoapEnvelope( );

                    if ( r.IsNull( LARGE_MESSAGE_COLUMN ) )
                        env.LoadXml( r[ SMALL_MESSAGE_COLUMN ].ToString( ) );
                    else
                        env.LoadXml( r[ LARGE_MESSAGE_COLUMN ].ToString( ) );

                    _endpoints.AddMessage( r[ ENDPOINT_COLUMN ].ToString( ), env );
                }
            }
            _endpoints.DispatchMessages( );
        }

        private DataSet GetLatestMessagesForEndpoints( )
        {
            DataSet ds = null;

            string endpointList = BuildEndpointList( );

            if ( endpointList != null )
            {
                using ( SqlConnection con = new SqlConnection( ConnectionString ) )
                {
                    con.Open( );

                    SqlCommand com = new SqlCommand( STORED_PROC, con );

                    com.CommandType = CommandType.StoredProcedure;

                    com.Parameters.Add( ENDPOINTS_PARAMETER, SqlDbType.NVarChar, 4000 ).Value = endpointList;

                    SqlDataAdapter adapter = new SqlDataAdapter( com );

                    ds = new DataSet( MESSAGEBOX_TABLE );

                    adapter.Fill( ds, MESSAGEBOX_TABLE );

                    con.Close( );
                }
            }

            return ds;
        }

        private string BuildEndpointList( )
        {
            string[ ] endpoints = _endpoints.GetEndpointsWithOutstandingMessageRequests( );
            string endpointList = null;

            if ( endpoints != null )
            {
                StringBuilder builder = new StringBuilder( );

                bool first = true;

                foreach ( string s in endpoints )
                {
                    builder.AppendFormat( "{0}{1}", first ? "" : ",", s );

                    first = false;
                }
                endpointList = builder.ToString( );
            }
            return endpointList;
        }

        private string ConnectionString
        {
            get
            {
                if ( _connectionString == null )
                    _connectionString = SqlUtility.HostnameToConnectionString( _server, _unformatedConnectionString );
                
                return _connectionString;
            }
        }

        static SqlMessageReader( )
        {
            _endpointReaders = new Hashtable( );
        }

        public static SqlMessageReader GetReaderForServer( string server, string unformatedConnectionString )
        {
            lock ( _endpointReaders )
            {
                SqlMessageReader reader = _endpointReaders[ server ] as SqlMessageReader;

                if ( reader == null )
                {
                    reader = new SqlMessageReader( server, unformatedConnectionString );

                    _endpointReaders[ server ] = reader;
                }
        
                return reader;
            }
        }
    }
}