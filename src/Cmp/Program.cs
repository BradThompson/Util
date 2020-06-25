using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Cmp
{
    public class Cmp
    {
        const long MaxSize = 1048576;       // Maximum file size that we can handle

        public static int Main(string[] args)
        {
            try
            {
                if (args.Length != 2)
                {
                    Usage();
                    return 2;
                }
                string fileOne = args[0];
                string fileTwo = args[1];
                if (!File.Exists(fileOne))
                {
                    Console.WriteLine($"{fileOne} does not exist.");
                    return 2;
                }
                if (!File.Exists(fileTwo))
                {
                    Console.WriteLine($"{fileTwo} does not exist.");
                    return 2;
                }

                // Read the file into <bits>
                int totalOne = 0;
                int totalTwo = 0;
                var fsOne = new FileStream(fileOne, FileMode.Open, FileAccess.Read);
                var fsTwo = new FileStream(fileTwo, FileMode.Open, FileAccess.Read);
                var kbOne = new byte[1024];
                var kbTwo = new byte[1024];
                int countReadOne = 1;
                int countReadTwo = 1;
                while (countReadOne > 0 && countReadTwo > 0)
                {
                    countReadOne = fsOne.Read(kbOne, 0, 1024);
                    countReadTwo = fsTwo.Read(kbTwo, 0, 1024);
                    if (countReadOne < countReadTwo)
                    {
                        Console.WriteLine($"Files differ in size {fileTwo} is larger than {fileOne}");
                        return 1;
                    }
                    if (countReadOne > countReadTwo)
                    {
                        Console.WriteLine($"Files differ in size {fileOne} is larger than {fileTwo}");
                        return 1;
                    }
                    for (int i = 0; i < countReadOne; ++i)
                    {
                        if (kbOne[i] != kbTwo[i])
                        {
                            Console.WriteLine($"Files differ at byte {totalOne + i}");
                            return 1;
                        }
                    }
                    totalOne += countReadOne;
                    totalTwo += countReadTwo;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception curing Cmp: {e.Message}");
                return 2;
            }
            Console.WriteLine("Files are identical");
            return 0;
        }

        static private void Usage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("Cmp firstFile secondFile");
            Console.WriteLine();
            Console.WriteLine("Compares the files with byte compare");
            Console.WriteLine();
        }
    }
}
