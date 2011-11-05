using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FileSystem;

namespace WebserviceFileSystem
{
    public class WSFileSystem : IFileSystem
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
        public string UploadFile(string filename)
        {
            // get the exact file name from the path
            String strFile = System.IO.Path.GetFileName(filename);

            // create an instance fo the web service
            WebserviceFileSystem.Uploader.FileUploader srv = newUploader();

            WebserviceFileSystem.BigFile file = new WebserviceFileSystem.BigFile(filename);
            string msg = "";
            for (int i = 0; i < file.getFilePartCount(); i++)
            {
                byte[] data = file.readFilePart(i);
                string sTmp = srv.UploadFile(data, removeDriver(filename) + "-part" +
                    i.ToString().PadLeft(8, '0'));

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
            return msg;
        }

        public string Remove(string filename)
        {
            // create an instance fo the web service
            WebserviceFileSystem.Uploader.FileUploader srv = newUploader();

            return srv.Remove(filename);
        }

        public WebserviceFileSystem.Uploader.FileItem[] DownloadFile(string filename, string directory)
        {
                // create an instance fo the web service
            WebserviceFileSystem.Uploader.FileUploader srv = newUploader();

            return srv.DownloadFile(filename, directory);
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
    }

}
