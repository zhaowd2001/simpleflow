using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;


namespace TestUploader
{
    /// <summary>
    /// A test form used to upload a file from a windows application using
    /// the Uploader Web Service
    /// </summary>
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            url_ = new TestUploader.Uploader.FileUploader().Url;
        }


        /// <summary>
        /// Upload any file to the web service; this function may be
        /// used in any application where it is necessary to upload
        /// a file through a web service
        /// </summary>
        /// <param name="filename">Pass the file path to upload</param>
        private void UploadFile(string filename)
        {
            try
            {
                // get the exact file name from the path
                String strFile = System.IO.Path.GetFileName(filename);

                // create an instance fo the web service
                TestUploader.Uploader.FileUploader srv = newUploader();

                WebserviceFileSystem.BigFile file = new WebserviceFileSystem.BigFile(filename);

                for (int i = 0; i < file.getFilePartCount(); i++)
                {
                    byte[] data = file.readFilePart(i);
                    string sTmp = srv.UploadFile(data, removeDriver(filename)+"-part"+
                        i.ToString().PadLeft(8,'0'));

                    // this will always say OK unless an error occurs,
                    // if an error occurs, the service returns the error message
#if false
                    if (
                    MessageBox.Show("File Upload Status: Part %" + i.ToString() + " of "+
                    file.getFilePartCount().ToString()+
                    ":" + sTmp, "File Upload", 
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button1
                    ) == System.Windows.Forms.DialogResult.Cancel)
                        break;
#endif
                }
            }
            catch (Exception ex)
            {
                // display an error message to the user
                MessageBox.Show(ex.Message.ToString()+"\n"+ex.ToString(), "Upload Error");
            }
        }

        private void Remove(string filename)
        {
            try
            {
                // create an instance fo the web service
                TestUploader.Uploader.FileUploader srv = newUploader();

                string sTmp = srv.Remove(filename);
                MessageBox.Show("Remove Status: " + sTmp, "Remove");
            }
            catch (Exception ex)
            {
                // display an error message to the user
                MessageBox.Show(ex.Message.ToString(), "Upload Error");
            }
        }


        /// <summary>
        /// Allow the user to browse for a file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Open File";
            openFileDialog1.Filter = "All Files|*.*";
            openFileDialog1.FileName = "";

            try
            {
                openFileDialog1.InitialDirectory = "C:\\Temp";
            }
            catch
            {
                // skip it 
            }

            openFileDialog1.ShowDialog();

            if (openFileDialog1.FileName == "")
                return;
            else
                txtFileName.Text = openFileDialog1.FileName;

        }



        /// <summary>
        /// If the user has selected a file, send it to the upload method, 
        /// the upload method will convert the file to a byte array and
        /// send it through the web service
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            if (txtFileName.Text != string.Empty)
                UploadFile(txtFileName.Text);
            else
                MessageBox.Show("You must select a file first.", "No File Selected");
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            Remove(txtFileName.Text);
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            DownloadFile(txtFileName.Text);
        }

        private void DownloadFile(string filename)
        {
            try
            {
                // create an instance fo the web service
                TestUploader.Uploader.FileUploader srv = newUploader();

                TestUploader.Uploader.FileItem [] files = srv.DownloadFile(filename, "testdata");
                MessageBox.Show("File Download Status: " + files[0].path, "File Download ");
            }
            catch (Exception ex)
            {
                // display an error message to the user
                MessageBox.Show(ex.Message.ToString(), "Upload Error");
            }
        }

        private  Uploader.FileUploader newUploader()
        {
            Uploader.FileUploader ret = new TestUploader.Uploader.FileUploader();
            ret.Url = url_;
            return ret;
        }

        private void btnSetWebUrl_Click(object sender, EventArgs e)
        {
            url_ = "http://upload.3wfocus.com/zhaowd/FileUploader.asmx";
        }

        string url_;

        private void btnList_Click(object sender, EventArgs e)
        {
            try
            {
                // create an instance fo the web service
                TestUploader.Uploader.FileUploader srv = newUploader();
                string[] files = srv.List( txtFileName.Text, "testdata");
                string msg = string.Join("\n", files);
                MessageBox.Show("File List Status: \n" + msg, "File List ");
            }
            catch (Exception ex)
            {
                // display an error message to the user
                MessageBox.Show(ex.Message.ToString(), "List Error");
            }
        }

        private void btnMove_Click(object sender, EventArgs e)
        {
            try
            {
                // create an instance fo the web service
                TestUploader.Uploader.FileUploader srv = newUploader();
                string ret = srv.Move(txtFileName.Text, txtNewName.Text);
                MessageBox.Show("File Move Status: \n" + ret, "File Move ");
            }
            catch (Exception ex)
            {
                // display an error message to the user
                MessageBox.Show(ex.Message.ToString(), "Move Error");
            }
        }

        private static string removeDriver(string filename)
        {
            if (filename.Length >= 3)
            {
                if((filename[1] == ':') &&
                    (filename[2] == '\\'))
                    return filename.Substring(3);
            }

            return filename;
        }
    }
}