using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace AsyncGen
{
    class Program
    {
        static void Main(string[] args)
        {
            WriteHighlight(ConsoleColor.Green, "AsyncGen v1.0 2011 by Aldrick");
            Console.WriteLine();

            if (null == args || args.Length == 0)
            {
                WriteHighlight(ConsoleColor.Red, "The source code file(.cs) of sync methods is not specified.");
                Console.ReadKey();
            }
            else
            {
                var filePath = args[0];
                if (!File.Exists(filePath))
                {
                    WriteHighlight(ConsoleColor.Red, "The specified source code file does not exist.");
                    Console.ReadKey();
                }
                else
                {
                    try
                    {
                        AsyncMethodsGenerator.Generate(filePath);

                        WriteHighlight(ConsoleColor.Green, "Async methods file (AMicroblog.Async.g.cs) generated successfully.");
                    }
                    catch (Exception ex)
                    {
                        WriteHighlight(ConsoleColor.Red, string.Format("Failed to generate due to: {0}", ex.Message));                        
                        Console.ReadKey();
                    }
                }
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
