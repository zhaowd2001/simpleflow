using System;
using System.Collections.Generic;
using System.Text;

namespace LocalFileSystem
{
    public class LargeLocalFileInfo
    {
        public static string FILE_NAME_EXT = ".largelocalfile";

        public static byte[] buildContent(string[] fileParts)
        {
            //convert to UTF-8 bytes
            List<byte> ret = new List<byte>();
            foreach (string s in fileParts)
            {
                ret.AddRange(UTF8Encoding.UTF8.GetBytes(s));
                ret.Add( 10 ); // 10 = \n
            }
            //
            //remove last char : \n
            ret.RemoveAt(ret.Count - 1);
            return ret.ToArray();
        }

        public static string[] parseContent(byte[] data)
        {
            string s = UTF8Encoding.UTF8.GetString(data,0,data.Length);
            string[] lines = s.Split(new char[] { '\n' });
            return lines;
        }
    }
}
