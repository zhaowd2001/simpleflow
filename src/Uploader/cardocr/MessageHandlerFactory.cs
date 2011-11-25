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
            JobInfo j = buildJobInfo(message);
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

        private static JobInfo buildJobInfo(string message)
        {
            if (message.StartsWith("{") &&
                message.EndsWith("}"))
            {
                JobInfo j = (JobInfo)Newtonsoft.Json.JsonConvert.DeserializeObject<JobInfo>(message);
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
            s_services.Add(c.getAppID() + "-" + c.getVersion(), c);
        }
    }
}
