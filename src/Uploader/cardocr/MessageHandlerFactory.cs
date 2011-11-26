using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cardocr
{
    public class MessageHandlerFactory
    {
        static Dictionary<string, IMessageHandler> s_services = new Dictionary<string, IMessageHandler>();
        public static IMessageHandler findHandler(string message, out JobInfo job)
        {
            job = null;
            if (message == null)
                return null;
            //
            JobInfo j = decodeJobInfo(message);
            if (j == null)
                return null;
            job = j;

            string appKey = buildAppID(j);
            if (appKey == null)
                return null;

            if (s_services.ContainsKey(appKey))
                return s_services[appKey];
            return null;
        }

        static string decodeFilePathForJson(string path)
        {
            return path.Replace('/', '\\');
        }

        private static JobInfo decodeJobInfo(string message)
        {
            if (message.StartsWith("{") &&
                message.EndsWith("}"))
            {
                JobInfo j = (JobInfo)Newtonsoft.Json.JsonConvert.DeserializeObject<JobInfo>(message);
                //
                j.RemoteFilePath = decodeFilePathForJson(j.RemoteFilePath);
                //
                return j;
            }

            return null;
        }

        public static string buildAppID(JobInfo j)
        {
            if (j == null)
                return null;
            return j.AppID + "-" + j.Version;
        }

        //
        public static void init()
        {
            CardOcrImpl c = new CardOcrImpl();
            string key = c.getAppID() + "-" + c.getVersion();
            if (s_services.ContainsKey(key))
                s_services.Remove(key);
            s_services.Add(key, c);
        }
    }
}
