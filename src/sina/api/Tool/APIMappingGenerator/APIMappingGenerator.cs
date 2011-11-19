using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data.SQLite;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;

namespace APIMappingGenerator
{
    public static class APIMappingGenerator
    {
        private static List<string> ignoreList = new List<string>();
        static APIMappingGenerator()
        {
            var ignoreListFile = "IgnoreList.txt";
            if (File.Exists(ignoreListFile))
            {
                var ignoredMethods = File.ReadAllLines(ignoreListFile);
                foreach (var item in ignoredMethods)
                {
                    if (!string.IsNullOrEmpty(item) || !item.StartsWith("//"))
                        ignoreList.Add(item);
                }
            }
        }

        private static DbConnection CreateConnection()
        {
            var conn = SQLiteFactory.Instance.CreateConnection();
            conn.ConnectionString = "Data Source=AMicroblogAPI.db";
            return conn;
        }

        public static void Generate(string apiCommentXmlFileLocation)
        {
            var xml = File.ReadAllText(apiCommentXmlFileLocation);
            xml = Regex.Replace(xml, "<see cref=\"(.+?)\"/>", "$1");
            xml = Regex.Replace(xml, "<paramref name=\"(.+?)\"/>", "param '$1'");
            xml = Regex.Replace(xml, "<c>(.+?)</c>", "$1");

            var xmlDoc = XmlSerializationHelper.XmlToObject<XmlDoc>(xml);
            var apiMembers = new Collection<XmlDocMember>();
            var asyncAPIMembers = new Collection<XmlDocMember>();
            foreach (var item in xmlDoc.Members)
            {
                if (item.MemberType == MemberType.Method && item.Name.StartsWith("M:AMicroblogAPI.AMicroblog."))
                {
                    if (Regex.IsMatch(item.Name, @"M:AMicroblogAPI.AMicroblog.(\w+?)Async\("))
                        asyncAPIMembers.Add(item);
                    else
                        apiMembers.Add(item);
                }
            }

            var apiInfos = new Collection<APIInfo>();
            foreach (var apiMember in apiMembers)
            { 
                var apiInfo = new APIInfo();
                apiInfo.APIMethodSignature = apiMember.Name.Replace("M:AMicroblogAPI.AMicroblog.", string.Empty);

                var nameMatch = Regex.Match(apiInfo.APIMethodSignature, @"^(.+?)\(", RegexOptions.IgnoreCase);
                if (nameMatch.Success)
                    apiInfo.APIMethodName = nameMatch.Groups[1].Value;
                else
                    apiInfo.APIMethodName = apiInfo.APIMethodSignature;

                // Goes through ignore list.
                if (ignoreList.Contains(apiInfo.APIMethodName))
                {
                    continue;
                }

                var remarks = apiMember.Remarks;
                if (string.IsNullOrEmpty(remarks) && null != apiMember.Summary)
                    remarks = apiMember.Summary.Remarks;

                if (!string.IsNullOrEmpty(remarks))
                {
                    var match = Regex.Match(remarks, @"See (http://open.weibo.com/wiki/([\w_/]+?))[\s\n]", RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        apiInfo.WikiUri = match.Groups[1].Value.ToLowerInvariant();
                        apiInfo.APIUri = match.Groups[2].Value.ToLowerInvariant(); ;
                    }
                }

                if(null != apiMember.Summary)
                    apiInfo.Summary = apiMember.Summary.Summary.Trim('\n', ' ');

                apiInfos.Add(apiInfo);
            }

            GenerateHtmlReport(apiInfos);

            GenerateDB(apiInfos);
        }

        public static void GenerateHtmlReport(IEnumerable<APIInfo> apiInfos)
        {
            var template = File.ReadAllText("MappingReportTemplate.htm");
            var apiBuilder = new StringBuilder();
            var formmater = "<tr><td class=\"tdStyle\">&nbsp;{0}</td><td class=\"tdStyle\">&nbsp;<a href=\"{3}\" target=\"_blank\">{2}</a></td><td class=\"tdStyle\">&nbsp;{4}</td></tr> <tr><td colspan=\"3\" class=\"senTDStyle\"  style=\"color:Gray;font-size: 11px;\">&nbsp;&nbsp;<em>{1}</em></td></tr>";
            foreach (var api in apiInfos)
            {
                apiBuilder.AppendFormat(formmater, api.APIMethodName, api.APIMethodSignature, api.APIUri, api.WikiUri, api.Summary);
            }

            template = template.Replace("{APIs}", apiBuilder.ToString());

            File.WriteAllText("APIMapping.htm", template, Encoding.UTF8);
        }

        public static void GenerateDB(IEnumerable<APIInfo> apiInfos)
        {
            using (var conn = CreateConnection())
            {
                conn.Open();
                var tran = conn.BeginTransaction();

                var cmd = conn.CreateCommand();
                cmd.CommandText = "DELETE FROM [APIMapping]";
                cmd.ExecuteNonQuery();

                foreach (var apiInfo in apiInfos)
                {
                    cmd = conn.CreateCommand();
                    cmd.CommandText = "INSERT INTO [APIMapping]([APIMethodName], [APIMethodSignature], [APIUri], [WikiUri], [Summary]) VALUES(@APIMethodName, @APIMethodSignature, @APIUri, @WikiUri, @Summary)";
                    cmd.Parameters.Add(new SQLiteParameter("@APIMethodName", apiInfo.APIMethodName));
                    cmd.Parameters.Add(new SQLiteParameter("@APIMethodSignature", apiInfo.APIMethodSignature));
                    cmd.Parameters.Add(new SQLiteParameter("@APIUri", apiInfo.APIUri));
                    cmd.Parameters.Add(new SQLiteParameter("@WikiUri", apiInfo.WikiUri));
                    cmd.Parameters.Add(new SQLiteParameter("@Summary", apiInfo.Summary));

                    cmd.ExecuteNonQuery();
                }

                tran.Commit();
            }
        }
    }
}
