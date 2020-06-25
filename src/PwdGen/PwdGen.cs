using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Text.RegularExpressions;

namespace TBTT
{
    internal class MainProgram
    {
        static void Main(string[] args)
        {
            MessageLogging.SetLogFileName(MessageLogging.WriteType.OverWrite, true);
            CommandLine cl = new CommandLine();
            if (!cl.TryProcessCommandLine(args))
                return;
            if (cl.IsUsageRequest)
            {
                Usage();
                return;
            }
            if (cl.Verbose)
            {
                Console.WriteLine($"Verbose: {cl.Verbose}");
                Console.WriteLine($"MinimumLength: {cl.MinimumLength}");
                Console.WriteLine($"MaximumLength: {cl.MaximumLength}");
                Console.WriteLine($"IncludeUppercase: {cl.IncludeUppercase}");
                Console.WriteLine($"IncludeLowercase: {cl.IncludeLowercase}");
                Console.WriteLine($"IncludeNumbers: {cl.IncludeNumbers}");
                Console.WriteLine($"IncludeSymbols: {cl.IncludeSymbols}");
                Console.WriteLine($"IncludeSimpleSymbols: {cl.IncludeSimpleSymbols}");
                Console.WriteLine($"ExcludeAmbiguous: {cl.ExcludeAmbiguous}");
                Console.WriteLine($"{cl.AvailableCharacters.Count}");
                Console.WriteLine($"Seed: {cl.Seed}");
            }
            string pwd = "";
            Random r = new Random(cl.Seed);
            int passwordLength = cl.MinimumLength;
            if (cl.MaximumLength != cl.MinimumLength)
            {
                passwordLength = r.Next(cl.MinimumLength, cl.MaximumLength + 1);
            }
            if (cl.Verbose)
            {
                Console.WriteLine($"passwordLength: {passwordLength}");
            }
            for (int i = 0; i < passwordLength; i++)
            {
                pwd += cl.AvailableCharacters[r.Next(cl.AvailableCharacters.Count)];
            }
            Console.WriteLine(pwd);
        }

        static private void Usage()
        {
            Console.WriteLine("Your usage");
            Console.WriteLine("    /?       - This message");
            Console.WriteLine("    /V       - Verbose output. Can be used to view default values.");
            Console.WriteLine("    /Min     - Minimum number of characters");
            Console.WriteLine("    /Max     - Maximum number of characters");
            Console.WriteLine("    /UC      - Include Upper Case letters");
            Console.WriteLine("    /LC      - Inlude Lower Case letters");
            Console.WriteLine("    /Num     - Include numbers");
            Console.WriteLine("    /Simple  - Include subset of special symbols");
            Console.WriteLine("    /Symbols - Include all special symbols");
            Console.WriteLine("    /ExAmb   - Exclude Ambiguous characters");
        }
    }
}
