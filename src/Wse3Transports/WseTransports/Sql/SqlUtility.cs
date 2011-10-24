//WSE 3 Version by Rodolfo Finochietti (ml@pboard.com.ar)
using System;
using System.Globalization;

using Microsoft.Web.Services3;
using Microsoft.Web.Services3.Addressing;

namespace WseTransports.Sql
{
    public class SqlUtility
    {
        private SqlUtility( ) { }
        
        public static string EndpointToConnectionString( EndpointReference endpoint, string connectionString )
        {
            return HostnameToConnectionString( endpoint.TransportAddress.Host, connectionString );
        }

        public static string HostnameToConnectionString( string hostName, string connectionString )
        {
            return string.Format( CultureInfo.InvariantCulture, connectionString, hostName );
        }
    }
}