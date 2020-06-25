/*
 * Suggested improvements: Wow this code is poorly written. It could do with a re-write.
 * More time can be spent trying to make sure binaries won't get changed.
 * At the very least we could at least find a list of standard binary file
 * types suc as .exe, .dll, jpg, bmp, png, etc and definitely ignore them.
 * Then as a belt and suspenders, try to determine if a file is binary. I've seen several such
 * suggestions on the internet. The IsValidFile method is not good enough.
 * I REALLY want to get rid of the "dangerous" message.
 * We need more tests that actually make changes. We only have one now.
 * 
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace Solstice
{
    public class ReplaceInFiles
    {
        public enum YesorNo
        {
            Yes,
            No
        }
        public static int Main(string[] args)
        {
            bool result;
            int overallResult = 0;

            YesorNo quiet;
            if (!TryGetQuiet(args, out quiet))
            {
                quiet = YesorNo.No;
            }
            if (quiet == YesorNo.No)
            {
                Console.WriteLine("A VERY light weight and DANGEROUS EXE for string search and replace.");
                Console.WriteLine("Don't use it on large files. DON'T USE IT ON BINARY FILES!");
                Console.WriteLine();
            }

            SearchOption searchOption;
            if (!TryGetFileSearchOption(args, out searchOption))
            {
                searchOption = SearchOption.TopDirectoryOnly;
            }
            YesorNo doChange;
            if (!TryGetDoChange(args, out doChange))
            {
                doChange = YesorNo.No;
            }
            string path;
            if (!TryGetPath(args, out path))
            {
                Usage();
                return -1;
            }
            string searchString;
            if (!TryGetSearchString(args, out searchString))
            {
                Usage();
                return -1;
            }
            string replacementString = "";
            bool doReplacement = TryGetReplacementString(args, out replacementString);
            if (!doReplacement && doChange == YesorNo.Yes)
            {
                Usage();
                return -1;
            }
            bool isCaseInsensitive = TryGetParameter(args, "i");
            // First see if the specified file is a file name or a wild carded string.
            try
            {
                if (searchOption == SearchOption.TopDirectoryOnly)
                {
                    FileInfo checkFile = new FileInfo(path);
                    if (checkFile.Exists)
                    {
                        if (doChange == YesorNo.No)
                        {
                            result = FindStringInFile(checkFile, searchString, isCaseInsensitive);
                            if (result)
                            {
                                Console.WriteLine("Found \"{0}\" in {1}", searchString, checkFile.FullName);
                                Console.WriteLine();
                                Console.WriteLine("No files changed.");
                            }
                        }
                        else
                        {
                            result = ReplaceStringInFile(checkFile, searchString, replacementString, isCaseInsensitive);
                        }
                        if (result)
                            return 1;
                        return 0;
                    }
                    else
                    {
                        DirectoryInfo checkDir = new DirectoryInfo(path);
                        if (checkDir.Exists)
                            Console.WriteLine("Please specify a file, not a directory");
                        else
                            Console.WriteLine("Could not find the file \"{0}\"", path);
                        return -1;
                    }
                }
            }
            catch (System.ArgumentException)
            {
            }

            // Do the search on all files, using recursive if necesary
            string root = Path.GetDirectoryName(path);
            string searchPattern = Path.GetFileName(path); // Could include wild cards.
            if (root.Length == 0)
            {
                root = ".";
            }

            var di = new DirectoryInfo(root);
            var files = di.GetFiles(searchPattern, searchOption);
            if (files.Length == 0)
            {
                Console.WriteLine("Could not find file(s) with {0}", path);
                return -1;
            }
            foreach (var f in files)
            {
                if (doChange == YesorNo.No)
                {
                    result = FindStringInFile(f, searchString, isCaseInsensitive);
                    if (result)
                        Console.WriteLine("Found \"{0}\" in {1}", searchString, f.FullName);
                }
                else
                {
                    result = ReplaceStringInFile(f, searchString, replacementString, isCaseInsensitive);
                }
                if (result)
                    overallResult++;
            }
            if (overallResult > 0 && doChange == YesorNo.No)
                Console.WriteLine("No files changed.");

            if (quiet == YesorNo.No)
            {
                Console.WriteLine();
                Console.WriteLine("Done");
            }
            return overallResult;
        }

        enum ByteOrderMark
        {
            UTF8,
            UTF_16_little_endian,       // MS standard for unicode file.
            UTF_16_big_endian,
            UTF_32_little_endian,
            UTF_32_big_endian,
        }

        public static Encoding GetFileEncoding(string srcFile)
        {
            Encoding enc = Encoding.Default;

            byte[] buffer = new byte[5];
            FileStream file = new FileStream(srcFile, FileMode.Open);
            try
            {
                int fileSize = file.Read(buffer, 0, 5);
                if (fileSize < 5)
                    return Encoding.Default;
                if (buffer[0] == 0xEF && buffer[1] == 0xBB && buffer[2] == 0xBF)
                    enc = Encoding.UTF8;
                else if (buffer[0] == 0xFF && buffer[1] == 0xFE)
                    enc = Encoding.Unicode;
                else if (buffer[0] == 0xFE && buffer[1] == 0xFF)
                    enc = Encoding.BigEndianUnicode;
                else if (buffer[0] == 0 && buffer[1] == 0 && buffer[2] == 0xFE && buffer[3] == 0xFF)
                    enc = Encoding.UTF32;
                else if (buffer[0] == 0x2B && buffer[1] == 0x2F && buffer[2] == 0x76)
                    enc = Encoding.UTF7;
            }
            finally
            {
                file.Close();
            }

            return enc;
        }

        public static bool IsValidFile(string srcFile, out string message)
        {
            message = "";
            byte[] buffer = new byte[4096];
            FileStream file = new FileStream(srcFile, FileMode.Open);
            try
            {
                int fileSize = file.Read(buffer, 0, 5);
                if (fileSize < 5)
                    return true;
                if (buffer[0] == 0xFF && buffer[1] == 0xFE && buffer[0] == 0x00 && buffer[1] == 0x00)
                {
                    message = string.Format("The file {0} is determined to be UTF-32 little endian which is not supported.", srcFile);
                    return false;
                }
                if (buffer[0] == 0x4D && buffer[1] == 0x5A)
                {
                    message = string.Format("The file {0} is an executable file. It cannot be changed.", srcFile);
                    return false;
                }
                fileSize = file.Read(buffer, 0, buffer.Length);
                float nonCharacterCount = 0;
                for (int i = 1; i < fileSize; i++)
                {
                    if (buffer[i] < 0x20 || buffer[i] > 127)
                        nonCharacterCount++;
                }
                if (nonCharacterCount / fileSize > .33)
                {
                    message = string.Format("The file {0} is determined to be a binary file.", srcFile);
                    return false;
                }
            }
            finally
            {
                file.Close();
            }
            return true;
        }

        public static bool ReplaceStringInFile(FileInfo fi, string searchString, string replacementString, bool isCaseInsensitive)
        {
            Encoding enc = GetFileEncoding(fi.FullName);
            if (enc == Encoding.Default)
            {
                // This reduces the performance, but this way I can check to see if a file will be changed before I
                // check to see if it would be a valid file.
                if (!FindStringInFile(fi, searchString, isCaseInsensitive))
                    return false;
                string message;
                if (!IsValidFile(fi.FullName, out message))
                {
                    throw new Exception(message);
                }
            }

            string content = File.ReadAllText(fi.FullName);
            string newContent;
            if (isCaseInsensitive)
            {
                newContent = ReplaceString(content, searchString, replacementString, StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                newContent = content.Replace(searchString, replacementString);
            }
            if (content == newContent)
            {
                return false;
            }
            File.WriteAllText(fi.FullName, newContent, enc);
            Console.WriteLine("Replaced \"{0}\" with \"{1}\" in {2}", searchString, replacementString, fi.FullName);
            return true;
        }

        // This comes from https://stackoverflow.com/questions/244531/is-there-an-alternative-to-string-replace-that-is-case-insensitive
        public static string ReplaceString(string str, string oldValue, string newValue, StringComparison comparison)
        {
            StringBuilder sb = new StringBuilder();

            int previousIndex = 0;
            int index = str.IndexOf(oldValue, comparison);
            while (index != -1)
            {
                sb.Append(str.Substring(previousIndex, index - previousIndex));
                sb.Append(newValue);
                index += oldValue.Length;

                previousIndex = index;
                index = str.IndexOf(oldValue, index, comparison);
            }
            sb.Append(str.Substring(previousIndex));

            return sb.ToString();
        }

        /// <summary>
        /// A very inefficient method for searching a file. The entire file must be able
        /// to be loaded into memory.
        /// </summary>
        /// <param name="fi"></param>
        /// <param name="searchString"></param>
        /// <returns></returns>
        public static bool FindStringInFile(FileInfo fi, string searchString, bool isCaseInsensitive)
        {
            if (searchString.Length == 0)
                return false;
            string content = File.ReadAllText(fi.FullName);
            int idx;
            if (isCaseInsensitive)
                idx = content.IndexOf(searchString, StringComparison.OrdinalIgnoreCase);
            else
                idx = content.IndexOf(searchString);
            return (idx >= 0);
        }

        public static bool TryGetFileSearchOption(string[] args, out SearchOption so)
        {
            so = SearchOption.TopDirectoryOnly;
            if (TryGetParameter(args, "x"))
            {
                so = SearchOption.AllDirectories;
                return true;
            }
            return false;
        }

        public static bool TryGetDoChange(string[] args, out YesorNo dc)
        {
            dc = YesorNo.No;
            if (TryGetParameter(args, "go"))
            {
                dc = YesorNo.Yes;
                return true;
            }
            return false;
        }

        public static bool TryGetQuiet(string[] args, out YesorNo quiet)
        {
            quiet = YesorNo.No;
            if (TryGetParameter(args, "q"))
            {
                quiet = YesorNo.Yes;
                return true;
            }
            return false;
        }

        /// <summary>
        /// The search file or path is not optional. If not found the program exits.
        /// The path can be a file, a directory or even wild cards.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool TryGetPath(string[] args, out string path)
        {
            if (!TryGetStringInArgs(args, "f", out path))
                return false;
            path = path.Trim();
            if (path.Length == 0)
                return false;
            return true;
        }

        /// <summary>
        /// The Search string is not optional. If not found the program exits.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="searchString"></param>
        /// <returns></returns>
        public static bool TryGetSearchString(string[] args, out string searchString)
        {
            return TryGetStringInArgs(args, "s", out searchString);
        }

        /// <summary>
        /// The replacement string is optional.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="replacementString"></param>
        /// <returns></returns>
        public static bool TryGetReplacementString(string[] args, out string replacementString)
        {
            return TryGetStringInArgs(args, "r", out replacementString);
        }

        /// <summary>
        /// Given a list of arguments, and a string parameter, returns true/false if it is found.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="parameter"></param>
        /// <param name="isPresent"></param>
        /// <returns></returns>
        public static bool TryGetParameter(string[] args, string parameter)
        {
            foreach (string str in args)
            {
                if (str.Trim().Length - parameter.Length <= 0)   // e.g.: "-r"  or "/GO"
                    continue;
                string flag = str.Substring(0, 1);
                if (flag != "/" && flag != "-")
                    continue;
                string checkStr = str.Trim().ToLower().Substring(1);
                if (checkStr == parameter.ToLower())
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns the specified string from the args list.
        /// The "parameter" is a single character that will be used to create a search string.
        /// For example, "f" will search for /f:, -f:, /F: -F:. The value after the : is returned.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="parameter">A single character</param>
        /// <param name="foundString"></param>
        /// <returns></returns>
        public static bool TryGetStringInArgs(string[] args, string parameter, out string foundString)
        {
            foundString = null;
            string param = parameter.ToLower() + ":";
            foreach (string str in args)
            {
                if (str.Trim().Length - parameter.Length - 2 <= 0)   // e.g.: "-r:"  or "/F:"
                    continue;
                string flag = str.Substring(0, 1);
                if (flag != "/" && flag != "-")
                    continue;
                string checkStr = str.Trim().ToLower().Substring(1, 2);
                if (checkStr == parameter + ":")
                {
                    foundString = str.Substring(3);
                    return true;
                }
            }
            return false;
        }

        public static void Usage()
        {
            var exeName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            Console.WriteLine();
            Console.WriteLine("Finds or Finds and Replaces a string in a given file, a set of files, all files.");
            Console.WriteLine("or all directories recursively.");
            Console.WriteLine("Usage:");
            Console.WriteLine();
            Console.WriteLine("/f:Files - Search file(s). DOS rules for wild cards are used when specifying filenames.");
            Console.WriteLine("/s:TextToFind - Use quotes to use spaces");
            Console.WriteLine("/r:ReplacementText - Use quotes to use spaces");
            Console.WriteLine("/i case insensitive search.");
            Console.WriteLine("/x recurses through all directories.");
            Console.WriteLine("/go actually makes the changes.");
            Console.WriteLine("/q Minimizes screen output.");
            return;
        }
    }
}
