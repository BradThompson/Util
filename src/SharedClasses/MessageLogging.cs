using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace TBTT
{
    // Logs can be located in a Log directory below the EXE.
    //      For example:
    //      R:\ADS\Brad\git\Tools\ObjectCreationScripts\bin\Debug\Log\ObjectCreationScripts.log
    //          or:
    //      R:\ADS\Brad\git\bin\Log\ObjectCreationScripts.log
    // Then can also be stored in the Temp directory:
    //      For example:
    //      C:\Users\bthompson_ADMIN\AppData\Local\Temp\Log\ObjectCreationScripts.log
    // Files can be Append, OvedrWrite, or CreateNew
    //      Append and OverWrite are self explanatory. CreateNew creates a new log file by incrementing the 
    //      number of the log file: MyLogFile1.log, MyLogFile2.log, MyLogFile3.log, MyLogFile4.log
    // The user can request the name of the log file with GetLogFileName()
    // Logging should never interfere with the program. All Exceptions are caught and ignored.

    public static class MessageLogging
    {
        public enum WriteType
        {
            Append = 0,
            OverWrite,
            CreateNew
        }

        private static FileInfo LogFile = null;
        public static bool WriteToConsole = false;

        static MessageLogging()
        {
        }

        public static void WriteLine()
        {
            Write("\r\n");
        }

        public static void WriteLine(string text)
        {
            try
            {
                Write(string.Format("{0}\r\n", text));
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception occured during WriteLine, message logging: " + e.Message);
                if (WriteToConsole)
                    Console.WriteLine("Exception occured during WriteLine, message logging: " + e.Message);
            }
        }

        public static void Write(string text)
        {
            try
            {
                if (LogFile == null)
                {
                    SetLogFileName(WriteType.Append);
                }
                Debug.WriteLine(text);
                if (WriteToConsole)
                    Console.WriteLine(text);
                File.AppendAllText(LogFile.FullName, string.Format("{0}", text));
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception occured Write during message logging: " + e.Message);
                if (WriteToConsole)
                    Console.WriteLine("Exception occured Write during message logging: " + e.Message);
            }
        }

        public static void SetLogFileName(WriteType writeType, bool useTempFolder = false)
        {
            try
            {
                string logFolder = CreateLogFolder(useTempFolder);
                string fileName = string.Format("{0}.log", GetProgramName());
                if (writeType == WriteType.CreateNew)
                {
                    string[] existingLogFiles = Directory.GetFiles(logFolder, string.Format("{0}*.log", GetProgramName()));
                    if (existingLogFiles.Length != 0)
                    {
                        int maxLogFileNumber = 0;
                        foreach (string logFile in existingLogFiles)
                        {
                            string[] sep = new string[1];
                            sep[0] = GetProgramName();
                            string[] logNumbers = logFile.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                            foreach (string workingNumber in logNumbers)
                            {
                                string num = workingNumber.ToUpper().Replace(".LOG", "");
                                if (int.TryParse(num, out int n))
                                {
                                    if (n > maxLogFileNumber)
                                    {
                                        maxLogFileNumber = n;
                                    }
                                }
                            }
                        }
                        fileName = string.Format("{0}{1}.log", GetProgramName(), maxLogFileNumber + 1);
                    }
                }
                LogFile = new FileInfo(Path.Combine(logFolder, fileName));
                Debug.WriteLine(string.Format("Logfile name: {0}", LogFile.FullName));
                string initial = string.Format("\r\nLog start: {0}\r\n----------------------\r\n", DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
                if (writeType == WriteType.OverWrite)
                    File.WriteAllText(LogFile.FullName, initial);
                else
                    File.AppendAllText(LogFile.FullName, initial);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception occured during SetLogFileName: " + e.Message);
                if (WriteToConsole)
                    Console.WriteLine("Exception occured during SetLogFileName: " + e.Message);
            }
        }

        private static string GetProgramName()
        {
            try
            {
                if (Assembly.GetEntryAssembly() == null || Assembly.GetEntryAssembly().GetName() == null || string.IsNullOrWhiteSpace(Assembly.GetEntryAssembly().GetName().Name))
                    return "UnknownProgram";
                return Assembly.GetEntryAssembly().GetName().Name;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception occured during GetProgramName: " + e.Message);
                if (WriteToConsole)
                    Console.WriteLine("Exception occured during GetProgramName: " + e.Message);
                return "UnknownProgram";
            }
        }

        private static string CreateLogFolder(bool useTempFolder)
        {
            try
            {
                string logPath = "";
                if (useTempFolder)
                {
                    logPath = Path.Combine(Path.GetTempPath(), "Log");
                }
                else
                {
                    if (Assembly.GetExecutingAssembly() == null || string.IsNullOrWhiteSpace(Assembly.GetExecutingAssembly().CodeBase))
                    {
                        logPath = Path.Combine(Path.GetTempPath(), "Log");
                    }
                    else
                    {
                        string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                        UriBuilder uri = new UriBuilder(codeBase);
                        string uriPath = Uri.UnescapeDataString(uri.Path);
                        string path = Path.GetDirectoryName(uriPath);
                        logPath = Path.Combine(path, "Log");
                    }
                }
                DirectoryInfo di = new DirectoryInfo(logPath);
                if (!di.Exists)
                {
                    Directory.CreateDirectory(di.FullName);
                }
                return di.FullName;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception occured during CreateLogFolder: " + e.Message);
                if (WriteToConsole)
                    Console.WriteLine("Exception occured during CreateLogFolder: " + e.Message);
                return Path.GetTempPath();
            }
        }

        public static string GetLogFileName()
        {
            try
            {
                if (LogFile == null)
                {
                    SetLogFileName(WriteType.Append);
                }
                return LogFile.FullName;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception occured during GetLogFileName: " + e.Message);
                if (WriteToConsole)
                    Console.WriteLine("Exception occured during GetLogFileName: " + e.Message);
                return Path.Combine(Path.GetTempPath(), "LogFile");
            }
        }
    }
}