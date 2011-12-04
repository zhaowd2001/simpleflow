using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
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
        private uploaderWS.FileUploader m_service = new cardocr_mobile6.uploaderWS.FileUploader();

        Updater.ConfigUtil config_ = new Updater.ConfigUtil();

        string webServiceUrl_ = null;
        //sdcc:
        //this.Url = "http://13.187.242.140/mb/FileUploader.asmx";

        //home:
        //string webServiceUrl_ = "http://192.168.72.130/mb/FileUploader.asmx";

        //sdcc localhost:
        //string webServiceUrl_ = "http://13.187.241.42:21369/mb/FileUploader.asmx";

        // Helper procedure
        private DialogResult ShowMessageBox(string msg)
        {
            return ShowMessageBox(msg, "", MessageBoxButtons.OK);
        }

        // Helper procedure
        private DialogResult ShowMessageBox(string msg, string caption)
        {
            return ShowMessageBox(msg, caption, MessageBoxButtons.OK);
        }

        // We'd like to keep the window smaller than full-screen
        // Unfortunately, after getting focus back from modal dialog (message box) it gets automatically resized 
        // by the shell. To prevent this we resize it again
        private DialogResult ShowMessageBox(string msg, string caption, MessageBoxButtons buttons)
        {

            DialogResult res = MessageBox.Show(msg, caption, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
            return res;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Exception ex;
            if( ! config_.init(out ex))
            {
                ShowMessageBox("Failed to read updater configuration: " + ex.ToString());
                this.Close();
                return;
            }
            webServiceUrl_ = config_.getServiceUrl();
            clientID_ = sessionID_.ToByteArray()[0].ToString();

            messageBus_ = new WSFileSystem(sessionID_,webServiceUrl_);

            lblInfo.Text = sessionID_.ToString() + " - " + clientID_;
            lblInfo.Text += "\n";
            lblInfo.Text += messageBus_.Url_;

            btnUpload.Enabled = false;

            btnStartSession.Enabled = true;
            btnSendMessage.Enabled = false;
            btnStopSession.Enabled = false;

            btnCamera.Focus();

            // Subscribe for event
            //m_service.GetMessageCompleted += new uploaderWS.GetMessageCompletedEventHandler(m_service_GetActiveClientsCompleted);
        }

        void m_service_GetActiveClientsCompleted(IAsyncResult ar)
        {
            uploaderWS.GetMessageResult r = m_service.EndGetMessage(ar);
            /*
            if (e.Error != null)
            {
                listBoxClients.Items.Add(e.Error.ToString());
                return;
            }
            */
            displayMessageFromServer(r);

            // This call reactivates GetActiveClients event listener only if we are not closing it
            if ( !r._Done)
                m_service.BeginGetMessage(sessionID_, new AsyncCallback(m_service_GetActiveClientsCompleted), new object());
        }

        public delegate void UpdateTextInListBox(string message);

        private void displayMessageFromServer(uploaderWS.GetMessageResult e)
        {
            // Add current list of active clients to list box
            String[] clients = e._ClientIDs;

            string msg1 = "Server:";
            string msgData = null;
            foreach (uploaderWS.Message msg in e._Messages)
            {
                if (msgData == null)
                    msgData = msg.Data;
                msg1 += msg.From;
                msg1 += " said to me:";
                msg1 += msg.Data;
                msg1 += ". ";
            }

            //
            msg1 = handleJobResult(msgData) + msg1;

            //
            lblInfo.BeginInvoke(new UpdateTextInListBox(lblInfo_setText), new object[] { msg1 });
        }

        static string decodeFilePathForJson(string path)
        {
            return path.Replace('/', '\\');
        }

        private static cardocr.JobInfo decodeJobInfo(string message)
        {
            if (message == null)
                return null;

            if (message.StartsWith("{") &&
                message.EndsWith("}"))
            {
                CodeBetter.Json.JsonReader r = new CodeBetter.Json.JsonReader(message);
                cardocr.JobInfo j = CodeBetter.Json.JsonDeserializer.Deserialize<cardocr.JobInfo>(r);
                //
                j.RemoteFilePath = decodeFilePathForJson(j.RemoteFilePath);
                j.ResultRemoteFilePath = decodeFilePathForJson(j.ResultRemoteFilePath);
                //
                return j;
            }

            return null;
        }

        string handleJobResult(string data)
        {
            string ret = "";
            cardocr.JobInfo j = decodeJobInfo(data);
            if (j != null)
            {
                if (j.Result == 0)//succss
                {
                    //download
                    string l = cameraFileName_ + ".txt";
                    download(j.ResultRemoteFilePath, l);
                    System.Diagnostics.Process.Start(l, null);
                    ret += "Job Success.";
                }
                else
                {
                    ret += string.Format("Job Error:{0:X}.", j.Result);
                }
            }
            return ret;
        }

        void download(string remote, string local)
        {
            string[] files = null;
            messageBus_.DownloadLargeFile(
                WSFileSystem.getFileName(remote),
                WSFileSystem.getFolder(remote),
                local,
                out files);
        }

        void lblInfo_setText(string t)
        {
            lblInfo.Text = t;
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            lblInfo.Text = "uploading";
            btnUpload.Enabled = false;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                //
                string ret = messageBus_.UploadLargeFile(
                    camera_const.getCameraFilePath(cameraFileName_), 
                    camera_const.getRemoteFolder());
                MessageBox.Show(string.Format("Upload ->{0}", ret));
                lblInfo.Text = ret;
                btnSendMessage.Focus();
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

            lblInfo.Text = "";

            m_service.Url = messageBus_.Url_;
            m_service.StartSession(sessionID_, clientID_);
            m_service.BeginGetMessage(sessionID_, new AsyncCallback(m_service_GetActiveClientsCompleted), m_service);
            m_service.Timeout = -1;
            
            btnStopSession.Enabled = true;
            btnSendMessage.Enabled = true;
            btnUpload.Enabled = true;

            Cursor.Current = Cursors.Default;
            btnUpload.Focus();
            lblInfo.Text = "started";
        }

        private void btnSendMessage_Click(object sender, EventArgs e)
        {
            lblInfo.Text = "sending";
            Cursor.Current = Cursors.WaitCursor;
            btnSendMessage.Enabled = false;
            try
            {
                uploaderWS.Message msg = new cardocr_mobile6.uploaderWS.Message();
                msg.Data = encodeJobInfo();
                msg.To = camera_const.getTargetAll();
                m_service.SendMessage(sessionID_, msg);

                lblInfo.Text = "sent ok";
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                btnSendMessage.Enabled = true;
                btnSendMessage.Focus();
            }
        }

        string encodeFilePathForJson(string path)
        {
            if (path == null)
                return path;
            return path.Replace('\\', '/');
        }

        private string encodeJobInfo()
        {
            cardocr.JobInfo job = new cardocr.JobInfo();
            job.RemoteFilePath = encodeFilePathForJson(camera_const.getRemoteFilePath(cameraFileName_));
            job.ResultRemoteFilePath = encodeFilePathForJson(job.ResultRemoteFilePath);
            job.AppID = camera_const.getCardOcr_AppID();
            job.Version = camera_const.getCardOcr_Version();

            //
            return toString(job);
        }


        private static string toString(cardocr.JobInfo job)
        {
            job.setNullFieldToEmpty();
            //
            //
            StringBuilder sb = new StringBuilder();
            CodeBetter.Json.JsonWriter w = new CodeBetter.Json.JsonWriter(sb);
            CodeBetter.Json.JsonSerializer.Serialize(w, job);
            return sb.ToString();
        }

        private void btnStopSession_Click(object sender, EventArgs e)
        {
            lblInfo.Text = "stopping";
            btnStopSession.Enabled = false;
            btnSendMessage.Enabled = false;
            btnUpload.Enabled = false;
            btnStartSession.Enabled = true;
            Cursor.Current = Cursors.WaitCursor;

            m_service.StopSession(sessionID_);
            
            Cursor.Current = Cursors.Default;
            btnStartSession.Focus();
            lblInfo.Text = "stopped";
        }

        private void btnQuit_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            this.Close();
            Application.Exit();
            Cursor.Current = Cursors.Default;
        }

        string cameraFileName_;

        void checkFolderExists(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        private void btnCamera_Click(object sender, EventArgs e)
        {
            //
            checkFolderExists(camera_const.getCameraWorkFolder());
            //
            cameraFileName_ = Guid.NewGuid().ToString()+".jpg";
            lblInfo.Text = "cameraing";
            CameraCaptureDialog cameraCapture = new CameraCaptureDialog();

            cameraCapture.Owner = null;
            cameraCapture.InitialDirectory = camera_const.getCameraWorkFolder();
            cameraCapture.DefaultFileName = cameraFileName_;
            cameraCapture.Title = "Camera Demo";
            cameraCapture.VideoTypes = CameraCaptureVideoTypes.Standard;
            cameraCapture.Resolution = new Size(176, 144);
            cameraCapture.VideoTimeLimit = new TimeSpan(0, 0, 15);  // Limited to 15 seconds of video.
            cameraCapture.Mode = CameraCaptureMode.Still;

            if (DialogResult.OK == cameraCapture.ShowDialog())
            {
                MessageBox.Show(string.Format("The picture or video has been successfully captured to:\n{0}", cameraCapture.FileName));
                btnStartSession.Focus();
            }
            lblInfo.Text = "ok";
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            Updater.frmUpdate f = new Updater.frmUpdate();
            f.WebServiceUrl_ = webServiceUrl_;
            f.ShowDialog();
        }

    }
}