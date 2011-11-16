using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

using FileSystem;

namespace LocalFileSystem
{
    /// <summary>
    /// This web method will provide an web method to load any
    /// file onto the server; the UploadFile web method
    /// will accept the report and store it in the local file system.
    /// </summary>
    public class FileUploaderImpl
    {

        public FileUploaderImpl(string uploadFolder)
        {
            this.UploadFolder = uploadFolder;
        }
        //
        public string UploadFile(byte[] f, string fileName)
        {
            string filePath = UploadFolder + fileName;
            LocalFileSystemUtil.writeFile(f, filePath, FileMode.Create);
            return removeUploadFolder(filePath);
        }

        //
        public FileContent[] DownloadFile(string fileSearchPattern, string dirSearchPattern)
        {
            List<FileContent> ret = new List<FileContent>();
            string[] files = List(fileSearchPattern, dirSearchPattern);
            foreach (string file in files)
            {
                ret.Add(new FileContent(file, LocalFileSystemUtil.readFile(
                    UploadFolder+file)));
            }
            return ret.ToArray();
        }

        //
        public string[] List(string fileSearchPattern, string dirSearchPattern)
        {
            List<String> al = new List<string>();
            LocalFileSystemUtil.FindFile(UploadFolder, fileSearchPattern, dirSearchPattern, ref al);
            
            List<String> ret = new List<string>();
            foreach (String fullPath in al)
            {
                ret.Add(removeUploadFolder(fullPath));
            }
            return ret.ToArray();
        }

        //
        public string Move(string oldFileName, string newFileName)
        {
            checkFileName(oldFileName);
            checkFileName(newFileName);
            //create folder for new file
            string oldFile = UploadFolder + oldFileName;
            string newFile = UploadFolder + newFileName;

            LocalFileSystemUtil.createDirectoryForFile(newFile);

            if (File.Exists(newFile))
            {//copy then del

                File.Copy(oldFile, newFile, true);
                File.Delete(oldFile);
            }else
            {//direct move
                //
                File.Move( oldFile, newFile);
            }
            return newFileName;
        }

        //
        public string Remove(string fileName)
        {
            checkFileName(fileName);
            string fullPath = UploadFolder + fileName;
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return fileName;
            }
            return "NOTFOUND";
        }

        private void checkFileName(string fileName)
        {
            FileInfo fi = new FileInfo(UploadFolder + fileName);
            //remove last char '\'
            string s = fi.Directory.FullName.ToLower().Substring(0, UploadFolder.Length - 1);
            string s2 = UploadFolder.ToLower().Substring(0, UploadFolder.Length - 1);
            if (s != s2)
                new Exception("invalid filename:" + fileName);
        }

        public string UploadFolder { get; set; }

        private string removeUploadFolder(string fullPath)
        {
            return fullPath.Substring(UploadFolder.Length);
        }
    }
}
