using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using FileSystem;
using LocalFileSystem;

namespace WebserviceFileSystem
{
    public class WSFileSystem 
    {
        public WSFileSystem()
        {
            Url = new WebserviceFileSystem.Uploader.FileUploader().Url;
        }

        /// <summary>
        /// Upload any file to the web service; this function may be
        /// used in any application where it is necessary to upload
        /// a file through a web service
        /// </summary>
        /// <param name="filename">Pass the file path to upload</param>
        public string UploadLargeFile(string filename)
        {
            // get the exact file name from the path
            String strFile = System.IO.Path.GetFileName(filename);

            // create an instance fo the web service
            WebserviceFileSystem.Uploader.FileUploader srv = newUploader();

            LargeLocalFileReader file = new LargeLocalFileReader(filename);
            string msg = "";
            List<String> fileParts = new List<string>();
            for (int i = 0; i < file.getFilePartCount(); i++)
            {
                byte[] data = file.readFilePart(i);
                string sTmp = srv.UploadFile(data, removeDriver(filename) + "-part" +
                    i.ToString().PadLeft(8, '0'));

                fileParts.Add(sTmp);
                msg += "\n" + sTmp;
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
            //write large info file
            {
                byte[] data = LargeLocalFileInfo.buildContent(fileParts.ToArray());
                string largeFileName = removeDriver(filename) + LargeLocalFileInfo.FILE_NAME_EXT;
                string sTmp = srv.UploadFile(data, largeFileName);
                msg += "\n" + sTmp;
            }
            return msg;
        }

        public string Remove(string filename)
        {
            // create an instance fo the web service
            WebserviceFileSystem.Uploader.FileUploader srv = newUploader();

            return srv.Remove(filename);
        }

        private WebserviceFileSystem.Uploader.FileContent[] DownloadFile(string filename, string directory)
        {
            // create an instance fo the web service
            WebserviceFileSystem.Uploader.FileUploader srv = newUploader();

            return srv.DownloadFile(filename, directory);
        }

        public string  DownloadLargeFile(string remoteFileName, string remoteDirectory, string destFilePath)
        {
            string descriptionFileName = remoteFileName + LargeLocalFileInfo.FILE_NAME_EXT;

            // create an instance fo the web service
            WebserviceFileSystem.Uploader.FileUploader srv = newUploader();

            WebserviceFileSystem.Uploader.FileContent[] descriptionFileContent =
                srv.DownloadFile(descriptionFileName, remoteDirectory);

            string[] fileParts = content2string(descriptionFileContent[0].content_);
            foreach (string filePart in fileParts)
            {
                WebserviceFileSystem.Uploader.FileContent[] partData = srv.DownloadFile(
                    getFileName(filePart),
                    getFolder(filePart));
                LocalFileSystem.LocalFileSystemUtil.writeFile(partData[0].content_, destFilePath, FileMode.Append);
            }
            //
            //
            string f = descriptionFileContent[0].path_;
            f = f.Substring(0, f.Length - LargeLocalFileInfo.FILE_NAME_EXT.Length);
            return f;
        }

        static string[] content2string(byte[] d)
        {
            return UTF8Encoding.UTF8.GetString(d).Split(new char[] { '\n' });
        }

        public string[] List(string filename, string directory)
        {
            // create an instance fo the web service
            WebserviceFileSystem.Uploader.FileUploader srv = newUploader();
            string[] files = srv.List(filename, directory);
            return files;
        }

        public string Move(string oldfilename, string newfilename)
        {
            // create an instance fo the web service
            WebserviceFileSystem.Uploader.FileUploader srv = newUploader();
            return srv.Move(oldfilename, newfilename);
        }
        /// <summary>
        /// 
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Uploader.FileUploader newUploader()
        {
            Uploader.FileUploader ret = new WebserviceFileSystem.Uploader.FileUploader();
            ret.Url = Url;
            return ret;
        }


        private static string removeDriver(string filename)
        {
            if (filename.Length >= 3)
            {
                if ((filename[1] == ':') &&
                    (filename[2] == '\\'))
                    return filename.Substring(3);
            }

            return filename;
        }

        static public string getFileName(string filePath)
        {
            string[] arr = filePath.Split(new char[] { '\\' });
            return arr[arr.Length - 1];
        }

        static public string getFolder(string filePath)
        {
            string[] arr = filePath.Split(new char[] { '\\' });
            string r = "";
            for(int i=0;i<arr.Length-1;i++){
                r += arr[i];
                r += "\\";
            }
            if (r.Length > 0)
            {
                if (r[r.Length - 1] == '\\')
                {
                    r = r.Substring(0, r.Length - 1);
                }
            }
            return r;
        }
    }
}
