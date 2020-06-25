using System;
using System.IO;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace TBTT
{
    public class Registry
    {
        const string TBTTRegistry = @"Software\TBTT";
        public string ProgramKey { get; private set; }

        public Registry(string programKey)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(programKey), "Must define programKey");
            Debug.Assert(!programKey.Contains(@"\"), "Must not define slashes");
            ProgramKey = Path.Combine(TBTTRegistry, programKey);
        }

        private void Initialize(bool forWrite)
        {
            try
            {
                using (RegistryKey hklm = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default))
                {
                    if (forWrite)
                    {
                        hklm.CreateSubKey(ProgramKey, forWrite);
                    }
                    else
                    {
                        hklm.OpenSubKey(ProgramKey, false);
                    }
                }
            }
            catch (ArgumentNullException)
            {
                Console.WriteLine("Program Error");
                throw;
            }
            catch (System.Security.SecurityException)
            {
                Console.WriteLine($"Security failure with {ProgramKey}");
                throw;
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine($"Access is denied to {ProgramKey}");
                throw;
            }
        }


        // For this iteration we only handle strings.
        public string GetValue(string keyName, string defaultValue)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(keyName));

            using (RegistryKey hklm = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default))
            {
                using (RegistryKey registryKey = hklm.OpenSubKey(ProgramKey, false))
                {
                    if (registryKey == null)
                        return defaultValue;
                    return (string)registryKey.GetValue(keyName, defaultValue);
                }
            }
        }

        public void SetValue(string keyName, string value)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(keyName));

            using (RegistryKey hklm = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default))
            {
                using (RegistryKey registryKey = hklm.CreateSubKey(ProgramKey, true))
                {
                    if (registryKey == null)
                        Console.WriteLine("Hmmmm, I wonder why it was not able to set the value without an exception?");
                    registryKey.SetValue(keyName, value, RegistryValueKind.String);
                }
            }
        }
    }

    /*
    // Doing it the old way.
    string Testing1 = "Test 5";
    string Testing2 = "Test 6";
    string Testing3 = "Test 7";
    bool Testing4 = true;

    RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default);
    key = key.OpenSubKey(subkey);
    if (key != null)
    {
        Testing1 = (string)key.GetValue("StatusHtmlFile", Testing1);
        Conole.WriteLine("Testing1: {0}", Testing1);
        Testing2 = (string)key.GetValue("StatusXslFile", Testing2);
        Console.WriteLine("Testing2: {0}", Testing2);
        Testing3 = (string)key.GetValue("StatusXmlFile", Testing3);
        Console.WriteLine("Testing3: {0}", Testing3);
        Testing4 = Convert.ToBoolean(key.GetValue("DoTransform", Testing4 ? "True" : "False"));
        Console.WriteLine("Testing4: {0}", Testing4);
    }

    RegistryKey root;
    RegistryKey rk;
    root = Registry.CurrentUser;
    rk = root.CreateSubKey(subkey);
    rk.SetValue("StatusHtmlFile", Testing1);
    rk.SetValue("StatusXslFile", Testing2);
    rk.SetValue("StatusXmlFile", Testing3);
    rk.SetValue("DoTransform", Testing4);
    rk.Close();
}
*/
    //-     public static void Main1()
    //-     {
    //-         const string subkey = @"HKEY_CURRENT_USER\Software\Microsoft\Webstore";
    //- 
    //-         Microsoft.Win32.Registry.SetValue(subkey, "Testing", @"C:\MyFile.htm", RegistryValueKind.String);
    //-         string s = (string) Registry.GetValue(subkey, "Testings", "Nope");
    //-         Console.WriteLine("s: {0}", s);
    //-         return;
    //-     }
    //- 
    //-     public static void Main2()
    //-     {
    //-         // The name of the key must include a valid root.
    //-         const string subkey = @"HKEY_LOCAL_MACHINE\Software\Microsoft\Webstore";
    //- 
    //-         Registry.SetValue(subkey, "WmoDefaultSqlCommandTimeout", 120, RegistryValueKind.DWord);
    //-         Registry.SetValue(subkey, "WmoDefaultTrnLogCmdTimeout", 3600, RegistryValueKind.DWord);
    //-         Registry.SetValue(subkey, "WmoDefaultBackupRestoreSqlCmdTimeout", 43200, RegistryValueKind.DWord);
    //-         int i = (int)Registry.GetValue(subkey, "WmoDefaultSqlCommandTimeoutX", 0);
    //-         Console.WriteLine("i: {0}", i);
    //-         return;
    //-     }
    //- 
    //-     public static void Main3()
    //-     {
    //-         // An int value can be stored without specifying the
    //-         // registry data type, but long values will be stored
    //-         // as strings unless you specify the type. Note that
    //-         // the int is stored in the default name/value
    //-         // pair.
    //- 
    //-         const string keyName = @"HKEY_CURRENT_USER\Software\Microsoft\Webstore";
    //- 
    //-         Registry.SetValue(keyName, "", 5280);
    //-         Registry.SetValue(keyName, "TestLong", 12345678901234, RegistryValueKind.QWord);
    //- 
    //-         // Strings with expandable environment variables are
    //-         // stored as ordinary strings unless you specify the
    //-         // data type.
    //-         Registry.SetValue(keyName, "TestExpand", "My path: %path%");
    //-         Registry.SetValue(keyName, "TestExpand2", "My path: %path%", RegistryValueKind.ExpandString);
    //- 
    //-         // Arrays of strings are stored automatically as 
    //-         // MultiString. Similarly, arrays of Byte are stored
    //-         // automatically as Binary.
    //-         string[] strings = {"One", "Two", "Three"};
    //-         Registry.SetValue(keyName, "TestArray", strings);
    //- 
    //-         // Your default value is returned if the name/value pair
    //-         // does not exist.
    //-         string noSuch = (string) Registry.GetValue(keyName, "NoSuchName", "Return this default if NoSuchName does not exist.");
    //-         Console.WriteLine("\r\nNoSuchName: {0}", noSuch);
    //- 
    //-         // Retrieve the int and long values, specifying 
    //-         // numeric default values in case the name/value pairs
    //-         // do not exist. The int value is retrieved from the
    //-         // default (nameless) name/value pair for the key.
    //-         int tInteger = (int) Registry.GetValue(keyName, "", -1);
    //-         Console.WriteLine("(Default): {0}", tInteger);
    //-         long tLong = (long) Registry.GetValue(keyName, "TestLong", long.MinValue);
    //-         Console.WriteLine("TestLong: {0}", tLong);
    //- 
    //-         // When retrieving a MultiString value, you can specify
    //-         // an array for the default return value. 
    //-         string[] tArray = (string[]) Registry.GetValue(keyName, "TestArray", new string[] {"Default if TestArray does not exist."});
    //-         for(int i=0; i<tArray.Length; i++)
    //-         {
    //-             Console.WriteLine("TestArray({0}): {1}", i, tArray[i]);
    //-         }
    //- 
    //-         // A string with embedded environment variables is not
    //-         // expanded if it was stored as an ordinary string.
    //-         string tExpand = (string) Registry.GetValue(keyName, "TestExpand", "Default if TestExpand does not exist.");
    //-         Console.WriteLine("TestExpand: {0}", tExpand);
    //- 
    //-         // A string stored as ExpandString is expanded.
    //-         string tExpand2 = (string) Registry.GetValue(keyName, "TestExpand2", "Default if TestExpand2 does not exist.");
    //-         Console.WriteLine("TestExpand2: {0}...", tExpand2.Substring(0, 40));
    //- 
    //-         Console.WriteLine("\r\nUse the registry editor to examine the key.");
    //-         Console.WriteLine("Press the Enter key to delete the key.");
    //-         Console.ReadLine();
    //-         Registry.CurrentUser.DeleteSubKey(keyName);
    //-     }
}

//
// This code example produces output similar to the following:
//
//NoSuchName: Return this default if NoSuchName does not exist.
//(Default): 5280
//TestLong: 12345678901234
//TestArray(0): One
//TestArray(1): Two
//TestArray(2): Three
//TestExpand: My path: %path%
//TestExpand2: My path: D:\Program Files\Microsoft.NET\...
//
//Use the registry editor to examine the key.
//Press the Enter key to delete the key.
