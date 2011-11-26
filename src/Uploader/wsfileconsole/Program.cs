using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FileSystem;
using LocalFileSystem;
using WebserviceFileSystem;
using cardocr_mobile6;

namespace wsfileconsole
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                new Program().doMain(args);
            }
            catch (Exception e)
            {
                System.Environment.ExitCode = 1;
                throw e;
            }
        }


        WSFileSystem fileSystem_;
        
        void onWSFileSystemEvent(object Sender, WSFileSystemEventArgs e)
        {
            System.Console.Write(string.Format("{0}/{1}. ", e.part_, e.partCount_));
        }

        void doMain(string[] args)
        {
            Guid sessionID = Guid.NewGuid();
            fileSystem_ = new WSFileSystem(sessionID, "http://13.187.242.140/mb/FileUploader.asmx");
            if( getWebServiceUrl().Length >0)
                fileSystem_.Url_ = getWebServiceUrl();
            fileSystem_.WSFileSystemEvent += this.onWSFileSystemEvent;
            //
            handleCommandLine(args);
            //
            fileSystem_.WSFileSystemEvent -= this.onWSFileSystemEvent;
        }

        private void handleCommandLine(string[] args)
        {
            if (args.Length == 0)
            {
                usage();
            }
            else{
                string msg = "";
                if (args[0] == "upload")
                   msg = upload(args[1], args[2]);
                else if (args[0] == "download")
                      msg = download(args[1], args[2]);
                else if (args[0] == "move")
                    msg = move(args[1], args[2]);
                else if (args[0] == "del"){
                    List<String> filesToDel = new List<string>();
                    filesToDel.AddRange(args);
                    filesToDel.RemoveAt(0);
                    msg = del( filesToDel.ToArray());
                }
                else if (args[0] == "find")
                    msg = find(args[1], args[2]);
                else if (args[0] == "upload-move")
                    msg = Upload_Move(args[1], args[2], args[3]);
                else if (args[0] == "find-download-del")
                    msg = Find_Download_Del(args[1], args[2], args[3]);
                else
                    usage();
                System.Console.Write(msg);
                }
        }

        ///
        /// find-download-del:
        /// parameters: <remote file>   <remote folder>   <local folder>
        ///    1.find files in <remote folder>;
        ///    2.download them to <local folder>;
        ///    3.remove them from <remote folder>.
        ///    
        string Find_Download_Del(string remoteFile, string remoteFolder, string localFolder)
        {
            string ret = "";
            //find
            string[] remoteFiles = fileSystem_.ListLargeFile(remoteFile, remoteFolder);

            List<string[]> remoteFileParts = new List<string[]>();
            //download
            foreach (string remoteFilePath in remoteFiles)
            {
                string[] fileParts = null;
                ret += fileSystem_.DownloadLargeFile(
                    WSFileSystem.getFileName(remoteFilePath),
                    WSFileSystem.getFolder(remoteFilePath),
                    localFolder + "\\"+WSFileSystem.getFileName(remoteFilePath),
                    out fileParts);
                remoteFileParts.Add(fileParts);
                ret += "\n";
            }
            //del
            foreach (string[] fileParts in remoteFileParts)
            {
                ret += fileSystem_.Remove(fileParts);
                ret += "\n";
            }
            return ret;
        }

        string upload(string localFilePath, string remoteFolder)
        {
            return fileSystem_.UploadLargeFile(localFilePath, remoteFolder);
        }

        string find(string remoteFileName, string remoteFolder)
        {
            string[] files= fileSystem_.List(remoteFileName, remoteFolder);
            return string.Join("\n", files);
        }

        string del(string[] remoteFiles)
        {
            return fileSystem_.Remove(remoteFiles);
        }

        string move(string remoteFile1, string remoteFile2)
        {
            return fileSystem_.Move(remoteFile1, remoteFile2);
        }

        string download(string remoteFilePath, string localFolder)
        {
            string remoteFileName, remoteDirectory, localPath;
            remoteFileName = WSFileSystem.getFileName(remoteFilePath);
            remoteDirectory = WSFileSystem.getFolder(remoteFilePath);
            localPath = localFolder + "\\" + remoteFileName;

            string[] fileParts;
            return fileSystem_.DownloadLargeFile(remoteFileName, remoteDirectory, localPath, out fileParts);
        }

        ///
        ///upload-move:
        ///parameters: <local path>    <remote folder>   <remote folder2>
        ///1.upload <local path> to <remote folder>;
        ///2.move it from <remote folder> to <remote folder2>
        ///
        string Upload_Move(string localPath, string remoteFolder, string remoteFolder2)
        {
            string ret = upload(localPath, remoteFolder);
            ret += "\n";
            string remoteFileName = WSFileSystem.getFileName(localPath) + LargeLocalFileInfo.FILE_NAME_EXT;
            ret += move(remoteFolder + "\\" + remoteFileName,
                remoteFolder2 + "\\" + remoteFileName);
            return ret;
        }

        static string getWebServiceUrl()
        {
            return System.Configuration.ConfigurationManager
                .AppSettings["UploaderUrl"];
        }

        void usage()
        {
            string help = @"Web Service file system.
simple command:
upload   <local path>    <remote folder>
download <remote path>   <local folder>
move     <remote source> <remote destination>
del      <remote path>   <remote path 2> <...>
find     <remote file>   <remote folder>

complex command:
find-download-del:
parameters: <remote file>   <remote folder>   <local folder>
   1.find files in <remote folder>;
   2.download them to <local folder>;
   3.remove them from <remote folder>.

upload-move:
parameters: <local path>    <remote folder>   <remote folder2>
   1.upload <local path> to <remote folder>;
   2.move it from <remote folder> to <remote folder2>
";
            System.Console.Write(help);
        }
    }
}
