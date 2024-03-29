﻿using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using AMicroblogAPI.Common;
using AMicroblogAPI.DataContract;

namespace AMicroblogAPI.HttpRequests
{
    /// <summary>
    /// Represents the base class for HTTP requests.
    /// </summary>
    public abstract class HttpRequest : IHttpRequest
    {
        /// <summary>
        /// The uri the request goes to.
        /// </summary>
        protected string Uri;

        /// <summary>
        /// Initializes a new instance of <see cref="HttpRequest"/> with the specified <paramref name="uri"/>.
        /// </summary>
        /// <param name="uri">The uri the request goes to.</param>
        protected HttpRequest(string uri)
        {
            this.Uri = uri;
        }

        /// <summary>
        /// Performs the HTTP request.
        /// </summary>
        /// <returns>The server response string(UTF8 decoded).</returns>
        public virtual string Request()
        {
            HttpWebRequest req = HttpWebRequest.Create(ConstructUri()) as HttpWebRequest;

            AppendHeaders(req.Headers);

            if (!string.IsNullOrEmpty(Method))
            {
                req.Method = Method;
            }

            if (!string.IsNullOrEmpty(ContentType))
            {
                req.ContentType = ContentType;
            }

            PrepareRequest(req);

            // Calls GetRequestStream with a "GET" method, an exception is thrown "Cannot send a content-body with this verb-type."
            if (req.Method == HttpMethod.Post)
            {
                using (var reqStream = req.GetRequestStream())
                {
                    WriteBody(reqStream);
                }

                // ContentLength is automatically calculated.
            }

            WebResponse resp = null;
            try
            {
                resp = req.GetResponse();
            }
            catch (WebException wex)
            {
                ErrorResponse errorResp;
                var webResp = wex.Response as HttpWebResponse;
                if (null == webResp)
                    throw new AMicroblogException(LocalErrorCode.NetworkUnavailable);

                try
                {
                    var responseContent = RetrieveResponse(webResp);

                    if (!string.IsNullOrEmpty(responseContent) && responseContent.StartsWith(Constants.XmlHeader, StringComparison.InvariantCultureIgnoreCase))
                    {
                        errorResp = XmlSerializationHelper.XmlToObject<ErrorResponse>(responseContent);
                        // Retrieves the error code and wraps the exception.
                        var errorCode = AMicroblogException.RetrieveErrorCode(errorResp.ErrorMessage);
                        if (!string.IsNullOrEmpty(errorCode))
                            errorResp.ErrorCode = int.Parse(errorCode);
                    }
                    else
                    {
                        // Handle other cases
                        errorResp = new ErrorResponse() { ErrorCode = (int)webResp.StatusCode, ErrorMessage = webResp.StatusDescription };
                    }
                }
                catch (Exception exx)
                {
                    throw new AMicroblogException(LocalErrorCode.ProcessResponseErrorHandlingFailed, exx);
                }

                var aex = new AMicroblogException(errorResp.ErrorMessage, wex) { IsRemoteError = true, ErrorCode = errorResp.ErrorCode };

                var e = new ResponseErrorEventArgs(req.RequestUri.AbsoluteUri, aex, req.Method, req.ContentType);
                Environment.NotifyResponseError(e);

                aex.IsHandled = e.IsHandled;

                throw aex;
            }

            return RetrieveResponse(resp);
        }

        /// <summary>
        /// The asynchronous implementation of <see cref="IHttpRequest.Request"/>
        /// </summary>
        /// <param name="callback">The callback to invoke when the async requests completes.</param>
        public void RequestAsync(AsyncCallback<string> callback)
        {
            var thread = new Thread(new ParameterizedThreadStart(DoRequest));
            thread.IsBackground = true;
            thread.Name = string.Format("RequestingThread:{0}", Uri);
            thread.Start(callback);
        }

        /// <summary>
        /// Performs the async request.
        /// </summary>
        /// <param name="state"></param>
        private void DoRequest(object state)
        {
            var callback = state as AsyncCallback<string>;
            AsyncCallResult<string> reqResult = new AsyncCallResult<string>();

            try
            {
                reqResult.Data = Request();
                reqResult.Success = true;
            }
            catch (Exception ex)
            {
                reqResult.Success = false;
                reqResult.Exception = ex;
            }
            finally
            {
                callback(reqResult);
            }
        }

        /// <summary>
        /// When overriden in derived classes, constructs the final request uri based on relevent conditions.
        /// </summary>
        /// <returns>The final request uri constucted.</returns>
        protected virtual string ConstructUri()
        {
            return Uri;
        }

        /// <summary>
        /// When overridden in derived classes, appends HTTP headers into the request.
        /// </summary>
        /// <param name="headers">The web header collection object.</param>
        protected virtual void AppendHeaders(WebHeaderCollection headers)
        {
            // Nothing to do.
        }

        /// <summary>
        /// When overridden in derived classes, sets additional request arguments on the web request object.
        /// </summary>
        /// <param name="webRequest">The web request object.</param>
        protected virtual void PrepareRequest(HttpWebRequest webRequest)
        {
            // Nothing to do.
        }

        /// <summary>
        /// When overridden in derived classes, writes data into the request stream.
        /// <remarks>HTTP GET type request is not supported.</remarks>
        /// </summary>
        /// <param name="reqStream">The request stream object.</param>
        protected virtual void WriteBody(Stream reqStream)
        {
            // Nothing to do. 
        }

        /// <summary>
        /// Retrieves the response from the request.
        /// </summary>
        /// <param name="webResponse"></param>
        /// <returns>The server response string(UTF8 decoded).</returns>
        protected virtual string RetrieveResponse(WebResponse webResponse)
        {
            var respContent = string.Empty;
            var respStream = webResponse.GetResponseStream();
            using (StreamReader reader = new StreamReader(respStream, Encoding.UTF8))
            {
                respContent = reader.ReadToEnd();
            }
            webResponse.Close();

            return respContent;
        }

        /// <summary>
        /// Gets or sets the HTTP method of the request.(i.e: POST, GET etc.)
        /// </summary>
        protected virtual string Method
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the content type of the request.(i.e: text/plain etc.)
        /// </summary>
        protected virtual string ContentType
        {
            get;
            set;
        }
    }
}
