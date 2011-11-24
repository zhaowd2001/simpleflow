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

        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
        }

        private void btnStartSession_Click(object sender, EventArgs e)
        {
            messageBus_.StartSession(sessionID_, clientID_);
        }

        private void btnSendMessage_Click(object sender, EventArgs e)
        {
            uploaderWS.Message msg = new cardocr_mobile6.uploaderWS.Message();
            msg.Data = DateTime.Now.ToLongTimeString();
            msg.To = "all";
            messageBus_.SendMessage(sessionID_, msg);
        }

        private void btnStopSession_Click(object sender, EventArgs e)
        {
            messageBus_.StopSession(sessionID_);
        }

    }
}