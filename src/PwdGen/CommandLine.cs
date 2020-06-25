using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TBTT
{
    class CommandLine : BaseCommandLineProcessor
    {
        public const string ShowUsageSwitch = "?";
        public const string VerboseSwitch = "V";
        public const string MinimumLengthSwitch = "Min";
        public const string MaximumLengthSwitch = "Max";
        public const string IncludeUppercaseSwitch = "UC";
        public const string IncludeLowercaseSwitch = "LC";
        public const string IncludeNumbersSwitch = "Num";
        public const string IncludeSimpleSymbolsSwitch = "Simple";
        public const string IncludeSymbolsSwitch = "Symbols";
        public const string ExcludeAmbiguousSwitch = "ExAmb";
        public int Seed;

        public bool IsUsageRequest { get; set; }
        public bool Verbose;
        public int MinimumLength = 8;
        public int MaximumLength = 16;
        public bool IncludeUppercase;
        public bool IncludeLowercase;
        public bool IncludeNumbers;
        public bool IncludeSimpleSymbols;
        public bool IncludeSymbols;
        public bool ExcludeAmbiguous;

        public List<string> AvailableCharacters { get; private set; }
        string Uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        string Lowercase = "abcdefghijklmnopqrstuvwxyz";
        string Numbers = "0123456789";
        string SimpleSymbols = "!$%*_-=?";
        string Symbols = "~!@#$%^*()_=[]{};',.?";
        string Ambiguous = "il1Lo0O@#|$";

        public CommandLine() : base()
        {
            Switches.Add(ShowUsageSwitch.ToUpper(), new SwitchDescription(SwitchDescription.SwitchTypeOption.TrueFalse));
            Switches.Add(VerboseSwitch.ToUpper(), new SwitchDescription(SwitchDescription.SwitchTypeOption.TrueFalse));
            Switches.Add(MinimumLengthSwitch.ToUpper(), new SwitchDescription(SwitchDescription.SwitchTypeOption.ValueSwitch));
            Switches.Add(MaximumLengthSwitch.ToUpper(), new SwitchDescription(SwitchDescription.SwitchTypeOption.ValueSwitch));
            Switches.Add(IncludeUppercaseSwitch.ToUpper(), new SwitchDescription(SwitchDescription.SwitchTypeOption.TrueFalse));
            Switches.Add(IncludeLowercaseSwitch.ToUpper(), new SwitchDescription(SwitchDescription.SwitchTypeOption.TrueFalse));
            Switches.Add(IncludeNumbersSwitch.ToUpper(), new SwitchDescription(SwitchDescription.SwitchTypeOption.TrueFalse));
            Switches.Add(IncludeSymbolsSwitch.ToUpper(), new SwitchDescription(SwitchDescription.SwitchTypeOption.TrueFalse));
            Switches.Add(IncludeSimpleSymbolsSwitch.ToUpper(), new SwitchDescription(SwitchDescription.SwitchTypeOption.TrueFalse));
            Switches.Add(ExcludeAmbiguousSwitch.ToUpper(), new SwitchDescription(SwitchDescription.SwitchTypeOption.TrueFalse));
            AvailableCharacters = new List<string>();
        }

        public bool TryProcessCommandLine(string[] args)
        {
            try
            {
            base.ProcessCommandLine(args);
            // This can be done better. We should create an object that contains the string switch and the get/set function.
            IsUsageRequest = this.GetSwitchBool(ShowUsageSwitch);
            if (IsUsageRequest)
                return true;
            Verbose = this.GetSwitchBool(VerboseSwitch);
            if (args.Length <= 0 || (args.Length == 1 && Verbose))
            {
                LoadVariablesFromRegistry();
            }
            else
            {
                IncludeUppercase = this.GetSwitchBool(IncludeUppercaseSwitch);
                IncludeLowercase = this.GetSwitchBool(IncludeLowercaseSwitch);
                IncludeNumbers = this.GetSwitchBool(IncludeNumbersSwitch);
                IncludeSymbols = this.GetSwitchBool(IncludeSymbolsSwitch);
                IncludeSimpleSymbols = this.GetSwitchBool(IncludeSimpleSymbolsSwitch);
                ExcludeAmbiguous = this.GetSwitchBool(ExcludeAmbiguousSwitch);
            }
            if (IncludeSimpleSymbols && IncludeSymbols)
            {
                Console.WriteLine("Cannot specify both /Simple and /Symbols");
                return false;
            }
            if (!string.IsNullOrWhiteSpace(this.GetSwitchValue(MinimumLengthSwitch)))
            {
                if (!int.TryParse(this.GetSwitchValue(MinimumLengthSwitch), out int minLen))
                {
                    Console.WriteLine("/Min switch must be an integer");
                    return false;
                }
                if (minLen < 8 || minLen > 32)
                {
                    Console.WriteLine("/Min must be between 8 and 32");
                    return false;
                }
                MinimumLength = minLen;
            }
            if (!string.IsNullOrWhiteSpace(this.GetSwitchValue(MaximumLengthSwitch)))
            {
                if (!int.TryParse(this.GetSwitchValue(MaximumLengthSwitch), out int maxLen))
                {
                    Console.WriteLine("/max switch must be an integer");
                    return false;
                }
                if (maxLen < MinimumLength || maxLen > 32)
                {
                    Console.WriteLine($"/Max must be greater then /Min {MinimumLength} and less then 32");
                    return false;
                }
                MaximumLength = maxLen;
            }
            if (!IncludeUppercase && !IncludeLowercase && !IncludeNumbers && !IncludeSymbols && !ExcludeAmbiguous && !IncludeSimpleSymbols)
            {
                Console.WriteLine("Must specify at least one of the include switches");
                IsUsageRequest = true;
                return true;
            }
            if (IncludeUppercase)
            {
                foreach (var uc in Uppercase.ToCharArray())
                {
                    AvailableCharacters.Add(uc.ToString());
                }
            }
            if (IncludeLowercase)
            {
                foreach (var lc in Lowercase.ToCharArray())
                {
                    AvailableCharacters.Add(lc.ToString());
                }
            }
            if (IncludeNumbers)
            {
                foreach (var n in Numbers.ToCharArray())
                {
                    AvailableCharacters.Add(n.ToString());
                }
            }
            if (IncludeSymbols)
            {
                foreach (var s in Symbols.ToCharArray())
                {
                    AvailableCharacters.Add(s.ToString());
                }
            }
            if (IncludeSimpleSymbols)
            {
                foreach (var s in SimpleSymbols.ToCharArray())
                {
                    AvailableCharacters.Add(s.ToString());
                }
            }
            if (ExcludeAmbiguous)
            {
                foreach (var e in Ambiguous.ToCharArray())
                {
                    if (AvailableCharacters.Contains(e.ToString()))
                        AvailableCharacters.Remove(e.ToString());
                }
            }
            // Create a truely random seed.
            string working = DateTime.Now.Ticks.ToString();
            string reverseWorking = "";
            for (var i = working.Length - 1; i > working.Length - 10; i--)
            {
                reverseWorking += working[i];
            }
            Seed = int.Parse(reverseWorking);
            SaveVariablesToRegistry();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception during TryProcessCommandLine: {e.Message}");
                return false;
            }
            return true;
        }

        public void LoadVariablesFromRegistry()
        {
            Registry r = new Registry("PwdGen");
            IncludeUppercase = ConvertStringToBoolean(r.GetValue(IncludeUppercaseSwitch, "False"));
            IncludeLowercase = ConvertStringToBoolean(r.GetValue(IncludeLowercaseSwitch, "False"));
            IncludeNumbers = ConvertStringToBoolean(r.GetValue(IncludeNumbersSwitch, "False"));
            IncludeSymbols = ConvertStringToBoolean(r.GetValue(IncludeSymbolsSwitch, "False"));
            IncludeSimpleSymbols = ConvertStringToBoolean(r.GetValue(IncludeSimpleSymbolsSwitch, "False"));
            ExcludeAmbiguous = ConvertStringToBoolean(r.GetValue(ExcludeAmbiguousSwitch, "False"));
            MinimumLength = int.Parse(r.GetValue(MinimumLengthSwitch, "8"));
            MaximumLength = int.Parse(r.GetValue(MaximumLengthSwitch, "16"));
        }

        public void SaveVariablesToRegistry()
        {
            Registry r = new Registry("PwdGen");
            r.SetValue(IncludeUppercaseSwitch, IncludeUppercase.ToString());
            r.SetValue(IncludeLowercaseSwitch, IncludeLowercase.ToString());
            r.SetValue(IncludeNumbersSwitch, IncludeNumbers.ToString());
            r.SetValue(IncludeSymbolsSwitch, IncludeSymbols.ToString());
            r.SetValue(IncludeSimpleSymbolsSwitch, IncludeSimpleSymbols.ToString());
            r.SetValue(ExcludeAmbiguousSwitch, ExcludeAmbiguous.ToString());
            r.SetValue(MinimumLengthSwitch, MinimumLength.ToString());
            r.SetValue(MaximumLengthSwitch, MaximumLength.ToString());
        }

        private bool ConvertStringToBoolean(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return false;
            if (str.ToLower() == "true")
                return true;
            if (str.ToLower() == "false")
                return false;
            if (!int.TryParse(str, out int result))
                return false;
            if (result == 0)
                return false;
            return true;
        }
    }
}
