using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace cardocr
{
    public class CardOcrImpl : IMessageHandler
    {
        AppConfigInfo exeInfo = AppConfigImpl.load();

        Guid sessionID_;
        string webServiceUrl_;
        public JobInfo Execute(Guid sessionID, string webServiceUrl, JobInfo job)
        {
            sessionID_ = sessionID;
            webServiceUrl_ = webServiceUrl;
            //
            string localFile = exeInfo.workingFolder_+"\\"+ job.RemoteFilePath;
            download(job.RemoteFilePath, localFile);

            string outputFile = localFile + ".txt";

            int result = ocrExecute(localFile, outputFile);
            job.Result = result;
            
            if (result == 0)
            {//success
                string rFile = job.RemoteFilePath + ".txt";
                upload(rFile, outputFile);
                job.ResultRemoteFilePath = rFile;
            }
            else
            {//error
            }
            //
            return job;
        }

        void upload(string remote, string local)
        {
            cardocr_mobile6.WSFileSystem f = newWebServiceFileSystem();

            string ret =
            f.UploadLargeFile(local, remote);
        }

        private cardocr_mobile6.WSFileSystem newWebServiceFileSystem()
        {
            cardocr_mobile6.WSFileSystem f = new cardocr_mobile6.WSFileSystem(sessionID_, webServiceUrl_);
            return f;
        }

        void download(string remote, string local)
        {
            cardocr_mobile6.WSFileSystem f = new cardocr_mobile6.WSFileSystem(sessionID_, webServiceUrl_);

            string[] files;
            string ret = 
            f.DownloadLargeFile(cardocr_mobile6.WSFileSystem.getFileName(remote),
                cardocr_mobile6.WSFileSystem.getFolder(remote),
                local,
                out files);
        }

        int ocrExecute(string input, string output)
        {
            int result = -1;
            exeInfo.parameter_ += " -inputpath " + input;
            exeInfo.parameter_ += " -outputfile " + output;
            result = launch(exeInfo);
            if (result == 0)//success
            {
                string resultStr = File.ReadAllText(output);
                if (resultStr.Length == 0)
                {
                    File.WriteAllText(output, "Nothing");
                }
            }
            //
            return result;
        }

        public string getAppID()
        {
            return "cardocr";
        }
        public string getVersion()
        {
            return "1.0";
        }

        int launch(AppConfigInfo exe)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();

            startInfo.FileName = exe.exePath_;
            startInfo.WorkingDirectory = exe.workingFolder_;
            startInfo.Arguments = exe.parameter_;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;

            Process p = System.Diagnostics.Process.Start(startInfo);
            string o = p.StandardOutput.ReadToEnd();
            string e = p.StandardError.ReadToEnd();
            p.WaitForExit();
            return p.ExitCode;
        }
    }
}
