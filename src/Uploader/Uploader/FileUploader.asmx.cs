using System;
using System.Data;
using System.Web;
using System.Collections;
using System.Collections.Generic;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.IO;


namespace Uploader
{
    /// <summary>
    /// This web method will provide an web method to load any
    /// file onto the server; the UploadFile web method
    /// will accept the report and store it in the local file system.
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    public class FileUploader : System.Web.Services.WebService
    {

        [WebMethod]
        public string UploadFile(byte[] f, string fileName)
        {
            // the byte array argument contains the content of the file
            // the string argument contains the name and extension
            // of the file passed in the byte array
            try
            {
                // instance a memory stream and pass the
                // byte array to its constructor
                MemoryStream ms = new MemoryStream(f);

                // instance a filestream pointing to the 
                // storage folder, use the original file name
                // to name the resulting file
                FileStream fs = new FileStream
                    (getUploadFolder() +
                    fileName, FileMode.Create);

                // write the memory stream containing the original
                // file as a byte array to the filestream
                ms.WriteTo(fs);

                // clean up
                ms.Close();
                fs.Close();
                fs.Dispose();

                // return OK if we made it this far
                return "OK";
            }
            catch (Exception ex)
            {
                // return the error message if the operation fails
                return ex.Message.ToString();
            }
        }

        [WebMethod]
        public Dictionary<string, byte[]> DownloadFile(string fileSearchPattern, string dirSearchPattern)
        {
            Dictionary<string, byte[]> ret = new Dictionary<string, byte[]>();
            string[] files = List(fileSearchPattern, dirSearchPattern);
            foreach (string file in files)
            {
                ret.Add(file, readfile2byte(file));
            }
            return ret;
        }

        [WebMethod]
        public string[] List(string fileSearchPattern, string dirSearchPattern)
        {
            List<String> al = new List<string>();
            GetAllFileByDir(getUploadFolder(), fileSearchPattern, dirSearchPattern, ref al);
            return al.ToArray();
        }

        [WebMethod]
        public string Move(string oldFileName, string newFileName)
        {
            checkFileName(oldFileName);
            checkFileName(newFileName);
            File.Move(getUploadFolder() + oldFileName, getUploadFolder() + newFileName);
            return "OK";
        }

        [WebMethod]
        public string Remove(string fileName)
        {
            checkFileName(fileName);
            File.Delete(getUploadFolder() + fileName);
            return "OK";
        }

        private static void checkFileName(string fileName)
        {
            FileInfo fi = new FileInfo(getUploadFolder() + fileName);
            //remove last char '\'
            string s = fi.Directory.FullName.ToLower().Substring(0, getUploadFolder().Length - 1);
            string s2 = getUploadFolder().ToLower().Substring(0, getUploadFolder().Length - 1);
            if (s != s2)
                new Exception("invalid filename:" + fileName);
        }

        private static void GetAllFileByDir(string DirPath,
            string fileSearchPattern,
            string dirSearchPattern,
            ref List<String> AL)
        {
            //C#枚举文件的代码实现
            //列举出所有文件,添加到AL  

            foreach (string file in Directory.GetFiles(DirPath, fileSearchPattern))
                AL.Add(file);

            //列举出所有子文件夹,并对之调用GetAllFileByDir自己;  
            //C#枚举文件的代码实现
            foreach (string dir in Directory.GetDirectories(DirPath, dirSearchPattern))
                GetAllFileByDir(dir, fileSearchPattern, dirSearchPattern, ref AL);
        }

        private static byte[] readfile2byte(string filePath)
        {
            checkFileName(filePath);
            String strFile = getUploadFolder() + filePath;

            // get the file information form the selected file
            FileInfo fInfo = new FileInfo(strFile);

            long numBytes = fInfo.Length;
            // set up a file stream and binary reader for the 
            // selected file
            FileStream fStream = new FileStream(strFile, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fStream);

            // convert the file to a byte array
            byte[] data = br.ReadBytes((int)numBytes);
            br.Close();

            fStream.Close();
            fStream.Dispose();

            return data;
        }

        private static string getUploadFolder()
        {
            return System.Web.Hosting.HostingEnvironment.MapPath("~/TransientStorage/");
        }
    }
}
