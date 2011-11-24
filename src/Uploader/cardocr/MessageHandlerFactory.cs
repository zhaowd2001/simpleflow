using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cardocr
{
    public class MessageHandlerFactory
    {
        static Dictionary<string, IMessageHandler> s_services = new Dictionary<string, IMessageHandler>();
        public static IMessageHandler findHandler(string message)
        {
            if (message == null)
                return null;

            string[] fields = splitMessage(message);
            string appKey = buildAppID(fields);
            if (appKey == null)
                return null;

            if (s_services.ContainsKey(appKey))
                return s_services[appKey];

            return null;
        }

        public static string buildAppID(string[] fields)
        {
            if(fields.Length >= 3)
                return fields[1] + "-" + fields[2];
            return null;
        }

        public static string[] splitMessage(string message)
        {
            return message.Split(new char[] { '`' });
        }
        //
        public static void init()
        {
            CardOcrImpl c = new CardOcrImpl();
            s_services.Add(c.getAppID() + "-" + c.getVersion(), c);
        }
    }
}
