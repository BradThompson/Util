using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace ToExe
{
    class ToExe
    {
        static string AliasFileName = ".toExeNavigation";
        static string ToBatchFilename = "tobat.bat";
        static string AliasFullPath;
        static string BatchFullPath;
        static SortedDictionary<string, string> NavigationList = new SortedDictionary<string, string>();

        static void Main(string[] args)
        {
            try
            {
                Initialize();
                if (args.Length == 1 && args[0].ToUpper() == "/E")
                {
                    ClearAllAliases();
                    return;
                }
                GetAliases();
                if (args.Length == 0)
                {
                    ShowList();
                    return;
                }
                string param1 = args[0].ToUpper();
                if (param1.Length > 1 && param1[0] == '-')
                {
                    param1 = "/" + param1.Substring(1);
                }
                switch (param1)
                {
                    case "/S":
                        if (args.Length != 3)
                        {
                            throw new Exception("/S parameter requires two parameters. See Usage");
                        }
                        if (args[1].IndexOf(' ') >= 0)
                        {
                            throw new Exception("aliases cannot contain spaces");
                        }
                        string aliasAndDir = Environment.CommandLine.Substring(Environment.CommandLine.IndexOf("/S ", StringComparison.OrdinalIgnoreCase) + 3);
                        string directory = aliasAndDir.Substring(aliasAndDir.IndexOf(" ", StringComparison.OrdinalIgnoreCase) + 1);
                        SetAlias(args[1], directory);
                        break;
                    case "/C":
                        if (args.Length != 2)
                        {
                            throw new Exception("/C parameter requires one parameter. See Usage");
                        }
                        ClearAlias(args[1]);
                        break;
                    case "/?":
                        SimpleUsage();
                        break;
                    case "/??":
                        MoreDetails();
                        break;
                    default:
                        if (param1[0] == '/')
                        {
                            throw new Exception("Unsupported switch. See Usage");
                        }
                        CreateChangeDirectoryBatchFile(args[0]);
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine();
                Console.WriteLine(e.Message);
            }
        }

        static void Initialize()
        {
            string userProfile = Environment.GetEnvironmentVariable("USERPROFILE");
            if (string.IsNullOrEmpty(userProfile))
            {
                throw new Exception("Environment variable USERPROFILE was empty.");
            }
            string tempDir = Environment.GetEnvironmentVariable("TEMP");
            if (string.IsNullOrEmpty(tempDir))
            {
                throw new Exception("Environment variable TEMP was empty.");
            }
            AliasFullPath = Path.Combine(userProfile, ToExe.AliasFileName);
            BatchFullPath = Path.Combine(tempDir, ToExe.ToBatchFilename);
        }

        static void CreateChangeDirectoryBatchFile(string alias)
        {
            try
            {
                if (File.Exists(BatchFullPath))
                {
                    File.Delete(BatchFullPath);
                }
            }
            catch (Exception e)
            {
                throw new Exception(String.Format("Exception {0} deleting file {1}", e.GetType(), BatchFullPath));
            }

            string dir = "";
            foreach (var item in NavigationList)
            {
                if (item.Key.Equals(alias, StringComparison.OrdinalIgnoreCase))
                {
                    dir = item.Value;
                }
            }
            if (string.IsNullOrEmpty(dir))
            {
                throw new Exception(String.Format("The alias \"{0}\" was not found", alias));
            }
            try
            {
                File.WriteAllText(BatchFullPath, string.Format("CD /D \"{0}\"", dir));
            }
            catch (Exception e2)
            {
                throw new Exception(String.Format("Exception {0} writing file {1}", e2.GetType(), BatchFullPath));
            }
        }

        static void SetAlias(string alias, string dir)
        {
            char[] trimQuotes = new char[] { '\"', ' ' };
            dir = dir.Trim(trimQuotes);
            DirectoryInfo di = new DirectoryInfo(dir);
            if (!di.Exists)
            {
                throw new Exception(string.Format("Could not find the directory {0}", dir));
            }
            if (NavigationList.ContainsKey(alias))
            {
                NavigationList.Remove(alias);
            }
            NavigationList.Add(alias, di.FullName);
            WriteList();
        }

        static void ClearAlias(string alias)
        {
            if (NavigationList.ContainsKey(alias))
            {
                NavigationList.Remove(alias);
            }
            WriteList();
        }

        static void ClearAllAliases()
        {
            Console.WriteLine();
            Console.WriteLine("Clearing all aliases");
            CreateEmptyNavigationFile();
        }

        static void ShowList()
        {
            Console.WriteLine("TO: Directory aliases:");
            Console.WriteLine();
            if (NavigationList.Count == 0)
            {
                Console.WriteLine("The list is empty");
            }
            foreach (var item in NavigationList)
            {
                Console.WriteLine("{0,16} {1}", item.Key, item.Value);
            }
            Console.WriteLine();
        }

        static void GetAliases()
        {
            string[] aliases = new string[] { };
            try
            {
                aliases = File.ReadAllLines(AliasFullPath);
            }
            catch (System.IO.FileNotFoundException)
            {
                CreateEmptyNavigationFile();
            }
            catch (System.Exception e)
            {
                throw new Exception(string.Format("{0} exception occured trying to read {1}", e.GetType(), AliasFullPath));
            }
            foreach (string line in aliases)
            {
                int aliasIndex = line.IndexOf(' ');
                if (aliasIndex <= 0)
                {
                    throw new Exception(string.Format("Corrupt navigation file {0}", AliasFileName));
                }
                string alias = line.Substring(0, aliasIndex);
                if (NavigationList.ContainsKey(alias))
                {
                    throw new Exception(string.Format("Corrupt navigation list: {0}", AliasFullPath));
                }
                NavigationList.Add(alias, line.Substring(aliasIndex + 1));
            }
        }

        static void CreateEmptyNavigationFile()
        {
            NavigationList.Clear();
            WriteList();
        }

        static void WriteList()
        {
            if (NavigationList.Count == 0)
            {
                try
                {
                    File.WriteAllText(ToExe.AliasFullPath, "");
                }
                catch (System.Exception e)
                {
                    Console.WriteLine(string.Format("{0} exception occured trying to write {1}", e.GetType(), ToExe.AliasFullPath));
                }
                return;
            }
            string[] lines = new string[NavigationList.Count];
            int count = 0;
            foreach (KeyValuePair<string, string> kvp in NavigationList)
            {
                lines[count] = string.Format("{0} {1}", kvp.Key, kvp.Value);
                count++;
            }
            try
            {
                File.WriteAllLines(ToExe.AliasFullPath, lines);
            }
            catch (System.Exception e)
            {
                Console.WriteLine(string.Format("{0} exception occured trying to write {1}", e.GetType(), ToExe.AliasFullPath));
            }
        }

        static void SimpleUsage()
        {
            Console.WriteLine();
            Console.WriteLine("TOEXE Utility v2.0");
            Console.WriteLine();
            Console.WriteLine("SYNTAX:");
            Console.WriteLine("    TO                List alias and directory pairs");
            Console.WriteLine("    TO alias          Change directory to alias");
            Console.WriteLine("    TO /S alias dir   Set new alias for a directory");
            Console.WriteLine("    TO /C alias       Remove alias from list");
            Console.WriteLine("    TO /E             Remove all aliases");
            Console.WriteLine("    TO /?             This message");
            Console.WriteLine("    TO /??            More details");
            Console.WriteLine();
        }

        static void MoreDetails()
        {
            Console.WriteLine();
            Console.WriteLine("TOEXE Utility v2.0");
            Console.WriteLine();
            Console.WriteLine("Alias directory utility.");
            Console.WriteLine("Stores aliases in the file {0}", AliasFullPath);
            Console.WriteLine("Creates a temporary batch file in {0}\\{1}", Environment.GetEnvironmentVariable("TMP"), ToExe.ToBatchFilename);
            Console.WriteLine("The TO.BAT file calls ToExe to create the temp batch file, then calls it.");
            Console.WriteLine();
        }
    }
}