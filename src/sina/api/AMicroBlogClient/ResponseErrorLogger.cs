using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AMicroblogAPI.Common;
using System.IO;

namespace AMicroblogAPISample
{
    /// <summary>
    /// Provides methods to log any response error into ResponseError.txt.
    /// </summary>
    public class ResponseErrorLogger : IResponseErrorHandler
    {
        private const string format = "[{0}]    [{1}] [{2}] [{3}] {4}\r\n";
        public void Handle(ResponseErrorData errorData)
        {
            var exMsg = errorData.Exception.Message;
            var uri = errorData.RequestUri.Replace("http://api.t.sina.com.cn/", string.Empty);
            var content = string.Format(format, DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), uri, errorData.HttpMethod, errorData.ErrorCode, exMsg);
            File.AppendAllText("ResponseError.txt", content);
        }
    }
}
