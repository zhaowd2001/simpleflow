using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileSystem
{
    public class FileItem
    {
        public string path { get; set; }
        public byte[] data { get; set; }

        public FileItem()
        {
        }

        public FileItem(string p, byte[] d)
        {
            path = p;
            data = d;
        }
    };
}
