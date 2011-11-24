using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.WindowsMobile.Forms;

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

        WSFileSystem messageBus_;

        private void Form1_Load(object sender, EventArgs e)
        {
            clientID_ = sessionID_.ToByteArray()[0].ToString();
            messageBus_ = new WSFileSystem(sessionID_);

            lblInfo.Text = sessionID_.ToString() + " - " + clientID_;
            lblInfo.Text += "\n";
            lblInfo.Text += messageBus_.Url_;

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
                //
                string ret = messageBus_.UploadLargeFile(getCameraFilePath(), "temp");
                MessageBox.Show(string.Format("Upload ->{0}", ret));
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
            
            messageBus_.newUploader().StartSession(sessionID_, clientID_);
            
            btnStopSession.Enabled = true;
            btnSendMessage.Enabled = true;
            btnUpload.Enabled = true;

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
                messageBus_.newUploader().SendMessage(sessionID_, msg);
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
            btnUpload.Enabled = false;
            btnStartSession.Enabled = true;
            Cursor.Current = Cursors.WaitCursor;

            messageBus_.newUploader().StopSession(sessionID_);
            
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

        public static string CAMERA_FOLDER = @"\My Documents";
        public static string CAMERA_FILENAME = @"test.jpg";

        public static string getCameraFilePath()
        {
            return CAMERA_FOLDER + @"\" + CAMERA_FILENAME;
        }

        private void btnCamera_Click(object sender, EventArgs e)
        {
            CameraCaptureDialog cameraCapture = new CameraCaptureDialog();

            cameraCapture.Owner = null;
            cameraCapture.InitialDirectory = CAMERA_FOLDER;
            cameraCapture.DefaultFileName = CAMERA_FILENAME;
            cameraCapture.Title = "Camera Demo";
            cameraCapture.VideoTypes = CameraCaptureVideoTypes.Standard;
            cameraCapture.Resolution = new Size(176, 144);
            cameraCapture.VideoTimeLimit = new TimeSpan(0, 0, 15);  // Limited to 15 seconds of video.
            cameraCapture.Mode = CameraCaptureMode.Still;

            if (DialogResult.OK == cameraCapture.ShowDialog())
            {
                MessageBox.Show(string.Format("The picture or video has been successfully captured to:\n{0}", cameraCapture.FileName));
            }
        }

    }
}