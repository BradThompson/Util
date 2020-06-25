using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;

namespace TBTT
{
    public class BaseCommandLineProcessor
    {
        #region Variables

        public Dictionary<string, SwitchDescription> Switches { get; private set; }
        public List<string> NonSwitchValues;

        #endregion

        public BaseCommandLineProcessor()
        {
            Switches = new Dictionary<string, SwitchDescription>();
            NonSwitchValues = new List<string>();
        }

        public void AddSwitch(string switchName, SwitchDescription description)
        {
            Switches.Add(switchName.ToUpper(), description);
        }

        public string GetSwitchValue(string switchName)
        {
            switchName = GetSwitchCommon(switchName, SwitchDescription.SwitchTypeOption.ValueSwitch);
            if (!Switches.ContainsKey(switchName))
                throw new Exception("Unknown switch");
            return Switches[switchName].SwitchValue;
        }

        public bool GetSwitchBool(string switchName)
        {
            switchName = GetSwitchCommon(switchName, SwitchDescription.SwitchTypeOption.TrueFalse);
            if (!Switches.ContainsKey(switchName))
                throw new Exception("Unknown switch");
            return Switches[switchName].SwitchBool;
        }

        private string GetSwitchCommon(string switchName, SwitchDescription.SwitchTypeOption option)
        {
            switchName = switchName.Trim().ToUpper();
            if (!Switches.ContainsKey(switchName))
            {
                throw new Exception("Unknown key. Please add this key to the command line processor Switches list");
            }
            if (Switches[switchName].SwitchType != option)
                throw new Exception(string.Format("Switch is a {0} switch. Invalid request", option.ToString()));
            return switchName;
        }

        // See the usage for the available parameters. This will process the command line looking for switches.
        // The switch is denoted with a dash or forward slash. Case is not sensitive. For example: -f is equivalent to /F
        // If the switch is unrecognised this returns false.
        protected void ProcessCommandLine(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                string item = args[i];
                if (item.Length > 1 && item.Substring(0, 1) == "/" || item.Substring(0, 1) == "-")
                {
                    string switchKey = item.Substring(1).ToUpper();
                    if (Switches.ContainsKey(switchKey))
                    {
                        SwitchDescription foundSwitch = Switches[switchKey];
                        if (foundSwitch.SwitchType == SwitchDescription.SwitchTypeOption.TrueFalse && foundSwitch.SwitchBool)
                        {
                            throw new Exception(string.Format("{0} is specified twice on the command line. This is invalid", item));
                        }
                        if (foundSwitch.SwitchType == SwitchDescription.SwitchTypeOption.TrueFalse)
                        {
                            Switches[switchKey].SwitchBool = true;
                        }
                        else
                        {
                            if (i + 1 == args.Length || args[i + 1].Substring(0, 1) == "/" || args[i + 1].Substring(0, 1) == "-")
                            {
                                throw new Exception(string.Format("{0} requires a value after the switch.", item));
                            }
                            Switches[switchKey].SwitchValue = args[i + 1];
                            i++;
                        }
                    }
                    else
                    {
                        throw new Exception(string.Format("The switch {0} is invalid", item));
                    }
                }
                else
                {
                    NonSwitchValues.Add(item);
                }
            }
        }
    }
}
