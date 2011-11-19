using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;

namespace APIMappingGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            WriteHighlight(ConsoleColor.Green, "APIMappingGenerator v1.0 2011 by Aldrick");
            Console.WriteLine();

            var filePath = string.Empty;
            if (null == args || args.Length == 0)
            {
                WriteHighlight(ConsoleColor.Red, "The xml comments file is not specified. Will try using AMicroblogAPI.xml");
                Console.WriteLine();
                filePath = "AMicroblogAPI.xml";
            }
            else
            {
                filePath = args[0];
            }

            try
            {
                APIMappingGenerator.Generate(filePath);

                WriteHighlight(ConsoleColor.Green, "API mapping generated successfully (Output as APIReport.htm and AMicroblogAPI.db).");
            }
            catch (Exception ex)
            {
                WriteHighlight(ConsoleColor.Red, string.Format("Failed to generate API mapping due to: {0}", ex.Message));
                Console.ReadKey();
            }
        }

        static void WriteHighlight(ConsoleColor highlightColor, string msg)
        {
            var fg = Console.ForegroundColor;
            Console.ForegroundColor = highlightColor;
            Console.WriteLine(msg);
            Console.ForegroundColor = fg;
        }
    }
}
