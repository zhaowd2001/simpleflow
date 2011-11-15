
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WebServiceWindowsClient
{
    public partial class MessageBusForm : Form
    {
        #region local members
        private WebServiceWindowsClient.localhost.MessageBus m_service;
        private Guid m_sessionID;
        private String m_clientID;
        #endregion local members

        public MessageBusForm()
        {
            InitializeComponent();

            // Create proxy for WebService
            m_service = new WebServiceWindowsClient.localhost.MessageBus();

            // Subscribe for event
            m_service.GetMessageCompleted += new WebServiceWindowsClient.localhost.GetMessageCompletedEventHandler(m_service_GetActiveClientsCompleted);

            buttonStartSession.Enabled = true;
            buttonStopSession.Enabled = false;

            // Just for test purposes set m_sessionID and m_clientID to something random
            m_sessionID = Guid.NewGuid();
            m_clientID = Guid.NewGuid().ToByteArray()[0].ToString();
        }

        void m_service_GetActiveClientsCompleted(object sender, 
            WebServiceWindowsClient.localhost.GetMessageCompletedEventArgs e)
        {
            // Add current list of active clients to list box
            String[] clients = e.Result._ClientIDs;

            string msg1 = "";
            foreach (localhost.Message msg in e.Result._Messages)
            {
                msg1 += msg.From;
                msg1 += " says:";
                msg1 += msg.Data;
                msg1 += ". ";
            }

            string client_list = " clients:";
            foreach (String client in clients)
            {
                client_list += client + " ";
            }
            listBoxEvents.Items.Add(string.Format("GetActiveClients completed with result: {0}, {1}", client_list, msg1));

            // This call reactivates GetActiveClients event listener only if we are not closing it
            if (!e.Result._Done)
                m_service.GetMessageAsync(m_sessionID, new object());
        }

        private void buttonStartSession_Click(object sender, EventArgs e)
        {
            buttonStartSession.Enabled = false;
            buttonStopSession.Enabled = true;

            // Start session
            m_service.StartSession(m_sessionID, m_clientID);

            // Update listbox
            listBoxEvents.Items.Add(string.Format("New session for client {0} started", m_clientID));

            // This call activates GetActiveClients event listener
            m_service.GetMessageAsync(m_sessionID);
        }

        private void WebServiceClientForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Stop session
            m_service.StopSession(m_sessionID);
        }

        private void buttonStopSession_Click(object sender, EventArgs e)
        {
            buttonStartSession.Enabled = true;
            buttonStopSession.Enabled = false;

            // Stop session
            m_service.StopSession(m_sessionID);

            // Update listbox
            listBoxEvents.Items.Add(string.Format("Session for client {0} stopped", m_clientID));
        }

        private void MessageBusForm_Load(object sender, EventArgs e)
        {

        }

        private void btnSend_Click(object sender, EventArgs e)
        {

            //send message
            localhost.Message msg = new localhost.Message();
            msg.Data = System.DateTime.Now.ToShortTimeString();
            msg.To = txtTo.Text.Trim();
            m_service.SendMessage(m_sessionID, msg);
        }
    }
}