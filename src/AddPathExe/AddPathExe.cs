using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace AddPathExe
{
    public class AddPathExe
    {
        static string BatchFilename = "AddPathExe.bat";
        static public string BatchFullPath;
        static public bool RemovePath = false;
        static public bool SkipDirectoryTest = false;
        static public int InsertLocation = int.MaxValue;
        static public string PathToAdd = "";

        static public int Main(string[] args)
        {
            try
            {
                if (!Initialize(args))
                {
                    Console.WriteLine("Could not initialize");
                    return 0;
                }
                string oldPath = Environment.GetEnvironmentVariable("PATH");
                if (string.IsNullOrWhiteSpace(oldPath))
                {
                    oldPath = "";
                }
                string[] parts = oldPath.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                Dictionary<string, string> convertedPath = new Dictionary<string, string>();
                DirectoryInfo ptadi = new DirectoryInfo(PathToAdd);
                // Take the path apart. Remove duplicates and the old path if any.
                foreach (var part in parts)
                {
                    DirectoryInfo di = new DirectoryInfo(part);
                    if (convertedPath.ContainsKey(di.FullName))
                    {
                        Console.WriteLine("Path contains duplicate: {0}.", di.FullName);
                    }
                    // Only add to the path if it's not a duplicate and not the PathToAdd.
                    if (!convertedPath.ContainsKey(di.FullName) && di.FullName != ptadi.FullName)
                    {
                        convertedPath.Add(di.FullName, part.Trim());
                    }
                }

                string newPath = "";
                string semicolon = "";
                bool isInserted = false;
                int index = 0;
                foreach (var part in convertedPath)
                {
                    if (index == InsertLocation && !RemovePath)
                    {
                        isInserted = true;
                        newPath += semicolon + ptadi.FullName;
                        semicolon = ";";
                    }
                    newPath += semicolon + part.Value;
                    semicolon = ";";
                    index++;
                }
                if (InsertLocation >= convertedPath.Count && !RemovePath && !isInserted)
                {
                    newPath += semicolon + ptadi.FullName;
                }
                CreateBatchFile(string.Format("SET Path={0}\r\n", newPath));
                return 1;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception {0} with message: {1}", e.GetType(), e.Message);
                return 0;
            }
        }

        static bool Initialize(string[] args)
        {
            if (!ProcessCommandLine(new List<string>(args)))
            {
                Usage();
                return false;
            }
            Debug.Assert(!string.IsNullOrWhiteSpace(PathToAdd), "ProcessCommandLine must find PathToAdd");
            string tempDirectory = Environment.GetEnvironmentVariable("TEMP");
            if (string.IsNullOrEmpty(tempDirectory))
            {
                Console.WriteLine("Environment variable TEMP was empty.");
                return false;
            }
            BatchFullPath = Path.Combine(tempDirectory, BatchFilename);
            try
            {
                if (File.Exists(BatchFullPath))
                {
                    File.Delete(BatchFullPath);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error with BatchFile: {0}", e.Message);
                return false;
            }
            if (!SkipDirectoryTest && !RemovePath && !Directory.Exists(PathToAdd))
            {
                Console.WriteLine("Cannot add the path. It does not exist:");
                Console.WriteLine("    - {0}", PathToAdd);
                return false;
            }
            return true;
        }

        static bool ProcessCommandLine(List<string> args)
        {
            if (args.Count == 0)
                return false;
            foreach (var arg in args)
            {
                if (arg.IndexOf(';') >= 0)
                {
                    Console.WriteLine("Paths may not contain a semicolon");
                    return false;
                }
                if (arg.Substring(0, 1) == "-" || arg.Substring(0, 1) == "/")
                {
                    string param = arg.Substring(1);
                    if (param.Length == 0)
                    {
                        Console.WriteLine("Switches must have values");
                        return false;
                    }
                    switch (param.Substring(0, 1).ToUpper())
                    {
                        case "S":
                            SkipDirectoryTest = true;
                            break;
                        case "R":
                            RemovePath = true;
                            break;
                        case "I":
                            if (param.Length == 1)
                            {
                                Console.WriteLine("I parameter must specify index as in /I##");
                                return false;
                            }
                            if (!int.TryParse(param.Substring(1), out int result))
                            {
                                Console.WriteLine("I parameter must be an integer.");
                                return false;
                            }
                            if (result < 0)
                            {
                                Console.WriteLine("I parameter must be positive.");
                                return false;
                            }
                            InsertLocation = result;
                            break;
                        default:
                            Console.WriteLine("Unsupported switch: {0}", arg);
                            return false;
                    }
                    continue;
                }
                // If PathToAdd has already been set, that's incorrect.
                if (!string.IsNullOrWhiteSpace(PathToAdd))
                {
                    Console.WriteLine("PathToAdd is already set to\r\n{0}\r\n, cannot set to\r\n{1}", PathToAdd, arg);
                    return false;
                }
                PathToAdd = arg.Replace("\"", "").Trim();
            }
            // Cannot use /I## and /R together.
            if (RemovePath && InsertLocation != int.MaxValue)
            {
                Console.WriteLine("Cannot use Remove and Insert switches together");
                return false;
            }
            // Must declare PathToAdd or there is no point.
            if (string.IsNullOrWhiteSpace(PathToAdd))
            {
                Console.WriteLine("Must declare PathToAdd or there is no point.");
                return false;
            }
            DirectoryInfo di = new DirectoryInfo(PathToAdd);
            PathToAdd = di.FullName;
            return true;
        }

        static void CreateBatchFile(string newEnv)
        {
            try
            {
                File.WriteAllText(BatchFullPath, newEnv, Encoding.ASCII);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception {0} writing file {1} with message: {2}", e.GetType(), BatchFullPath, e.Message);
            }
        }

        static void Usage()
        {
            Console.WriteLine("Usage: AddPathExe pathToAdd [-r] [-s] [-i##]");
            Console.WriteLine("    /R   - Removes pathToAdd from PATH environment variable");
            Console.WriteLine("    /S   - Skips checking if pathToAdd directory exists");
            Console.WriteLine("    /I## - Inserts pathToAdd at the specified location");
            Console.WriteLine();
            Console.WriteLine("The best use is in conjunction with a batch file.");
            Console.WriteLine("Run this utility from the batch file, then execute the {0} file to set the path.", BatchFilename);
        }
    }
}