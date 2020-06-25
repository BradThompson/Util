using System;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace EchoColor
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
                return;
            if (args[0] == "/?")
            {
                Usage();
                return;
            }
            if (args[0].Equals("--help", StringComparison.OrdinalIgnoreCase))
            {
                DashDashHelp();
                return;
            }
            if (args.Length == 1)
            {
                Console.WriteLine("{0}", ProcessForDateTime(args[0]));
                return;
            }
            int trueStart = Environment.CommandLine.IndexOf(Environment.GetCommandLineArgs()[0], StringComparison.OrdinalIgnoreCase) + Environment.GetCommandLineArgs()[0].Length + 2;
            var commandLine = Environment.CommandLine.Substring(trueStart);

            if (args[0].Length != 2)
            {
                Console.WriteLine("{0}", ProcessForDateTime(commandLine));
                return;
            }
            var pat = "[0-9a-fA-F]";
            var foregroundString = args[0][0].ToString();
            var backgroundString = args[0][1].ToString();
            if (!System.Text.RegularExpressions.Regex.IsMatch(foregroundString, pat) ||
                !System.Text.RegularExpressions.Regex.IsMatch(backgroundString, pat))
            {
                Console.WriteLine("{0}", ProcessForDateTime(Environment.CommandLine.Substring(trueStart)));
                return;
            }

            trueStart = Environment.CommandLine.IndexOf(Environment.GetCommandLineArgs()[1], trueStart, StringComparison.OrdinalIgnoreCase) + Environment.GetCommandLineArgs()[1].Length + 1;
            var remainingCommandLine = Environment.CommandLine.Substring(trueStart);
            ConsoleColor foregroundColor = (ConsoleColor)uint.Parse(foregroundString, System.Globalization.NumberStyles.HexNumber);
            ConsoleColor backgroundColor = (ConsoleColor)uint.Parse(backgroundString, System.Globalization.NumberStyles.HexNumber);
            Console.ForegroundColor = foregroundColor;
            Console.BackgroundColor = backgroundColor;
            Console.WriteLine("{0}", ProcessForDateTime(remainingCommandLine));
            Console.ResetColor();
        }

        static string ProcessForDateTime(string commandLine)
        {
            string result = commandLine;
            result = Program.CaseInsenstiveReplace(result, "/datetime", DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss"));
            result = Program.CaseInsenstiveReplace(result, "/ddatetime", DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss"));
            result = Program.CaseInsenstiveReplace(result, "/time", DateTime.Now.ToString("hh:mm:ss"));
            result = Program.CaseInsenstiveReplace(result, "/dtime", DateTime.Now.ToString("hh-mm-ss"));
            result = Program.CaseInsenstiveReplace(result, "/date", DateTime.Now.ToString("MM/dd/yyyy"));
            result = Program.CaseInsenstiveReplace(result, "/ddate", DateTime.Now.ToString("MM-dd-yyyy"));
            return result;
        }

        /// <summary>
        /// A case insenstive replace function. From http://stackoverflow.com/questions/244531/is-there-an-alternative-to-string-replace-that-is-case-insensitive
        /// </summary>
        /// <param name="originalString">The string to examine.(HayStack)</param>
        /// <param name="oldValue">The value to replace.(Needle)</param>
        /// <param name="newValue">The new value to be inserted</param>
        /// <returns>A string</returns>
        public static string CaseInsenstiveReplace(string originalString, string oldValue, string newValue)
        {
            Regex regEx = new Regex(oldValue, RegexOptions.IgnoreCase);
            return regEx.Replace(originalString, newValue);
        }

        static void Usage()
        {
            Console.WriteLine();
            Console.WriteLine("Usage: EchoColor FB String to print to screen");
            Console.WriteLine("    Where FB are two hex digits representing the 15 foreground and background colors");
            Console.WriteLine();
            Console.WriteLine("Example:");
            Console.WriteLine("    EchoColor D9 The plane, the plane!");
            Console.WriteLine();
            Console.WriteLine("This will print: \"The plane, the plane!\" with red letters on a blue background.");
            Console.WriteLine("Spacing is maintained on the output.");
            Console.WriteLine();
            Console.WriteLine("Current foreground is: {0:X} - {1}", (int)Console.ForegroundColor, Console.ForegroundColor);
            Console.WriteLine("Current background is: {0:X} - {1}", (int)Console.BackgroundColor, Console.BackgroundColor);
            Console.WriteLine();
            Console.WriteLine("You can also specify '/date' or '/ddate' or '/time' or '/dtime' or '/datetime' or '/ddatetime'.");
            Console.WriteLine("    /date will be echoed as mm/dd/yyyy  -  /ddate will be echoed as mm-dd-yyyy");
            Console.WriteLine("    /time will be hh:mm:ss  - /dtime will be hh-mm-ss");
            Console.WriteLine("    /dateTime will be in the form MM/dd/yyyy hh:mm:ss");
            Console.WriteLine("    /ddateTime will be in the form MM-dd-yyyy hh-mm-ss");
            Console.WriteLine("    If you embed the parameter, EchoColor will substitute");
            Console.WriteLine("    Example: EchoColor a0 This is the date: /date and this is the time: /time");
            Console.WriteLine("    Displays the string in green with a black background with:");
            Console.WriteLine("        \"This is the date: 03-15-2017 and this is the time: 01:45:02\"");
            Console.WriteLine("    Example 2: EchoColor LogFile /ddate.txt");
            Console.WriteLine("    Displays the string : \"LogFile 03-15-2017.txt\"");
            Console.WriteLine();
            Console.WriteLine("Use '--help' for a full list of available colors.");
        }

        static void DashDashHelp()
        {
            Console.WriteLine("List of all colors");
            string[] names = Enum.GetNames(typeof(ConsoleColor));
            for (int i = 0; i < names.Length; i++)
            {
                Console.WriteLine("{0:X} : {1}", i, names[i]);
            }
            Console.WriteLine();
            Console.WriteLine("Examples of various color combinations:");
            Console.WriteLine();
            DemonstrateSentence("D9");
            DemonstrateSentence("80");
            DemonstrateSentence("0F");
            DemonstrateSentence("F0");
            DemonstrateSentence("E1");
        }

        static void DemonstrateSentence(string colorCombo)
        {
            var foregroundString = colorCombo[0].ToString();
            var backgroundString = colorCombo[1].ToString();
            ConsoleColor foregroundColor = (ConsoleColor)uint.Parse(foregroundString, System.Globalization.NumberStyles.HexNumber);
            ConsoleColor backgroundColor = (ConsoleColor)uint.Parse(backgroundString, System.Globalization.NumberStyles.HexNumber);
            string colors = string.Format("{0}{1}", foregroundString, backgroundString);
            string sentence = string.Format("Using {0}{1}, demonstrate {2,10} foreground and {3,10} background", foregroundString, backgroundString, foregroundColor, backgroundColor);
            Console.ForegroundColor = foregroundColor;
            Console.BackgroundColor = backgroundColor;
            Console.WriteLine("{0}", sentence);
            Console.ResetColor();
        }
    }
}
