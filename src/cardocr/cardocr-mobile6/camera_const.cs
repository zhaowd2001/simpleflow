using System;
using System.Collections.Generic;
using System.Text;

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
            return CAMERA_FOLDER + @"\" + fileName;
        }

        public static string CAMERA_FOLDER = @"\My Documents";
    }
}
