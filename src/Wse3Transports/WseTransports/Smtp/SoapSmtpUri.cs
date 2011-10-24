/*
 * Author: Steve Maine
 * Email: stevem@hyperthink.net
 * Web: http://hyperthink.net/blog
 *
 * This work is licensed under the Creative Commons Attribution License. 
 * To view a copy of this license, visit http://creativecommons.org/licenses/by/2.0/ 
 * or send a letter to Creative Commons, 559 Nathan Abbott Way, Stanford, California 94305, USA.
 * 
 * No warranties expressed or implied.
 */
//WSE 3 Version by Rodolfo Finochietti (ml@pboard.com.ar)
using System;

namespace WseTransports.Smtp
{
    //A few static helper methods for dealing with SoapSmtp uris.
    public class SoapSmtpUri
    {
        //The URI scheme for soap.smtp uri's
        public static string UriScheme = "soap.smtp";

        //Converts a POP3 email address to an equivalent soap.smtp:// uri
        public static Uri UriFromAddress( string address )
        {
            if ( address.IndexOf( "@" ) < 0 )
                throw new ArgumentException( "Invalid POP3 address", "address" );

            return new Uri( String.Format( "{0}://{1}", SoapSmtpUri.UriScheme, address ) );
        }

        //Converts a soap.smtp:// uri to the equivalent POP3 address
        public static string AddressFromUri( Uri uri )
        {
            if ( uri.Scheme != SoapSmtpUri.UriScheme )
                throw new ArgumentException( "Invalid soap.smtp:// uri", "uri" );

            string uriString = uri.ToString( );

            //+3 for the ://
            uriString = uriString.Remove( 0, SoapSmtpUri.UriScheme.Length + 3 );

            //Make sure to remove the trailing "/" 
            return uriString.Replace( "/", "" );
        }
    }
}