using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace CCDExe
{
    class CCDExe
    {
        static string BatchFilename = "ccdbat.bat";
        static string BatchFullPath;

        static void Main(string[] args)
        {
            string tempDir = Environment.GetEnvironmentVariable("TEMP");
            if (string.IsNullOrEmpty(tempDir))
            {
#if DEBUG
                Console.WriteLine("Environment variable TEMP was empty.");
#endif
            }
            BatchFullPath = Path.Combine(tempDir, BatchFilename);
            if (args.Length == 0)
            {
                CreateChangeDirectoryBatchFile("CD");
                return;
            }
            if (args.Length == 1 && args[0].ToUpper() == "/D")
            {
                CreateChangeDirectoryBatchFile("CD /D");
                return;
            }
            string commandLine = "";
            foreach (string segment in args)
            {
                if (segment.ToUpper() == "/D")
                    continue;
                commandLine += segment + " ";
            }
            commandLine = commandLine.Replace('/', '\\').Trim();
            if (CheckForDirectory(commandLine))
                return;
            if (CheckForFile(commandLine))
                return;
            string[] parts = commandLine.Split('\\');
            if (parts.Length > 1)
            {
                int index = commandLine.LastIndexOf('\\');
                if (CheckForDirectory(commandLine.Substring(0, index)))
                    return;
            }

            // If all else fails, just pass the command line 'as is' to CD and let it handle error messaging.
            string allElse = Environment.CommandLine.Substring(Environment.GetCommandLineArgs()[0].Length + 2).Trim();
            CreateChangeDirectoryBatchFile("CD " + allElse);
            return;
        }

        static Boolean CheckForDirectory(string directoryPath)
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(directoryPath);
                if (di.Exists)
                {
                    CreateChangeDirectoryBatchFile(string.Format("CD /D {0}", di.FullName));
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
#if DEBUG
                Console.WriteLine("Exception {0} in CheckForDirectory with message: {1}", e.GetType(), e.Message);
#endif
                CreateChangeDirectoryBatchFile(string.Format("CD {0}", directoryPath));
                return true;
            }
        }

        static Boolean CheckForFile(string filePath)
        {
            try {
                FileInfo fi = new FileInfo(filePath);
                if (fi.Exists)
                {
                    CreateChangeDirectoryBatchFile(string.Format("CD /D {0}", fi.DirectoryName));
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
#if DEBUG
                Console.WriteLine("Exception {0} in CheckForFile with message: {1}", e.GetType(), e.Message);
#endif
                CreateChangeDirectoryBatchFile(string.Format("CD {0}", filePath));
                return true;
            }
            return false;
        }

        static void CreateChangeDirectoryBatchFile(string cdString)
        {
            try
            {
                File.WriteAllText(BatchFullPath, cdString);
            }
            catch (Exception e)
            {
#if DEBUG
                Console.WriteLine("Exception {0} writing file {1} with message: {2}", e.GetType(), BatchFullPath, e.Message);
#endif
            }
        }
    }
}