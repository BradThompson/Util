// Hmmmmmmmmmmmm I don't like the name, but I want to go to bed.

using System;
using System.Runtime.CompilerServices;

namespace CommandLinePlay
{
    internal class CommandLineObject
    {
        public string Name;
        public DateTime Date;
        public bool IsUsage;
        public CommandLineObject(string[] args)
        {
            try
            {
                CommandLineProcessor clp = CommandLineProcessor.Create(
                    new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.NamedVariable, false, "Name"),
                    new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.NamedVariable, false, "Date"));

                clp.ProcessCommandLine(args);
                IsUsage = clp.IsUsage;
                if (IsUsage)
                    return;
                Name = "";
                if (!string.IsNullOrWhiteSpace(clp.NamedArgList["Name"].Value))
                    Name = clp.NamedArgList["Name"].Value;
                Date = GetDateTime(clp.NamedArgList["Date"].Value);
            }
            catch (BadCodeException bce)
            {
                Console.WriteLine($"Bad code!!! {bce.Message}");
                throw;
            }
            catch (UserInputException uie)
            {
                Console.WriteLine($"{uie.Message}");
                return;
            }
        }

        public override string ToString()
        {
            return ($"Name: {this.Name}, Date: {this.Date.ToString("MM/dd/yyyy hh:mm:ss.fff")}");
        }

        private DateTime GetDateTime(string date)
        {
            if (string.IsNullOrWhiteSpace(date) || !DateTime.TryParse(date, out DateTime processedDate))
                return new DateTime(1900, 1, 1);
            return processedDate;
        }
    }
}
