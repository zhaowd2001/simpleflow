using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace cardocr_mobile6
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Guid sessionID_ = Guid.NewGuid();
        String clientID_;
        uploaderWS.FileUploader messageBus_ = new cardocr_mobile6.uploaderWS.FileUploader();

        private void Form1_Load(object sender, EventArgs e)
        {
            clientID_ = sessionID_.ToByteArray()[0].ToString();
            lblInfo.Text = sessionID_.ToString() + " - " + clientID_;
            lblInfo.Text += "\n";
            lblInfo.Text += messageBus_.Url;

            btnUpload.Enabled = false;

            btnStartSession.Enabled = true;
            btnSendMessage.Enabled = false;
            btnStopSession.Enabled = false;

            btnStartSession.Focus();

        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            btnUpload.Enabled = false;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                btnUpload.Enabled = true;
            }
        }

        private void btnStartSession_Click(object sender, EventArgs e)
        {
            btnStartSession.Enabled = false;
            Cursor.Current = Cursors.WaitCursor;
            
            messageBus_.StartSession(sessionID_, clientID_);
            
            btnStopSession.Enabled = true;
            btnSendMessage.Enabled = true;

            Cursor.Current = Cursors.Default;
            btnSendMessage.Focus();
        }

        private void btnSendMessage_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            btnSendMessage.Enabled = false;
            try
            {
                uploaderWS.Message msg = new cardocr_mobile6.uploaderWS.Message();
                msg.Data = DateTime.Now.ToLongTimeString();
                msg.To = "all";
                messageBus_.SendMessage(sessionID_, msg);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                btnSendMessage.Enabled = true;
            }
        }

        private void btnStopSession_Click(object sender, EventArgs e)
        {
            btnStopSession.Enabled = false;
            btnSendMessage.Enabled = false;
            btnStartSession.Enabled = true;
            Cursor.Current = Cursors.WaitCursor;
            
            messageBus_.StopSession(sessionID_);
            
            Cursor.Current = Cursors.Default;
            btnStartSession.Focus();
        }

        private void btnQuit_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            this.Close();
            Application.Exit();
            Cursor.Current = Cursors.Default;
        }

    }
}