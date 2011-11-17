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
using MessageBus;

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
        FileUploaderImpl uploaderImpl_ = new FileUploaderImpl(getUploadFolder());
        private static string getUploadFolder()
        {
            return System.Web.Hosting.HostingEnvironment.MapPath("~/TransientStorage/");
        }


        [WebMethod]
        public string UploadFile(byte[] f, string fileName)
        {
            return uploaderImpl_.UploadFile(f, fileName);
        }

        [WebMethod]
        public FileContent[] DownloadFile(string fileSearchPattern, string dirSearchPattern)
        {
            return uploaderImpl_.DownloadFile(fileSearchPattern, dirSearchPattern);
        }

        [WebMethod]
        public string[] List(string fileSearchPattern, string dirSearchPattern)
        {
            return uploaderImpl_.List(fileSearchPattern, dirSearchPattern);
        }

        [WebMethod]
        public string Move(string oldFileName, string newFileName)
        {
            return uploaderImpl_.Move(oldFileName, newFileName);
        }

        [WebMethod]
        public string Remove(string fileName)
        {
            return uploaderImpl_.Remove(fileName);
        }

        //////////////////////////////////////////////////////////
        MessageBusImpl busImpl_ = new MessageBusImpl();
        // I wish to have something like commented text below to be able setup event staff for WebService
        // (see also commented out ActiveClientsChangedDelegate definition at the end of this namespace)
        // but unfortunately it's not a case:
        //[WebEvent]
        //public event ActiveClientsChangedDelegate OnActiveClientsChanged = null;

        #region WebService interface
        [WebMethod]
        public void StartSession(Guid sessionID, String clientID)
        {
            busImpl_.StartSession(sessionID, clientID);
        }

        [WebMethod]
        public void SendMessage(
            Guid sessionID,
            MessageBusImpl.Message msg)
        {
            busImpl_.SendMessage(sessionID, msg);
        }

        [WebMethod]
        public void StopSession(Guid sessionID)
        {
            busImpl_.StopSession(sessionID);
        }

        [WebMethod]
        public MessageBusImpl.GetMessageResult GetMessage(Guid sessionID)
        {
            return busImpl_.GetMessage(sessionID);
        }
        #endregion
    }
}
