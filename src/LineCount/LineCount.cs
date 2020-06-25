using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Text.RegularExpressions;

namespace LineCount
{
    internal class MainProgram
    {
        static void Main(string[] args)
        {
            if (args.Length != 1 || !File.Exists(args[0]))
            {
                Usage();
                return;
            }

            int lineCount = 0;
            try
            {
                using (StreamReader streamReader = new StreamReader(args[0]))
                {
                    while (!streamReader.EndOfStream)
                    {
                        streamReader.ReadLine();
                        lineCount++;
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine($"Exception reading file: {args[0]}\r\n{e.Message}");
                return;
            }
            Console.WriteLine($"{lineCount}");
        }

        static private void Usage()
        {
            Console.WriteLine("Usage: LineCount filePath");
        }
    }
}
