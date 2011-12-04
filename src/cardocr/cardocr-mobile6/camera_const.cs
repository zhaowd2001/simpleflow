using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;

namespace cardocr_mobile6
{
    public class camera_const
    {
        public static string getCardOcr_AppID()
        {
            return "cardocr";
        }

        public static string getCardOcr_Version()
        {
            return "1.0";
        }

        public static string getRemoteFolder()
        {
            return "temp";
        }

        public static string getRemoteFilePath(string fileName)
        {
            return getRemoteFolder() + @"\" + fileName;
        }

        public static string getTargetAll()
        {
            return "<all>";
        }

        public static string getCameraFilePath(string fileName)
        {
            return getCameraWorkFolder() + @"\" + fileName;
        }

        //public static string CAMERA_FOLDER = @"\My Documents";
        public static string getCameraWorkFolder()
        {
            string path = GetCurrentDirectory() + "\\temp";
            return path;
        }

        // Helper procedure
        public static string GetCurrentDirectory()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);
        }
    }
}
