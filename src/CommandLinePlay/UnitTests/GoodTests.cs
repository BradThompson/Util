using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Dynamic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace CommandLinePlay
{
    [TestClass]
    public class GoodTests
    {
        [TestMethod]
        public void OnlyCreate()
        {
            CommandLineProcessor cl = CommandLineProcessor.Create();
            Assert.AreEqual(0, cl.NamedArgList.Count);
            Assert.AreEqual(0, cl.UnNamedArgList.Count);
        }
        [TestMethod]
        public void CreateWithSwitches()
        {
            CommandLineProcessor cl = CommandLineProcessor.Create(
                    new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.NamedTrueFalse, true, "B1"),
                    new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.NamedTrueFalse, false, "b2"),
                    new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.NamedVariable, true, "Test"),
                    new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.NamedVariable, false, "x"));
            Assert.AreEqual(4, cl.NamedArgList.Count);
            Assert.AreEqual(0, cl.UnNamedArgList.Count);
        }
        [TestMethod]
        public void CreateWithValues()
        {
            CommandLineProcessor cl = CommandLineProcessor.Create(
                    new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.UnNamed, true),
                    new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.UnNamed, false));
            Assert.AreEqual(0, cl.NamedArgList.Count);
            Assert.AreEqual(2, cl.UnNamedArgList.Count);
        }

        [TestMethod]
        public void ProcessRequiredNamed()
        {
            string value = "required";
            CommandLineProcessor cl = CommandLineProcessor.Create(
                    new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.NamedTrueFalse, true, "A"),
                    new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.NamedVariable, true, "C"));
            cl.ProcessCommandLine(GetArgs($"-a -c {value}"));
            Assert.IsTrue(cl.NamedArgList["A"].BoolValue.Value);
            Assert.AreEqual(value, cl.NamedArgList["C"].Value);
        }

        [TestMethod]
        public void ProcessNamedSwitches()
        {
            string reqValue = "required";
            string notReqValue = "not required";
            CommandLineProcessor cl = CommandLineProcessor.Create(
                    new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.NamedTrueFalse, true, "A"),
                    new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.NamedTrueFalse, false, "B"),
                    new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.NamedVariable, true, "C"),
                    new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.NamedVariable, false, "D"));
            cl.ProcessCommandLine(GetArgs($"-a -c {reqValue} /b /D \"{notReqValue}\""));
            Assert.IsTrue(cl.NamedArgList["A"].BoolValue.Value);
            Assert.AreEqual(reqValue, cl.NamedArgList["C"].Value);
            Assert.IsTrue(cl.NamedArgList["B"].BoolValue.Value);
            Assert.AreEqual(notReqValue, cl.NamedArgList["D"].Value);
        }

        [TestMethod]
        public void ProcessWordNamedSwitches()
        {
            string reqValue = "really not required";
            string notReqValue = "You better believe this is required";
            CommandLineProcessor cl = CommandLineProcessor.Create(
                    new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.NamedTrueFalse, true, "Word1"),
                    new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.NamedTrueFalse, false, "Lala"),
                    new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.NamedVariable, true, "TinkerBell"),
                    new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.NamedVariable, false, "IsNice"));
            cl.ProcessCommandLine(GetArgs($"-Word1 /LALA -tinkerbell \"{reqValue}\" /iSnICE \"{notReqValue}\""));
            Assert.IsTrue(cl.NamedArgList["word1"].BoolValue.Value);
            Assert.IsTrue(cl.NamedArgList["lala"].BoolValue.Value);
            Assert.AreEqual(reqValue, cl.NamedArgList["tinkerbell"].Value);
            Assert.AreEqual(notReqValue, cl.NamedArgList["IsNice"].Value);
        }

        [TestMethod]
        public void ProcessNoNames()
        {
            string reqValue = "really not required";
            string notReqValue = "You better believe this is required";
            CommandLineProcessor cl = CommandLineProcessor.Create(
                    new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.UnNamed, true),
                    new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.UnNamed, true));
            cl.ProcessCommandLine(GetArgs($"\"{reqValue}\" \"{notReqValue}\""));
            Assert.AreEqual(reqValue, cl.UnNamedArgList[0].Value);
            Assert.AreEqual(notReqValue, cl.UnNamedArgList[1].Value);
        }

        [TestMethod]
        public void ProcessOneRequiredNoName()
        {
            string reqValue = "You better believe this is required";
            CommandLineProcessor cl = CommandLineProcessor.Create(
                    new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.UnNamed, true),
                    new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.UnNamed, false));
            cl.ProcessCommandLine(GetArgs($"\"{reqValue}\""));
            Assert.AreEqual(reqValue, cl.UnNamedArgList[0].Value);
        }

        [TestMethod]
        public void ProcessTwoRequiredNoName()
        {
            string reqValue1 = "Required 1";
            string reqValue2 = "RequiredTwo";
            string notRequired = "NotRequired";
            CommandLineProcessor cl = CommandLineProcessor.Create(
                    new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.UnNamed, true),
                    new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.UnNamed, true),
                    new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.UnNamed, false));
            cl.ProcessCommandLine(GetArgs($"\"{reqValue1}\" {notRequired} {reqValue2}"));
            Assert.AreEqual(3, cl.UnNamedArgList.Count);
            Assert.AreEqual(reqValue1, cl.UnNamedArgList[0].Value);
            Assert.AreEqual(notRequired, cl.UnNamedArgList[1].Value);
            Assert.AreEqual(reqValue2, cl.UnNamedArgList[2].Value);
        }

        [TestMethod]
        public void ProcessEverything()
        {
            CommandLineProcessor cl = CommandLineProcessor.Create(
                new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.NamedTrueFalse, true, "B1"),
                new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.NamedTrueFalse, false, "b2"),
                new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.NamedVariable, true, "Test"),
                new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.NamedVariable, false, "x"),
                new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.UnNamed, true),
                new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.UnNamed, false));
            cl.ProcessCommandLine(GetArgs($"-b1 /b2 -test Testing -x 1234 MyMomma CalledMe"));
            Assert.IsTrue(cl.NamedArgList["b1"].BoolValue.Value);
            Assert.IsTrue(cl.NamedArgList["b2"].BoolValue.Value);
            Assert.AreEqual("Testing", cl.NamedArgList["test"].Value);
            Assert.AreEqual("1234", cl.NamedArgList["x"].Value);
            Assert.AreEqual("MyMomma", cl.UnNamedArgList[0].Value);
            Assert.AreEqual("CalledMe", cl.UnNamedArgList[1].Value);
        }

        [TestMethod]
        public void ProcessEverythingDifferentOrder()
        {
            CommandLineProcessor cl = CommandLineProcessor.Create(
                new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.NamedTrueFalse, true, "One"),
                new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.NamedTrueFalse, false, "Two"),
                new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.NamedVariable, true, "Three"),
                new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.NamedVariable, false, "Four"),
                new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.UnNamed, true),
                new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.UnNamed, false));
            cl.ProcessCommandLine(GetArgs($"NotRequired Required /Four Test4 /Three Test3 /Two /One"));
            Assert.IsTrue(cl.NamedArgList["One"].BoolValue.Value);
            Assert.IsTrue(cl.NamedArgList["Two"].BoolValue.Value);
            Assert.AreEqual("Test3", cl.NamedArgList["Three"].Value);
            Assert.AreEqual("Test4", cl.NamedArgList["Four"].Value);
            Assert.AreEqual("NotRequired", cl.UnNamedArgList[0].Value);      // Todo fixx. Should check to make sure required parameters are first
            Assert.AreEqual("Required", cl.UnNamedArgList[1].Value);
        }

        [TestMethod]
        public void ProcessUsage()
        {
            CommandLineProcessor cl = CommandLineProcessor.Create();
            Assert.IsFalse(cl.IsUsage);
            cl.ProcessCommandLine(GetArgs($"-?"));
            Assert.IsTrue(cl.IsUsage);

            cl = CommandLineProcessor.Create();
            cl.ProcessCommandLine(GetArgs($"--help"));
            Assert.IsTrue(cl.IsUsage);

            cl = CommandLineProcessor.Create(
                new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.NamedTrueFalse, false, "Over"),
                new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.NamedVariable, false, "Joyed"),
                new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.UnNamed, false));
            cl.ProcessCommandLine(GetArgs($"-Over -Joyed \"To the\" World /?"));
            Assert.IsTrue(cl.IsUsage);
        }

        // Faking the quotes in the command line. DOS normally takes it out before sending to Main.
        public static string[] GetArgs(string cmdLine)
        {
            string[] args = cmdLine.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
            List<string> newArgs = new List<string>();
            StringBuilder sb = new StringBuilder();
            bool building = false;
            for (int i = 0; i < args.Length; i++)
            {
                string item = args[i];
                if (item.Substring(0, 1) == "\"")
                {
                    sb = new StringBuilder();
                    building = true;
                    sb.Append(item.Substring(1));
                    continue;
                }
                if (item.Substring(item.Length - 1, 1) == "\"")
                {
                    sb.Append($" {item.Substring(0, item.Length - 1)}");
                    building = false;
                    newArgs.Add(sb.ToString());
                    continue;
                }
                if (building)
                {
                    sb.Append($" {item}");
                    continue;
                }
                newArgs.Add(item);
            }
            return newArgs.ToArray();
        }
    }
}
