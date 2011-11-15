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
        private static Dictionary<Guid, ClientState> s_services = new Dictionary<Guid, ClientState>();

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
                    s_services.Add(sessionID, new ClientState(m_clientID));
                }
            }

            lock (s_services)
            {
                // Signal GetActiveClientsCompleted event for each client
                foreach (Guid sID in s_services.Keys)
                {
                    s_services[sID].GetActiveClientsCompleted.Set();
                }
            }
        }

        public class Message
        {
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
            }
        }

        void appendMessage(Message msg)
        {
            Guid to = findToSessionID(msg);
            if (to == Guid.Empty)
                throw new Exception(string.Format("{0} not found",
                msg.To));
            msg.ToSessionID = to.ToString();
            s_services[to].Messages.Add(msg);
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
                    StopGetActiveClients(sessionID);
                }
            }

            lock (s_services)
            {
                // Signal GetActiveClientsCompleted event for each client
                foreach (Guid sID in s_services.Keys)
                {
                    s_services[sID].GetActiveClientsCompleted.Set();
                }
                // Remove session from the list
                s_services.Remove(sessionID);
            }
        }

        [WebMethod]
        public GetActiveClientsResult GetActiveClients(Guid sessionID)
        {
            if (!s_services.ContainsKey(sessionID))
            {
                //string message = string.Format("No relevant sessions detected: {0}", sessionID.ToSTring());
                //System.Diagnostics.Debug.WriteLine(message);

                // Return empty client list
                return new GetActiveClientsResult(new String[] { }, true);
            }

            //DateTime dt = DateTime.Now;
            bool signalled = s_services[sessionID].GetActiveClientsCompleted.WaitOne();  // wait for GetActiveClientsCompleted event
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
                    GetActiveClientsResult result = new GetActiveClientsResult(clients.ToArray(),
                        exists ? s_services[sessionID].GetActiveClientsDone : true
                    );
                    result._Messages = s_services[sessionID].Messages;
                    return result;
                }
            }
            else
            {
                //string message = string.Format("No GetActiveClientsCompleted events detected during {0} ms timeout", timeoutMsec);
                //System.Diagnostics.Debug.WriteLine(message);

                bool exists = s_services.ContainsKey(sessionID);
                // Return empty client list
                return new GetActiveClientsResult(new String[] { },
                    exists ? s_services[sessionID].GetActiveClientsDone : true);
            }
        }
        #endregion

        private void StopGetActiveClients(Guid sessionID)
        {
            s_services[sessionID].GetActiveClientsDone = true;
            s_services[sessionID].GetActiveClientsCompleted.Set();
        }

        private class ClientState
        {
            public String ClientID;
            public List<Message> Messages = new List<Message>();
            public AutoResetEvent GetActiveClientsCompleted = new AutoResetEvent(false);
            public ClientState(String clientID)
            {
                ClientID = clientID;
            }

            public bool GetActiveClientsDone = false;
        }

        public class GetActiveClientsResult
        {
            public bool _Done = false;
            public String[] _Clients = new String[] { };
            public List<Message> _Messages = new List<Message>();

            public GetActiveClientsResult() { }
            public GetActiveClientsResult(String[] clients, bool done)
            {
                _Clients = clients;
                _Done = done;
            }
        }
    }

    //public delegate void ActiveClientsChangedDelegate(int[] clients);
}
