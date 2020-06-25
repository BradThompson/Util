using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Text.RegularExpressions;

namespace idir
{
    internal class MainProgram
    {
        static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                if (Environment.GetLogicalDrives().Length < 2)
                {
                    Console.WriteLine("There are no mapped drives.");
                    return -1;
                }
                foreach (var item in Environment.GetLogicalDrives())
                {
                    if (TryPrintConnection(item, Environment.CurrentDirectory))
                    {
                        return 0;
                    }
                }
                Console.WriteLine("File was not on a mapped drive");
                return -1;
            }
            if (args.Length > 1)
            {
                Usage();
                return -1;
            }
            FileInfo fi = new FileInfo(args[0]);
            if (fi.Exists)
            {
                foreach (var item in Environment.GetLogicalDrives())
                {
                    if (TryPrintConnection(item, fi.FullName))
                    {
                        return 0;
                    }
                }
                Console.WriteLine("File was not on a mapped drive");
                return -1;
            }
            DirectoryInfo di = new DirectoryInfo(args[0]);
            if (di.Exists)
            {
                foreach (var item in Environment.GetLogicalDrives())
                {
                    if (TryPrintConnection(item, di.FullName))
                    {
                        return 0;
                    }
                }
                Console.WriteLine("Directory was not on a mapped drive");
                return -1;
            }
            Console.WriteLine("File or directory was not found");
            return -1;
        }

        static bool TryPrintConnection(string drive, string path)
        {
            string UNCPath;

            if (drive.ToUpper() == path.Substring(0, 3).ToUpper())
            {
                if (!UNC.TryGetUNCPath(drive, out UNCPath))
                    return false;
                Console.WriteLine("{0}{1}", UNCPath, path.Substring(3));
                return true;
            }
            return false;
        }

        static private void Usage()
        {
            Console.WriteLine("Your usage");
        }
    }
}
