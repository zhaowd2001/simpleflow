using System;
using System.Collections.Generic;
using System.Text;

namespace cardocr_mobile6
{
    public class camera_const
    {
        public static string getMessagePrefix()
        {
            return "`cardocr`1.0`";
        }

        public static string getRemoteFolder()
        {
            return "temp";
        }

        public static string getRemoteFilePath()
        {
            return getRemoteFolder() + @"\" + CAMERA_FILENAME;
        }

        public static string getTargetAll()
        {
            return "<all>";
        }

        public static string getCameraFilePath()
        {
            return CAMERA_FOLDER + @"\" + CAMERA_FILENAME;
        }

        public static string CAMERA_FOLDER = @"\My Documents";
        public static string CAMERA_FILENAME = @"test.jpg";
    }
}
