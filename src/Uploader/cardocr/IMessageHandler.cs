using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cardocr
{
    public interface IMessageHandler
    {
        string getAppID();
        string getVersion();

        JobInfo Execute(Guid sessionID, string webServiceUrl, JobInfo job);
    }
}
