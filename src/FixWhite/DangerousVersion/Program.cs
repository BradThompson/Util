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
        public bool Verbose;
    }

    public class FixupWhiteSpace
    {
        const long MaxSize = 1048576;       // Maximum file size that we can handle
        static StructParameters Parameters;

        public static int Main(string[] args)
        {
            if (!GetParameters(args))
                return 1;
            if (Parameters.Verbose) Console.WriteLine("Checking for unicode");
            if (!CheckUnicode())
                return 1;
            if (Parameters.Verbose) Console.WriteLine("Not unicode");
            ReadAndWriteFiles();
            FixEndOfLine();
            CopyTempToOriginalFile();
            return 0;
        }

        public static void CopyTempToOriginalFile()
        {
            try
            {
                if (Parameters.InPlace)
                {
                    File.Copy(Parameters.FileDestination, Parameters.FileSource, true);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not copy {Parameters.FileDestination} to {Parameters.FileSource}. Exception: {e.Message}");
            }
        }

        public static int ReadAndWriteFiles()
        {
            int charCount = 0;
            byte c;
            string inPlaceTemp = Path.GetTempFileName();
            try
            {
                using (FileStream source = File.Open(Parameters.FileSource, FileMode.Open))
                {
                    using (FileStream dest = File.Create(Parameters.FileDestination))
                    {
                        int i;
                        while ((i = source.ReadByte()) != -1)
                        {
                            c = (byte)i;
                            charCount++;
                            if (Parameters.Verbose)
                            {
                                if (charCount % 1000 == 0)
                                    Console.WriteLine($"Char Count: {charCount}");
                            }
                            // Convert LF into CRLF.
                            if (c == 10)
                            {
                                foreach (byte nl in Environment.NewLine)
                                {
                                    dest.WriteByte(nl);
                                }
                                continue;
                            }
                            if (c == 13)
                            {
                                // Don't write CR. The LF code above takes care of this.
                                continue;
                            }
                            if (c == 9 && Parameters.FixTab)
                            {
                                foreach (byte nl in "    ") // Todo. Should we accept input on how big a tab should be?
                                {
                                    dest.WriteByte(nl);
                                }
                                continue;
                            }
                            dest.WriteByte(c);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
            return 0;
        }

        // If FixEndOfLine is specified, read each line and write each line without white space at the end of the line.
        public static void FixEndOfLine()
        {
            if (!Parameters.FixEndOfLine)
                return;
            string sourceFile = Parameters.FileSource;
            string destinationFile = Parameters.FileDestination;
            if (Parameters.InPlace)
            {
                sourceFile = Parameters.FileDestination;
                destinationFile = Path.GetTempFileName();
            }
            int lineCount = 0;
            try
            {
                using (StreamReader source = new StreamReader(sourceFile))
                {
                    using (StreamWriter dest = new StreamWriter(destinationFile, false))
                    {
                        string line;
                        while (source.Peek() > 0)
                        {
                            line = source.ReadLine();
                            lineCount++;
                            if (Parameters.Verbose)
                            {
                                if (lineCount % 1000 == 0)
                                    Console.WriteLine($"Line Count: {lineCount}");
                            }
                            dest.WriteLine(line.TrimEnd());
                        }
                    }
                }
                if (Parameters.InPlace)
                {
                    Parameters.FileDestination = destinationFile;
                }
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Console.WriteLine("Something is wrong with FixEndOfLine");
                Console.WriteLine(e.Message);
            }
        }
        static public bool CheckUnicode()
        {
            try
            {
                using (System.IO.FileStream file = new System.IO.FileStream(Parameters.FileSource, FileMode.Open))
                {
                    byte[] bom = new byte[4]; // Get the byte-order mark, if there is one 
                    file.Read(bom, 0, 4);
                    if ((bom[0] == 0xff && bom[1] == 0xfe) || // ucs-2le, ucs-4le, and ucs-16le 
                        (bom[0] == 0xfe && bom[1] == 0xff) || // utf-16 and ucs-2 
                        (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff)) // ucs-4 
                    {
                        Console.WriteLine("Unicode is not supported at this time.");
                        return false;
                    }
                }
                // UTF8 files are OK, as long as they don't have unicode characters in them.
                FileInfo fi = new FileInfo(Parameters.FileSource);
                if (fi.Length > MaxSize)
                {
                    Console.WriteLine($"The file {Parameters.FileSource} is to large to process. FixWhite can only handle files less than {MaxSize} characters.");
                    return false;
                }
                // Really inefficient, but hey, the whole program is hack anyway...
                string contents = File.ReadAllText(Parameters.FileSource);
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
            Parameters.Verbose = false;

            // Walk through each argument, assigning the values to the Parameters Structure
            // If the files is a switch, assign, then continue.
            // Otherwise the args are in order. First is source, second (if any) is destination.
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
                        case "V":
                            Parameters.Verbose = true;
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
                Console.WriteLine("When -I is used, do not specify a destination file\r\n");
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
            {
                Parameters.FileDestination = Path.GetTempFileName();
                return true;
            }
            if (File.Exists(Parameters.FileDestination) && !Parameters.OverWrite)
            {
                Console.WriteLine("The destination file {Parameters.FileDestination} exists and -F is not specified.");
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
            Console.WriteLine("    -v : Verbose output.");
            Console.WriteLine();
            Console.WriteLine("Only supports text, not unicode");
        }
    }
}
