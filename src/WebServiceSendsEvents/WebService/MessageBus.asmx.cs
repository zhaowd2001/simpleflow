using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Threading;

namespace MessageBus
{
    /// <summary>
    /// Summary description for WebService
    /// </summary>
    [WebService(Namespace = "http://localhost/MessageBus/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    public class MessageBus : System.Web.Services.WebService
    {
        MessageBusImpl busImpl_ = new MessageBusImpl();
        // I wish to have something like commented text below to be able setup event staff for WebService
        // (see also commented out ActiveClientsChangedDelegate definition at the end of this namespace)
        // but unfortunately it's not a case:
        //[WebEvent]
        //public event ActiveClientsChangedDelegate OnActiveClientsChanged = null;

        #region WebService interface
        [WebMethod]
        public void StartSession(Guid sessionID, String clientID)
        {
            busImpl_.StartSession(sessionID, clientID);
        }

        [WebMethod]
        public void SendMessage(
            Guid sessionID, 
            MessageBusImpl.Message msg)
        {
            busImpl_.SendMessage(sessionID, msg);
        }

        [WebMethod]
        public void StopSession(Guid sessionID)
        {
            busImpl_.StopSession(sessionID);
        }

        [WebMethod]
        public MessageBusImpl.GetMessageResult GetMessage(Guid sessionID)
        {
            return busImpl_.GetMessage(sessionID);
        }
        #endregion
    }

    //public delegate void ActiveClientsChangedDelegate(int[] clients);
}
