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
        string serverName_;
        string scriptName_;
        public UpdaterAgentImpl(string updaterAgentFolder, string serverName, string scriptName)
        {
            this.updaterAgentFolder_ = updaterAgentFolder;
            this.serverName_ = serverName;
            this.scriptName_ = scriptName;
        }

        public UpdateInfo GetUpdateInfo(string name, string arch,
          int maj, int min, int bld)
        {
            UpdateInfo ui = new UpdateInfo();
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
            string srv = this.serverName_;// Context.Request.ServerVariables["SERVER_NAME"];
            string path = this.scriptName_;// Context.Request.ServerVariables["SCRIPT_NAME"];
            ui.Url = string.Format("http://{0}{1}", srv, path);
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