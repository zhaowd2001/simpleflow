using System;
using System.Collections.Generic;
using System.Text;

namespace cardocr
{
    public class AppConfigImpl
    {
        public static string ConfigFilePath = "cardocr.json";
        public static AppConfigInfo load()
        {
            if (!System.IO.File.Exists(ConfigFilePath))
                return new AppConfigInfo();
            string json =
            System.IO.File.ReadAllText(ConfigFilePath);
            AppConfigInfo i = Newtonsoft.Json.JsonConvert.DeserializeObject<AppConfigInfo>(json);
            return i;
        }
        public static void save(AppConfigInfo i)
        {
            if (i == null)
                i = new AppConfigInfo();
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(i);
            System.IO.File.WriteAllText(ConfigFilePath, json);
        }
    }
}
