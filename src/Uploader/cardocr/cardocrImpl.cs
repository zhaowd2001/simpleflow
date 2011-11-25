using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cardocr
{
    public class CardOcrImpl : IMessageHandler
    {
        public string Execute(string message)
        {
            string ret = "result:";
            //
            string[] fields = message.Split(new char[] { '`' });
            string remoteFilePath = fields[3];

            ret += remoteFilePath;
            //
            //
            return ret;
        }

        public string getAppID()
        {
            return "cardocr";
        }
        public string getVersion()
        {
            return "1.0";
        }
    }
}
