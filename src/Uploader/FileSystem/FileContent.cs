using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileSystem
{
    public class FileContent
    {
        public string path_ { get; set; }
        public byte[] content_ { get; set; }

        public FileContent()
        {
        }

        public FileContent(string p, byte[] d)
        {
            path_ = p;
            content_ = d;
        }
    };
}
