using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FileSystem;
using LocalFileSystem;
using WebserviceFileSystem;

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
            fileSystem_ = new WSFileSystem();
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
                else if (args[0] == "del")
                    msg = del(args[1]);
                else if (args[0] == "find")
                    msg = find(args[1], args[2]);
                else
                    usage();
                System.Console.Write(msg);
                }
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

        string del(string remoteFile)
        {
            return fileSystem_.Remove(remoteFile);
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
            return fileSystem_.DownloadLargeFile(remoteFileName, remoteDirectory, localPath);
        }

        void usage()
        {
            string help = @"upload/download file";
            System.Console.Write(help);
        }
    }
}
