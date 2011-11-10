using System;
using System.Data;
using System.Web;
using System.Collections;
using System.Collections.Generic;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.IO;

using FileSystem;
using LocalFileSystem;

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
            string filePath = getUploadFolder() + fileName;
            LocalFileSystemUtil.writeFile(f, filePath);
            return "OK";
        }

        [WebMethod]
        public FileContent[] DownloadFile(string fileSearchPattern, string dirSearchPattern)
        {
            List<FileContent> ret = new List<FileContent>();
            string[] files = List(fileSearchPattern, dirSearchPattern);
            foreach (string file in files)
            {
                ret.Add(new FileContent(file, LocalFileSystemUtil.readFile(file)));
            }
            return ret.ToArray();
        }

        [WebMethod]
        public string[] List(string fileSearchPattern, string dirSearchPattern)
        {
            List<String> al = new List<string>();
            LocalFileSystemUtil.FindFile(getUploadFolder(), fileSearchPattern, dirSearchPattern, ref al);
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

        private static string getUploadFolder()
        {
            return System.Web.Hosting.HostingEnvironment.MapPath("~/TransientStorage/");
        }
    }
}
