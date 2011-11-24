﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;

namespace AsyncGen
{
    public static class AsyncMethodsGenerator
    {
        private const string pattern = @"public\s+static\s+(\w+?)\s+(\w+?)\((.*?)\)\s+\{(.+?)\r\n        \}";

        private const string formatter = @"        public static void {0}({1}){3}{2}{4}";

        private static List<string> ignoreList = new List<string>();

        static AsyncMethodsGenerator()
        {
            var ignoreListFile = "AsyncIgnoreList.txt";
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

        public static void Generate(string syncMethodSouceCodeFilePath)
        {
            try
            {
                if (File.Exists(syncMethodSouceCodeFilePath))
                {
                    var syncText = File.ReadAllText(syncMethodSouceCodeFilePath);
                    var matches = Regex.Matches(syncText, pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);

                    var builder = new StringBuilder();

                    builder.AppendLine("using System;");
                    builder.AppendLine("using System.Collections.Generic;");
                    builder.AppendLine("using System.Collections.ObjectModel;");
                    builder.AppendLine("using System.Globalization;");
                    builder.AppendLine("using System.IO;");
                    builder.AppendLine("using System.Text;");
                    builder.AppendLine("using AMicroblogAPI.Common;");
                    builder.AppendLine("using AMicroblogAPI.DataContract;");
                    builder.AppendLine("using AMicroblogAPI.HttpRequests;");
                    builder.AppendLine();
                    
                    builder.AppendLine("namespace AMicroblogAPI");
                    builder.AppendLine("{");

                    builder.AppendLine("    // Do not modify. This file is generated by AsyncGen.");
                    builder.AppendLine("    public static partial class AMicroblog");
                    builder.AppendLine("    {");
                    builder.AppendLine("        #region Async Methods\r\n");

                    foreach (Match match in matches)
                    {
                        var returnVal = match.Groups[1].Value;
                        var methodName = match.Groups[2].Value;
                        var methodParams = match.Groups[3].Value;
                        var methodBody = match.Groups[4].Value;

                        if (string.IsNullOrEmpty(methodName) || methodName.EndsWith("Async") ||
                            ignoreList.Contains(methodName))
                        {
                            continue;
                        }

                        if (methodName == "ExistsFriendship")
                            returnVal = "ExistsFriendshipResultInfo";

                        if (methodBody.Trim().Contains("return " + methodName))
                        {
                            continue;
                        }

                        // Method Signature
                        var paramFormatter = string.Empty;
                        var newParams = string.Empty;

                        bool isVoidReturn = returnVal == "void";
                        if (isVoidReturn)
                        {
                            if (string.IsNullOrEmpty(methodParams.Trim()))
                                paramFormatter = "VoidAsyncCallback callback{0}";
                            else
                                paramFormatter = "VoidAsyncCallback callback, {0}";

                            newParams = string.Format(paramFormatter, methodParams);
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(methodParams.Trim()))
                                paramFormatter = "AsyncCallback<{0}> callback{1}";
                            else
                                paramFormatter = "AsyncCallback<{0}> callback, {1}";

                            newParams = string.Format(paramFormatter, returnVal, methodParams);
                        }

                        // Body
                        var lines = methodBody.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                        var bodyBuilder = new StringBuilder();
                        for (int i = 0; i < lines.Length; i++)
                        {
                            var line = lines[i];
                            if (line.Contains("Request();"))
                            {
                                break;
                            }

                            bodyBuilder.AppendLine(line);
                        }

                        bodyBuilder.AppendLine("            requester.RequestAsync(delegate(AsyncCallResult<string> result)");
                        bodyBuilder.AppendLine("            {");
                        if(isVoidReturn)
                            bodyBuilder.AppendLine("                ProcessAsyncCallbackVoidResponse(result, callback);");                        
                        else
                            bodyBuilder.AppendLine("                ProcessAsyncCallbackResponse(result, callback);");
                        bodyBuilder.AppendLine("            });");

                        var newBody = bodyBuilder.ToString();

                        var methodComments =
                            "        /// <summary>\r\n" +
                            "        /// The async implementation of <see cref=\"{0}\"/>\r\n" +
                            "        /// </summary>\r\n";
                        builder.AppendFormat(methodComments, methodName);

                        builder.AppendFormat(formatter, methodName + "Async", newParams, newBody, "\r\n        {\r\n", "        }\r\n\r\n");
                    }

                    builder.AppendLine("        #endregion");
                    builder.AppendLine("    }");
                    builder.AppendLine("}");


                    var path = Path.GetDirectoryName(syncMethodSouceCodeFilePath) + @"\AMicroblog.Async.g.cs";
                    File.WriteAllText(path, builder.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to generate async methods.", ex);
                throw;
            }
        }
    }
}