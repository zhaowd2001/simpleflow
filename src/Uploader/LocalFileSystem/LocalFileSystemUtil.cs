using System;
using System.Collections.Generic;
using System.Text;

using System.IO;

namespace LocalFileSystem
{
    public class LocalFileSystemUtil
    {
        public static void createDirectoryForFile(string filePath)
        {
            FileInfo fi = new FileInfo(filePath);

            createDirectory(fi.Directory);
        }

        public static void FindFile(string DirPath,
            string fileSearchPattern,
            string subDir,
            ref List<String> AL)
        {
            //C#枚举文件的代码实现
            //列举出所有文件,添加到AL  

            foreach (string file in Directory.GetFiles(appendPath(DirPath, subDir), fileSearchPattern))
                AL.Add(file);

            //列举出所有子文件夹,并对之调用GetAllFileByDir自己;  
            //C#枚举文件的代码实现
            //foreach (string dir in Directory.GetDirectories(DirPath, dirSearchPattern))
            //    FindFile(dir, fileSearchPattern, dirSearchPattern, ref AL);
        }

        static string appendPath(string path, string subDir)
        {
            if (path[path.Length - 1] == '\\')
                return path + subDir;
            return path + "\\" + subDir;
        }

        public static void writeFile(byte[] f, string filePath, FileMode mode)
        {
            // the byte array argument contains the content of the file
            // the string argument contains the name and extension
            // of the file passed in the byte array
            // instance a memory stream and pass the
            // byte array to its constructor
            using (MemoryStream ms = new MemoryStream(f))
            {

                // instance a filestream pointing to the 
                // storage folder, use the original file name
                // to name the resulting file
                LocalFileSystemUtil.createDirectoryForFile(filePath);
                using (FileStream fs = new FileStream
                    (filePath, mode))
                {

                    // write the memory stream containing the original
                    // file as a byte array to the filestream
                    ms.WriteTo(fs);
                }
            }
            // return OK if we made it this far
        }

        public static byte[] readFile(string filePath)
        {
            String strFile = filePath;

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

            return data;
        }

        private static void createDirectory(DirectoryInfo d)
        {
            if (d.Exists)
                return;
            createDirectory(d.Parent);
            d.Create();
        }
    }
}
