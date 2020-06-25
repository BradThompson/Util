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
    public class SwitchDescription
    {
        #region Variables

        public enum SwitchTypeOption
        {
            TrueFalse = 0,
            ValueSwitch
        }

        // SwitchType Indicates if the switch is true/false or if there is a value to be expected as the next item in the command line.
        // Examples: SwitchType = TrueFalse with V as the SwitchName. When the user specifies /V, the SwitchBool will be set to true.
        // Examples: SwitchType = ValueSwitch with F as the SwitchName. When the user specifies /F, the SwitchValue will be set to the next string on the commandline.
        //           If the user enters invalid data such as /F /V or /F at the end of the commandline an error will occur of invalid command line.
        public SwitchTypeOption SwitchType { get; private set; }
        public bool SwitchBool { get; set; }
        public string SwitchValue { get; set; }

        #endregion Variables

        // The SwitchValue can have a default value if it is specified in the constructor.
        // The default of a TrueFalse switch is always false.
        public SwitchDescription(SwitchTypeOption switchType, string switchValue = null)
        {
            SwitchType = switchType;
            SwitchValue = switchValue;
            SwitchBool = false;
        }

    }
}