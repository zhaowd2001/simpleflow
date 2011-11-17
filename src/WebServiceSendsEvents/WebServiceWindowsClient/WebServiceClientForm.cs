
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
            btnSend.Enabled = false;

            // Just for test purposes set m_sessionID and m_clientID to something random
            m_sessionID = Guid.NewGuid();
            m_clientID = System.Net.Dns.GetHostName()+"-"+m_sessionID.ToByteArray()[0].ToString();
        }

        void m_service_GetActiveClientsCompleted(object sender, 
            WebServiceWindowsClient.localhost.GetMessageCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                listBoxClients.Items.Add(e.Error.ToString());
                return;
            }
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

            listBoxClients.Items.Clear();
            listBoxClients.Items.Add(m_clientID);
            listBoxClients.Items.AddRange(clients);
            listBoxEvents.Items.Add(string.Format("{0}", msg1));

            // This call reactivates GetActiveClients event listener only if we are not closing it
            if (!e.Result._Done)
                m_service.GetMessageAsync(m_sessionID, new object());
        }

        private void buttonStartSession_Click(object sender, EventArgs e)
        {
            buttonStartSession.Enabled = false;
            buttonStopSession.Enabled = true;
            btnSend.Enabled = true;

            // Start session
            m_service.StartSession(m_sessionID, m_clientID);

            // Update listbox
            listBoxEvents.Items.Add(string.Format("New session for client {0} started", m_clientID));

            // This call activates GetActiveClients event listener
            m_service.GetMessageAsync(m_sessionID);
            txtMessage.Focus();
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

            txtMessage.Focus();
        }

        private void MessageBusForm_Load(object sender, EventArgs e)
        {
            listBoxClients.Items.Add(m_clientID);
            txtTo.Text = m_clientID;
            txtMessage.Focus();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {

            //send message
            localhost.Message msg = new localhost.Message();
            msg.Data = System.DateTime.Now.ToLongTimeString() + " "+txtMessage.Text.Trim();
            msg.To = txtTo.Text.Trim();

            if (msg.To.Length == 0)
            {
                listBoxEvents.Items.Add("Please input 'To'");
                return;
            }

            try
            {
                m_service.SendMessage(m_sessionID, msg);
            }
            catch (Exception e1)
            {
                listBoxEvents.Items.Add(e1.ToString());
            }
            txtMessage.Focus();
        }

        private void txtMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                btnSend.PerformClick();
            }
        }

        private void listBoxClients_DoubleClick(object sender, EventArgs e)
        {
            if (listBoxClients.SelectedIndex >= 0)
            {
                txtTo.Text = listBoxClients.Items[listBoxClients.SelectedIndex].ToString();
            }
            txtMessage.Focus();
        }

        private void txtMessage_TextChanged(object sender, EventArgs e)
        {

        }

    }
}
