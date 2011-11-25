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
        private uploaderWS.FileUploader m_service;
        private Guid m_sessionID;
        private String m_clientID;
        #endregion local members

        public MessageBusForm()
        {
            InitializeComponent();

            // Create proxy for WebService
            m_service = new uploaderWS.FileUploader();

            // Subscribe for event
            m_service.GetMessageCompleted += new uploaderWS.GetMessageCompletedEventHandler(m_service_GetActiveClientsCompleted);

            buttonStartSession.Enabled = true;
            buttonStopSession.Enabled = false;
            btnSend.Enabled = false;

            // Just for test purposes set m_sessionID and m_clientID to something random
            m_sessionID = Guid.NewGuid();
            m_clientID = System.Net.Dns.GetHostName()+"-"+m_sessionID.ToByteArray()[0].ToString();
        }

        void m_service_GetActiveClientsCompleted(object sender,
            uploaderWS.GetMessageCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                listBoxClients.Items.Add(e.Error.ToString());
                return;
            }
            // Add current list of active clients to list box
            String[] clients = e.Result._ClientIDs;

            string msg1 = "";
            string message = null;
            string fromSession = null;
            foreach (uploaderWS.Message msg in e.Result._Messages)
            {
                msg1 += msg.From;
                msg1 += " said to me:";
                msg1 += msg.Data;
                msg1 += ". ";

                if (fromSession == null)
                    fromSession = msg.From;
                if (message == null)
                    message = msg.Data;
            }

            string r = app_handle_message(fromSession, message);
            if (r != null)
                msg1 += ", handler->" + r;

            listBoxClients.Items.Clear();
            listBoxClients.Items.Add(m_clientID);
            listBoxClients.Items.AddRange(clients);
            listBoxEvents.Items.Add(string.Format("{0}", msg1));

            //
            //
            // This call reactivates GetActiveClients event listener only if we are not closing it
            if (!e.Result._Done)
                m_service.GetMessageAsync(m_sessionID, new object());
        }

        private void buttonStartSession_Click(object sender, EventArgs e)
        {
            buttonStartSession.Enabled = false;
            buttonStopSession.Enabled = true;
            btnSend.Enabled = true;

            m_service.Url = combServer.Text.Trim();
            try
            {
                // Start session
                m_service.StartSession(m_sessionID, m_clientID);

                // Update listbox
                listBoxEvents.Items.Add(string.Format("New session for client {0} started", m_clientID));

                // This call activates GetActiveClients event listener
                m_service.GetMessageAsync(m_sessionID);
            }
            catch (Exception e1)
            {
                listBoxEvents.Items.Add(e1.ToString());
            }

            txtMessage.Focus();
        }

        private void WebServiceClientForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                // Stop session
                m_service.StopSession(m_sessionID);
            }
            catch (Exception)
            {
                //...
            }
        }

        private void buttonStopSession_Click(object sender, EventArgs e)
        {
            buttonStartSession.Enabled = true;
            buttonStopSession.Enabled = false;

            try
            {
                // Stop session
                m_service.StopSession(m_sessionID);

                // Update listbox
                listBoxEvents.Items.Add(string.Format("Session for client {0} stopped", m_clientID));
            }
            catch (Exception e1)
            {
                listBoxEvents.Items.Add(e1.ToString());
            }
            txtMessage.Focus();
        }

        static string getTargetAll()
        {
            return "<all>";
        }
        private void MessageBusForm_Load(object sender, EventArgs e)
        {
            app_init();
            form_init();
        }

        public static void app_init()
        {
            cardocr.MessageHandlerFactory.init();
        }

        public string app_handle_message(string fromSession, string message)
        {
            cardocr.JobInfo job = null;
            cardocr.IMessageHandler h = cardocr.MessageHandlerFactory.findHandler(message, out job);
            if (h == null || job==null)
                return null;

            job.SessionFrom = fromSession;
            string ret = h.Execute(job.RemoteFilePath);
            //
            sendJobResult(job, ret);
            return ret;
        }

        void sendJobResult(cardocr.JobInfo job, string result)
        {
            //send message
            uploaderWS.Message msg = new uploaderWS.Message();
            msg.Data = result;
            msg.To = job.SessionFrom;
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

        private void form_init()
        {
            listBoxClients.Items.Add(m_clientID);
            txtTo.Text = getTargetAll();// m_clientID;

            combServer.Items.Add(string.Format("http://{0}/mb/FileUploader.asmx", "chnxsc808w2k3sp2"));
            combServer.Items.Add("http://localhost:21369/FileUploader.asmx");
            combServer.Items.Add("http://upload.3wfocus.com/zhaowd/FileUploader.asmx");

            //m_service.Url;
            combServer.SelectedIndex = 0;

            txtMessage.Focus();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            //send message
            uploaderWS.Message msg = new uploaderWS.Message();
            msg.Data = System.DateTime.Now.ToLongTimeString() + " "+txtMessage.Text.Trim();
            msg.To = txtTo.Text.Trim();

            if (msg.To.Length == 0)
            {
                listBoxEvents.Items.Add("Please input 'To'");
                return;
            }

            listBoxEvents.Items.Add(string.Format(" I said to {0}:{1}", msg.To, msg.Data));
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

        private void lblTo_Click(object sender, EventArgs e)
        {

        }

    }
}
