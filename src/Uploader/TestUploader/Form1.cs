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


        private void Form1_Load(object sender, EventArgs e)
        {
            fileSystem_ = new WSFileSystem();
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
                string msg = fileSystem_.UploadFile(filename);
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
                string sTmp = fileSystem_.Remove(filename);
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
                WebserviceFileSystem.Uploader.FileContent[] files = fileSystem_.DownloadFile(filename, "testdata");
                MessageBox.Show("File Download Status: " + files[0].path_, "File Download ");
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
                string[] files = fileSystem_.List( txtFileName.Text, "testdata");
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
   }
}