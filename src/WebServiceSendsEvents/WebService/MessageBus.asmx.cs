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
        #region private members
        // This static Dictionary keeps track of all currently open sessions
        private static Dictionary<Guid, ClientSessionData> s_services = new Dictionary<Guid, ClientSessionData>();

        private String m_clientID;
        #endregion private members

        public MessageBus()
        {
        }

        // I wish to have something like commented text below to be able setup event staff for WebService
        // (see also commented out ActiveClientsChangedDelegate definition at the end of this namespace)
        // but unfortunately it's not a case:
        //[WebEvent]
        //public event ActiveClientsChangedDelegate OnActiveClientsChanged = null;

        #region WebService interface
        [WebMethod]
        public void StartSession(Guid sessionID, String clientID)
        {
            lock (s_services)
            {
                if (s_services.ContainsKey(sessionID))
                {
                    // Session found in the list
                    m_clientID = s_services[sessionID].ClientID;
                    s_services[sessionID].GetActiveClientsDone = false;
                }
                else
                {
                    // Add session to the list
                    m_clientID = clientID;
                    s_services.Add(sessionID, new ClientSessionData(m_clientID));
                }
            }

            lock (s_services)
            {
                // Signal GetActiveClientsCompleted event for each client
                foreach (Guid sID in s_services.Keys)
                {
                    s_services[sID].GetMessageCompleted.Set();
                }
            }
        }

        public class Message
        {
            public String FromSessionID { get; set; }
            public String ToSessionID { get; set; }
            public String To{get;set;}
            public String Data { get; set; }
        }

        [WebMethod]
        public void SendMessage(
            Guid sessionID, 
            Message msg)
        {
            lock (s_services)
            {
                msg.FromSessionID = sessionID.ToString();

                Guid to = findToSessionID(msg);
                if (to == Guid.Empty)
                    throw new Exception(string.Format("{0} not found",
                    msg.To));

                Guid toSessionID = appendMessageToTargetSession(to, msg);
                
                //notify target session
                s_services[toSessionID].GetMessageCompleted.Set();
            }
        }

        Guid appendMessageToTargetSession(Guid to, Message msg)
        {
            msg.ToSessionID = to.ToString();

            s_services[to].Messages.Add(msg);
            return to;
        }
        
        
        Guid findToSessionID(Message msg)
        {
            Guid to = Guid.Empty;
            foreach (Guid sID in s_services.Keys)
            {
                if (s_services[sID].ClientID == msg.To)
                {
                    to = sID;
                    break;
                }
            }
            return to;
        }

        [WebMethod]
        public void StopSession(Guid sessionID)
        {
            lock (s_services)
            {
                if (s_services.ContainsKey(sessionID))
                {
                    StopGetMessage(sessionID);
                }
            }

            lock (s_services)
            {
                // Signal GetActiveClientsCompleted event for each client
                foreach (Guid sID in s_services.Keys)
                {
                    s_services[sID].GetMessageCompleted.Set();
                }
                // Remove session from the list
                s_services.Remove(sessionID);
            }
        }

        [WebMethod]
        public GetMessageResult GetMessage(Guid sessionID)
        {
            if (!s_services.ContainsKey(sessionID))
            {
                //string message = string.Format("No relevant sessions detected: {0}", sessionID.ToSTring());
                //System.Diagnostics.Debug.WriteLine(message);

                // Return empty client list
                return new GetMessageResult(new String[] { }, true);
            }

            //DateTime dt = DateTime.Now;
            bool signalled = s_services[sessionID].GetMessageCompleted.WaitOne();  // wait for GetActiveClientsCompleted event
            if (signalled)
            {
                lock (s_services)
                {
                    //string message = string.Format("GetActiveClientsCompleted event detected during {0} ms timeout", (DateTime.Now - dt).TotalMilliseconds);
                    //System.Diagnostics.Debug.WriteLine(message);

                    List<Message> msgs = new List<Message>();

                    // Create client list and return it
                    List<String> clients = new List<String>();
                    foreach (Guid sID in s_services.Keys)
                    {
                        if (sID == sessionID)
                        {
                            continue;
                        }
                        clients.Add(s_services[sID].ClientID);
                    }
                    //
                    bool exists = s_services.ContainsKey(sessionID);
                    GetMessageResult result = new GetMessageResult(clients.ToArray(),
                        exists ? s_services[sessionID].GetActiveClientsDone : true
                    );
                    result._Messages = s_services[sessionID].Messages;
                    //clear message
                    s_services[sessionID].Messages = new List<Message>();
                    return result;
                }
            }
            else
            {
                //string message = string.Format("No GetActiveClientsCompleted events detected during {0} ms timeout", timeoutMsec);
                //System.Diagnostics.Debug.WriteLine(message);

                bool exists = s_services.ContainsKey(sessionID);
                // Return empty client list
                return new GetMessageResult(new String[] { },
                    exists ? s_services[sessionID].GetActiveClientsDone : true);
            }
        }
        #endregion

        private void StopGetMessage(Guid sessionID)
        {
            s_services[sessionID].GetActiveClientsDone = true;
            //clear message
            s_services[sessionID].Messages = new List<Message>();

            s_services[sessionID].GetMessageCompleted.Set();
        }

        private class ClientSessionData
        {
            public String ClientID;
            public List<Message> Messages = new List<Message>();
            public AutoResetEvent GetMessageCompleted = new AutoResetEvent(false);
            public ClientSessionData(String clientID)
            {
                ClientID = clientID;
            }

            public bool GetActiveClientsDone = false;
        }

        public class GetMessageResult
        {
            public bool _Done = false;
            public String[] _ClientIDs = new String[] { };
            public List<Message> _Messages = new List<Message>();

            public GetMessageResult() { }
            public GetMessageResult(String[] clients, bool done)
            {
                _ClientIDs = clients;
                _Done = done;
            }
        }
    }

    //public delegate void ActiveClientsChangedDelegate(int[] clients);
}
