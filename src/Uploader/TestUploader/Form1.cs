using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

using WebserviceFileSystem;

namespace TestUploader
{
    /// <summary>
    /// A test form used to upload a file from a windows application using
    /// the Uploader Web Service
    /// </summary>
    public partial class Form1 : Form
    {

        WSFileSystem fileSystem_;

        public Form1()
        {
            InitializeComponent();
        }

        void onWSFileSystemEvent(object Sender, WSFileSystemEventArgs e)
        {
            lblFileName.Text = e.filePath_;
            lblProgross.Text = string.Format("{0} of {1}", e.part_, e.partCount_);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            lblFileName.Text = "";
            lblProgross.Text = "";

            fileSystem_ = new WSFileSystem();
            fileSystem_.WSFileSystemEvent += this.onWSFileSystemEvent;
            fileSystem_.Url = "http://localhost:21369/fileUploader.asmx";
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
                string msg = fileSystem_.UploadLargeFile(filename,
                    WSFileSystem.removeDriver(
                    WSFileSystem.getFolder(filename)
                    )
                    );
                MessageBox.Show("File upload:" + msg);
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
                string sTmp = fileSystem_.Remove(new string[]{filename});
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

        static string folderPath_ = @"temp\in";
        private void btnDownload_Click(object sender, EventArgs e)
        {
            DownloadFile( WSFileSystem.getFileName(txtFileName.Text));
        }

        private void DownloadFile(string filename)
        {
            try
            {
                string[] fileParts;
                // create an instance fo the web service
                string filePath = fileSystem_.DownloadLargeFile(filename, folderPath_, @"e:\temp\3-2.bmp", out fileParts);
                MessageBox.Show("File Download Status: " + filePath, "File Download ");
            }
            catch (Exception ex)
            {
                // display an error message to the user
                MessageBox.Show(ex.Message.ToString(), "Upload Error");
            }
        }

        private void btnSetWebUrl_Click(object sender, EventArgs e)
        {
            fileSystem_.Url = "http://upload.3wfocus.com/zhaowd/FileUploader.asmx";
        }

        private void btnList_Click(object sender, EventArgs e)
        {
            try
            {
                string[] files = fileSystem_.List(txtFileName.Text, folderPath_);
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
                string ret = fileSystem_.Move(txtFileName.Text, txtNewName.Text);
                MessageBox.Show("File Move Status: \n" + ret, "File Move ");
            }
            catch (Exception ex)
            {
                // display an error message to the user
                MessageBox.Show(ex.Message.ToString(), "Move Error");
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            fileSystem_.WSFileSystemEvent -= this.onWSFileSystemEvent;
            fileSystem_ = null;
        }

   }
}