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
        FileUploaderImpl uploaderImpl = new FileUploaderImpl(getUploadFolder());
        private static string getUploadFolder()
        {
            return System.Web.Hosting.HostingEnvironment.MapPath("~/TransientStorage/");
        }


        [WebMethod]
        public string UploadFile(byte[] f, string fileName)
        {
            return uploaderImpl.UploadFile(f, fileName);
        }

        [WebMethod]
        public FileContent[] DownloadFile(string fileSearchPattern, string dirSearchPattern)
        {
            return uploaderImpl.DownloadFile(fileSearchPattern, dirSearchPattern);
        }

        [WebMethod]
        public string[] List(string fileSearchPattern, string dirSearchPattern)
        {
            return uploaderImpl.List(fileSearchPattern, dirSearchPattern);
        }

        [WebMethod]
        public string Move(string oldFileName, string newFileName)
        {
            return uploaderImpl.Move(oldFileName, newFileName);
        }

        [WebMethod]
        public string Remove(string fileName)
        {
            return uploaderImpl.Remove(fileName);
        }
    }
}
