using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Threading;

namespace WebService
{
    /// <summary>
    /// Summary description for WebService
    /// </summary>
    [WebService(Namespace = "http://localhost/webservices/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    public class WebService : System.Web.Services.WebService
    {
        MessageBus.MessageBusImpl busImpl = new MessageBus.MessageBusImpl();

        #region WebService interface
        [WebMethod]
        public void StartSession(Guid sessionID, int clientID)
        {
            busImpl.StartSession(sessionID, clientID);
        }

        [WebMethod]
        public void StopSession(Guid sessionID)
        {
            busImpl.StopSession(sessionID);
        }

        [WebMethod]
        public MessageBus.MessageBusImpl.GetActiveClientsResult GetActiveClients(Guid sessionID)
        {
            return busImpl.GetActiveClients(sessionID);
        }
        #endregion

    }

    //public delegate void ActiveClientsChangedDelegate(int[] clients);
}
