using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace LocalFileSystem
{
    public class LargeLocalFileReader
    {
        public LargeLocalFileReader(string filePath, long bytesPerPart /*= 100 * 1024*/)
        {
            FilePath_ = filePath;
            fileInfo_ = new FileInfo(filePath);
            BytesPerPart_ = bytesPerPart;
        }

        public int getFilePartCount()
        {
            long n = fileInfo_.Length / BytesPerPart_;
            if (fileInfo_.Length > n * BytesPerPart_)
                n++;
            return (int)n;
        }


        int getFilePartSize(long index)
        {
            long c = getFilePartCount();
            if (index == c - 1)
            {
                return (int)(fileInfo_.Length - (c - 1) * BytesPerPart_);
            }
            return (int)BytesPerPart_;
        }

        public byte[] readFilePart(long index)
        {
            // set up a file stream and binary reader for the 
            // selected file
            using (FileStream fStream = new FileStream(FilePath_, FileMode.Open, FileAccess.Read))
            {
                fStream.Seek(index * BytesPerPart_, SeekOrigin.Begin);
                byte[] ret = new byte[getFilePartSize(index)];

                // convert the file to a byte array
                int len = fStream.Read(ret, 0, getFilePartSize(index));
                return ret;
            }
        }

        public string FilePath_;
        public long BytesPerPart_;

        protected FileInfo fileInfo_;
    }
}
