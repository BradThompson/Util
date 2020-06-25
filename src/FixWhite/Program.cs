using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace FixWhite
{
    internal struct StructParameters
    {
        public string FileSource;
        public string FileDestination;
        public bool OverWrite;
        public bool InPlace;
        public bool FixEndOfLine;
        public bool FixTab;
    }

    public class FixupWhiteSpace
    {
        const long MaxSize = 1048576;       // Maximum file size that we can handle
        static StructParameters Parameters;

        public static int Main(string[] args)
        {
            if (!GetParameters(args))
                return 1;
            string destContents = "";
            if (!ReadContents(out string contents))
                return 1;
            for (int i = 0; i < contents.Length; ++i)
            {
                byte b = (byte)contents[i];
                if (b == 10)
                {
                    destContents += Environment.NewLine;
                    continue;
                }
                if (b == 13)
                {
                    if (i < contents.Length - 1 && (byte)contents[i + 1] == 10)
                        continue;    // LF check above handles this case.
                    // Do no harm. If a CR is by itself or even has LF ahead of it, ignore it.
                }
                if (b == 9 && Parameters.FixTab)
                {
                    destContents += "    ";
                    continue;
                }
                destContents += (char)b;
            }
            var addedCRLF = "";
            if (Parameters.FixEndOfLine)
            {
                string[] CRLF = { Environment.NewLine };
                char[] whiteSpace = { ' ', '\t', };
                var lines = destContents.Split(CRLF, StringSplitOptions.None);
                destContents = "";
                for (int i = 0; i < lines.Length; i++)
                {
                    var line = lines[i];
                    var x = line.TrimEnd(whiteSpace);
                    destContents += addedCRLF + x;
                    addedCRLF = Environment.NewLine;
                }
            }
            string dest = null;
            try
            {
                if (!string.IsNullOrEmpty(Parameters.FileDestination))
                {
                    dest = Parameters.FileDestination;
                    File.WriteAllText(dest, destContents);
                }
                else
                {
                    dest = Parameters.FileSource;
                    File.WriteAllText(dest, destContents);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not write to the file {0}. The exception was {1}", dest, e);
                return 1;
            }
            return 0;
        }

        static public bool ReadContents(out string contents)
        {
            contents = null;
            FileInfo fi = new FileInfo(Parameters.FileSource);
            if (fi.Length > MaxSize)
            {
                Console.WriteLine($"The file {Parameters.FileSource} is to large to process. FixWhite can only handle files less than {MaxSize} characters.");
                return false;
            }
            try
            {
                // Really inefficient, but hey, the whole program is hack anyway...
                contents = File.ReadAllText(Parameters.FileSource);
                string test = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(contents));
                if (test != contents)
                {
                    Console.WriteLine("Unicode is not supported at this time.");
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not read the file {Parameters.FileSource}. The exception was {e.Message}");
                return false;
            }
            if (contents.Length == 0)
            {
                Console.WriteLine($"Could not read the file {Parameters.FileSource}. Is the file empty?");
                return false;
            }
            return true;
        }

        static private bool GetParameters(string[] args)
        {
            Parameters = new StructParameters();
            if (args.Length == 0)
            {
                Usage();
                return false;
            }
            Parameters.FixEndOfLine = false;
            Parameters.FixTab = false;
            Parameters.OverWrite = false;
            Parameters.InPlace = false;

            foreach (var a in args)
            {
                string arg = a.Trim();
                if (arg == "" || arg == "?" || arg == "-?" || arg == "/?" || arg.Equals("--help", StringComparison.InvariantCultureIgnoreCase))
                {
                    Usage();
                    return false;
                }
                if (arg.Length == 2 && (arg.Substring(0, 1) == "/" || arg.Substring(0, 1) == "-"))
                {
                    switch (arg.Substring(1, 1).ToUpper())
                    {
                        case "F":
                            Parameters.OverWrite = true;
                            break;
                        case "I":
                            Parameters.InPlace = true;
                            break;
                        case "E":
                            Parameters.FixEndOfLine = true;
                            break;
                        case "T":
                            Parameters.FixTab = true;
                            break;
                        default:
                            Console.WriteLine($"Unknown switch: {a}\r\n");
                            return false;
                    }
                    continue;
                }
                if (Parameters.FileSource != null && Parameters.FileDestination != null)
                {
                    Console.WriteLine("To many files listed\r\n");
                    return false;
                }
                if (Parameters.FileSource != null && Parameters.FileDestination == null)
                {
                    Parameters.FileDestination = arg;
                }
                if (Parameters.FileSource == null)
                {
                    Parameters.FileSource = arg;
                }
            }
            if (string.IsNullOrEmpty(Parameters.FileSource))
            {
                Console.WriteLine("Source file not specified\r\n");
                return false;
            }
            if (!string.IsNullOrEmpty(Parameters.FileDestination) && Parameters.InPlace)
            {
                Console.WriteLine("When -i is used, do not specify a destination file\r\n");
                return false;
            }
            if (string.IsNullOrEmpty(Parameters.FileDestination) && !Parameters.InPlace)
            {
                Console.WriteLine("Destination file was not specified");
                return false;
            }
            if (!File.Exists(Parameters.FileSource))
            {
                Console.WriteLine($"The source file {Parameters.FileSource} does not exist.");
                return false;
            }
            if (Parameters.InPlace)
                return true;
            if (File.Exists(Parameters.FileDestination) && !Parameters.OverWrite)
            {
                Console.WriteLine("The destination file {Parameters.FileDestination} exists and -f is not specified.");
                return false;
            }
            return true;
        }

        static private void Usage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("FixWhite (-f) (-i) sourceFile destinationFile");
            Console.WriteLine("    -f : Force. Overwrite the destination file");
            Console.WriteLine("    -i : Inplace. Update the source file in place");
            Console.WriteLine("    -e : Fix end of line. Removes whitespace at end of line");
            Console.WriteLine("    -t : Fix tab. Converts tabs to 4 spaces. No attempt is made to preserve spacing");
            Console.WriteLine();
            Console.WriteLine("Only supports text, not unicode");
        }
    }
}
