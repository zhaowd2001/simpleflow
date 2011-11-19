using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AMicroblogAPI.Common;
using System.IO;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace AMicroblogAPISample
{
    public static class CredentialHelper
    {
        private const string tknFile = "LastUser.tkn";
        public static void Save(string userName, OAuthAccessToken accessToken)
        {
            try
            {
                var content = string.Format("{0},{1},{2},{3}", userName, accessToken.UserID, accessToken.Token, accessToken.Secret);

                var encryptedText = SimplyEncrypt(content);

                File.WriteAllText(tknFile, encryptedText);
            }
            catch
            {
                // Nothing to do.
            }
        }

        public static OAuthAccessToken Get()
        {
            OAuthAccessToken accessToken = null;
            try
            {
                if (File.Exists(tknFile))
                {
                    var text = File.ReadAllText(tknFile);

                    var content = SimplyDecrypt(text);

                    var match = Regex.Match(content, "^(.+?),(.+?),(.+?),(.+?)$", RegexOptions.IgnoreCase);
                    accessToken = new OAuthAccessToken();
                    AMicroblogAPI.Environment.AccessToken = accessToken;
                    AMicroblogAPI.Environment.CurrentUserAccount = match.Groups[1].Value;
                    accessToken.UserID = match.Groups[2].Value;
                    accessToken.Token = match.Groups[3].Value;
                    accessToken.Secret = match.Groups[4].Value;
                }
            }
            catch
            {
                // Nothing to do.
            }

            return accessToken;
        }

        private const string Secret = "AMicrobl"; // 8 bytes of key size
        private const string IV = "ALDRICKWAN";  // at least 8 bytes of IV
        public static string SimplyEncrypt(string rawContent)
        {
            var des = new DESCryptoServiceProvider();
            var encryptor = des.CreateEncryptor(Encoding.ASCII.GetBytes(Secret), Encoding.ASCII.GetBytes(IV));

            var dataToEnc = Encoding.UTF8.GetBytes(rawContent);
            var resultStr = encryptor.TransformFinalBlock(dataToEnc, 0, dataToEnc.Length);

            return Convert.ToBase64String(resultStr);
        }

        public static string SimplyDecrypt(string encryptedContent)
        {
            var result = string.Empty;

            var des = new DESCryptoServiceProvider();
            var decryptor = des.CreateDecryptor(Encoding.ASCII.GetBytes(Secret), Encoding.ASCII.GetBytes(IV));

            var dataToDec = Convert.FromBase64String(encryptedContent);
            var resultBytes = decryptor.TransformFinalBlock(dataToDec, 0, dataToDec.Length);

            result = Encoding.UTF8.GetString(resultBytes);

            return result;
        }

    }
}
