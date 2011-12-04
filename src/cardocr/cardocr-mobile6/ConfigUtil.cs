using System;
using System.Text;
using System.Xml;

namespace Updater
{
    public class ConfigUtil
    {
        // Configuration
        private XmlDocument xmlConfig;

        public string getCheckAssemblyName()
        {
            return xmlConfig["updateinfo"]["checkassembly"].GetAttribute("name");
        }

        public bool init(out Exception e)
        {
            e = null;
            xmlConfig = new XmlDocument();
            try
            {
                xmlConfig.Load(cardocr_mobile6.camera_const.GetCurrentDirectory() + @"\updatecfg.xml");
                return true;
            }
            catch (Exception ex)
            {
                e = ex;
                return false;
            }
        }

        public string getRemoteApp()
        {
            return xmlConfig["updateinfo"]["remoteapp"].GetAttribute("name").ToUpper();
        }

        public string getServiceUrl()
        {
            return xmlConfig["updateinfo"]["service"].GetAttribute("url");
        }
    }
}
