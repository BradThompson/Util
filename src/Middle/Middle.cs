using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace EdmondsCommunityCollege
{
    internal class MainProgram
    {
        static void Main(string[] args)
        {
            if (args.Length < 3 || args.Length > 4)
            {
                Usage();
                return;
            }
            string path = args[0];
            if (!Int32.TryParse(args[1], out Int32 start))
            {
                Console.WriteLine("Invalid start");
                Usage();
                return;
            }
            if (!Int32.TryParse(args[2], out Int32 length))
            {
                Console.WriteLine("Invalid length");
                Usage();
                return;
            }
            int outputType = 1;
            if (args.Length == 4)
            {
                if (!Int32.TryParse(args[3], out outputType))
                {
                    Console.WriteLine("Invalid output type");
                    Usage();
                    return;
                }
                if (outputType < 1 || outputType > 4)
                {
                    Console.WriteLine("Valid outputType is 1, 2 or 3");
                    Usage();
                    return;
                }
            }
            ReadCharacters(path, start, length, outputType);
        }

        private static void ReadCharacters(string path, int start, int length, int outputType)
        {
            using (FileStream fs = File.OpenRead(path))
            {
                fs.Seek(start, SeekOrigin.Begin);
                byte[] buffer = new byte[length];
                UTF8Encoding temp = new UTF8Encoding(true);
                while (fs.Read(buffer, 0, buffer.Length) > 0)
                {
                    switch (outputType)
                    {
                        case 1:
                            Console.WriteLine(System.Text.Encoding.ASCII.GetString(buffer));
                            break;
                        case 2:
                            foreach (byte b in buffer)
                            {
                                Console.Write($"{b} ");
                            }
                            break;
                        case 3:
                            Console.WriteLine(System.Text.Encoding.ASCII.GetString(buffer));
                            foreach (byte b in buffer)
                            {
                                Console.Write("{0:X} ", b);
                            }
                            break;
                        default:
                            break;
                    }
                    // Only want the specific part of the file.
                    break;
                }
                Console.WriteLine();
            }
        }
        static private void Usage()
        {
            Console.WriteLine("Middle file start length [String = 1 Or Integer = 2 Or Hex = 3. Default is String]");
        }
    }
}
