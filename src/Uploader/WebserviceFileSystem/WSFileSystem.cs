using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace cardocr_mobile6
{
    public class WSFileSystem 
    {
        public delegate void WSFileSystemEventHandler(object sender, WSFileSystemEventArgs e);
        public event WSFileSystemEventHandler WSFileSystemEvent;

        Guid sessionID_;
        public WSFileSystem(Guid sessionID)
        {
            sessionID_ = sessionID;
            Url_ = new uploaderWS.FileUploader().Url;
        }

        /// <summary>
        /// Upload any file to the web service; this function may be
        /// used in any application where it is necessary to upload
        /// a file through a web service
        /// </summary>
        /// <param name="localFilePath">Pass the file path to upload</param>
        public string UploadLargeFile(string localFilePath, string remoteFolder)
        {
            // get the exact file name from the path
            String strFile = System.IO.Path.GetFileName(localFilePath);

            // create an instance fo the web service
            uploaderWS.FileUploader srv = newUploader();

            LocalFileSystem.LargeLocalFileReader file = new 
                LocalFileSystem.LargeLocalFileReader(localFilePath, 4*1024);//4K

            WSFileSystemEventArgs e = new WSFileSystemEventArgs();
            e.filePath_ = localFilePath;
            e.partCount_ = file.getFilePartCount();
            string msg = "";
            List<String> fileParts = new List<string>();
            for (int i = 0; i < file.getFilePartCount(); i++)
            {
                byte[] data = file.readFilePart(i);
                string fileName = getFileName(localFilePath)+"-part" +
                    i.ToString().PadLeft(8, '0');
                string sTmp = srv.UploadFile(sessionID_, data, remoteFolder + "\\" + fileName);

                e.part_++;
                RaiseWSFileSystemEvent(e);

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
                byte[] data = LocalFileSystem.LargeLocalFileInfo.buildContent(fileParts.ToArray());
                string largeFileName = remoteFolder + "\\" + getFileName(localFilePath) + LocalFileSystem.LargeLocalFileInfo.FILE_NAME_EXT;
                string sTmp = srv.UploadFile(sessionID_, data, largeFileName);
                msg += "\n" + sTmp;
            }
            return msg;
        }

        public string Remove(string[] filenames)
        {
            // create an instance fo the web service
            uploaderWS.FileUploader srv = newUploader();
            string ret = "";
            foreach (string f in filenames)
            {
                ret += srv.Remove(sessionID_, f);
                ret += "\n";
            }

            return ret;
        }

        private uploaderWS.FileContent[] DownloadFile(string filename, string directory)
        {
            // create an instance fo the web service
            uploaderWS.FileUploader srv = newUploader();

            return srv.DownloadFile(sessionID_, filename, directory);
        }

        public string  DownloadLargeFile(
            string remoteFileName, 
            string remoteDirectory, 
            string destFilePath,
            out string[] arrFilePart)
        {
            List<String> listFilePart = new List<string>();
            string descriptionFileName = remoteFileName + LocalFileSystem.LargeLocalFileInfo.FILE_NAME_EXT;

            // create an instance fo the web service
            uploaderWS.FileUploader srv = newUploader();

            uploaderWS.FileContent[] descriptionFileContent =
                srv.DownloadFile(sessionID_, descriptionFileName, remoteDirectory);
            listFilePart.Add(remoteDirectory + "\\" + descriptionFileName);

            string fileNameOnServer = descriptionFileContent[0].path_;
            fileNameOnServer = fileNameOnServer.Substring(0, fileNameOnServer.Length - LocalFileSystem.LargeLocalFileInfo.FILE_NAME_EXT.Length);
            
            WSFileSystemEventArgs e = new WSFileSystemEventArgs();
            e.filePath_ = fileNameOnServer;

            string[] fileParts = content2string(descriptionFileContent[0].content_);
            listFilePart.AddRange(fileParts);
            e.partCount_ = fileParts.Length;

            foreach (string filePart in fileParts)
            {
                uploaderWS.FileContent[] partData = srv.DownloadFile(
                    sessionID_, 
                    getFileName(filePart),
                    getFolder(filePart));
                LocalFileSystem.LocalFileSystemUtil.writeFile(partData[0].content_, destFilePath, FileMode.Append);
                
                e.part_++;
                RaiseWSFileSystemEvent(e);
            }
            //
            //
            arrFilePart = listFilePart.ToArray();
            return fileNameOnServer;
        }

        static string[] content2string(byte[] d)
        {
            return UTF8Encoding.UTF8.GetString(d,0,d.Length).Split(new char[] { '\n' });
        }

        public string[] ListLargeFile(string filename, string directory)
        {
            List<string> ret = new List<string>();
            int extLen = LocalFileSystem.LargeLocalFileInfo.FILE_NAME_EXT.Length;
            foreach (string f in List(filename, directory))
            {

                ret.Add(
                    f.Remove(f.Length - extLen,
                    extLen
                    ));
            }
            return ret.ToArray();
        }

        public string[] List(string filename, string directory)
        {
            // create an instance fo the web service
            uploaderWS.FileUploader srv = newUploader();
            string[] files = srv.List(sessionID_, filename, directory);
            return files;
        }

        public string Move(string oldfilename, string newfilename)
        {
            // create an instance fo the web service
            uploaderWS.FileUploader srv = newUploader();
            return srv.Move(sessionID_, oldfilename, newfilename);
        }
        /// <summary>
        /// 
        /// </summary>
        public string Url_;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public uploaderWS.FileUploader newUploader()
        {
            uploaderWS.FileUploader ret = new uploaderWS.FileUploader();
            ret.Url = Url_;
            ret.Timeout = -1;
            return ret;
        }


        public static string removeDriver(string filename)
        {
            if (filename.Length >= 3)
            {
                if ((filename[1] == ':') &&
                    (filename[2] == '\\'))
                    return filename.Substring(3);
            }

            return filename;
        }

        public static string getFileName(string filePath)
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

        void RaiseWSFileSystemEvent(WSFileSystemEventArgs e)
        {
            if( WSFileSystemEvent != null )
                WSFileSystemEvent(this, e);
        }
    }
}
