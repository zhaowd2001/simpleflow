using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebserviceFileSystem
{
    public class BigFile
    {
        public BigFile(string filePath, long bytesPerPart = 100 * 1024)
        {
            FilePath = filePath;
            fileInfo_ = new FileInfo(filePath);
            BytesPerPart = bytesPerPart;
        }

        public int getFilePartCount()
        {
            long n = fileInfo_.Length / BytesPerPart;
            if (fileInfo_.Length > n * BytesPerPart)
                n++;
            return (int)n;
        }


        int getFilePartSize(long index)
        {
            long c = getFilePartCount();
            if (index == c - 1)
            {
                return (int)(fileInfo_.Length - (c - 1) * BytesPerPart);
            }
            return (int)BytesPerPart;
        }

        public byte[] readFilePart(long index)
        {
            // set up a file stream and binary reader for the 
            // selected file
            using (FileStream fStream = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
            {
                fStream.Seek(index * BytesPerPart, SeekOrigin.Begin);
                byte[] ret = new byte[getFilePartSize(index)];

                // convert the file to a byte array
                int len = fStream.Read(ret, 0, getFilePartSize(index));
                return ret;
            }
        }

        public string FilePath { get; set; }
        public long BytesPerPart { get; set; }

        protected FileInfo fileInfo_;
    }
}
