using System;

namespace CommandLinePlay
{
    internal class MainProgram
    {
        static void Main(string[] args)
        {
            try
            {
                CommandLineObject clo = new CommandLineObject(args);
                if (clo.IsUsage)
                {
                    Usage();
                    return;
                }
                Console.WriteLine(clo.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception at Main: {e.Message}, exiting");
                return;
            }
        }

        static private void Usage()
        {
            Console.WriteLine("Your usage");
        }
    }
}
