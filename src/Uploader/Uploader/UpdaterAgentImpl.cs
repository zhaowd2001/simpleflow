using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Collections;
using System.ComponentModel;
using System.Threading;
using System.Xml;

namespace UpdaterAgent
{
    /// <summary>
    /// Summary description for WebService
    /// </summary>
    public class UpdaterAgentImpl
    {
        string updaterAgentFolder_;
        string requestUrl_;
        public UpdaterAgentImpl(string updaterAgentFolder, string requesturl)
        {
            this.updaterAgentFolder_ = updaterAgentFolder;
            this.requestUrl_ = requesturl;
        }

   		/*
        name	"CARDOCR"	string
		platform	"PPC"	string
		arch	"84017152"	string
		maj	1	int
		min	0	int
		bld	0	int*/
        public UpdateInfo GetUpdateInfo(string name, string arch,
          int maj, int min, int bld)
        {
            UpdateInfo ui = new UpdateInfo();
            //for debug
            ui.newVersion = string.Format("{0}, {1}, {2}, {3}, {4}",
                name, arch, maj, min, bld);
            Version ver = new Version(maj, min, bld);
            XmlDocument xmlUpdateCfg = new XmlDocument();
            //Attempt to load xml configuration 
            try
            {
                xmlUpdateCfg.Load(updaterAgentFolder_+"\\updatecfg.xml");
            }
            catch (Exception ex)
            {
                ui.IsAvailable = false;
                return ui;
            }

            string xq = string.Format(
                "//downloadmodule[@arch=\"{0}\" and @name=\"{4}\" " +
                "and ( version/@maj>{1} or version/@maj={1} " +
                " and (version/@min > {2} or version/@min = {2}" +
                "and version/@bld > {3}))]",
                arch, maj, min, bld, name);
            XmlElement nodeUpdate =
               (XmlElement)xmlUpdateCfg["updateinfo"].SelectSingleNode(xq);
            if (nodeUpdate == null)
            {
                ui.IsAvailable = false;
                return ui;
            }

            // Build UpdateInfo structure
            ui.IsAvailable = true;
            ui.newVersion = new Version(string.Format("{0}.{1}.{2}",
                nodeUpdate["version"].Attributes["maj"].Value,
                int.Parse(nodeUpdate["version"].Attributes["min"].Value),
                int.Parse(nodeUpdate["version"].Attributes["bld"].Value))).ToString();
            ui.Url = this.requestUrl_;
            ui.Url = ui.Url.Substring(0, ui.Url.LastIndexOf("/"));
            ui.Url += "/" + nodeUpdate.InnerText.Trim();
            return ui;
        }

    }

    public class UpdateInfo
    {
        public string Url;
        public bool IsAvailable;
        public string newVersion;
    }

}