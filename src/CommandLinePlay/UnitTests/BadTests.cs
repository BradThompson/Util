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
    public class BadTests
    {
        // Only testing a limited number of "bad" values in parameters.
        [TestMethod]
        public void CreateWithSpaceInName()
        {
            try
            {
                CommandLineProcessor cl = CommandLineProcessor.Create(new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.NamedTrueFalse, true, "A A"));
                Assert.Fail("Expected BadCodeException");
            }
            catch (BadCodeException e)
            {
                Assert.AreEqual($"Invalid CommandLine parameter name: A A", e.Message);
            }
        }
        [TestMethod]
        public void CreateWithInvalidChar()
        {
            try
            {
                CommandLineProcessor cl = CommandLineProcessor.Create(new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.NamedTrueFalse, true, "$"));
                Assert.Fail("Expected BadCodeException");
            }
            catch (BadCodeException e)
            {
                Assert.AreEqual($"Invalid CommandLine parameter name: $", e.Message);
            }
        }
        [TestMethod]
        public void CreateWithSecondDash()
        {
            try
            {
                CommandLineProcessor cl = CommandLineProcessor.Create(new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.NamedTrueFalse, true, "-"));
                Assert.Fail("Expected BadCodeException");
            }
            catch (BadCodeException e)
            {
                Assert.AreEqual($"Invalid CommandLine parameter name: -", e.Message);
            }
        }
        [TestMethod]
        public void CreateWithQuestionMark()
        {
            try
            {
                CommandLineProcessor cl = CommandLineProcessor.Create(new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.NamedTrueFalse, true, "?"));
                Assert.Fail("Expected BadCodeException");
            }
            catch (BadCodeException e)
            {
                Assert.AreEqual($"Invalid CommandLine parameter name: ?", e.Message);
            }
        }
        [TestMethod]
        public void CreateWithSwitchTwice()
        {
            try
            {
                CommandLineProcessor cl = CommandLineProcessor.Create(
                        new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.NamedTrueFalse, true, "A"),
                        new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.NamedTrueFalse, false, "A"));
                Assert.Fail("Expected BadCodeException");
            }
            catch (BadCodeException e)
            {
                Assert.AreEqual($"The CommandLine parameter A has already been added", e.Message);
            }
        }
        [TestMethod]
        public void CreateWithRequiredAfterNotRequired()
        {
            try
            {
                CommandLineProcessor cl = CommandLineProcessor.Create(
                        new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.UnNamed, false, "A"),
                        new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.UnNamed, true, "B"));
                Assert.Fail("Expected BadCodeException");
            }
            catch (BadCodeException e)
            {
                Assert.AreEqual($"Declare all required UnNamed parameters before declaring parameters that are not required.", e.Message);
            }
        }
        [TestMethod]
        public void ProcessCommandLineDashByItself()
        {
            try
            {
                CommandLineProcessor cl = CommandLineProcessor.Create(
                        new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.NamedTrueFalse, false, "A"),
                        new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.NamedVariable, false, "B"),
                        new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.UnNamed, false));
                cl.ProcessCommandLine(GoodTests.GetArgs($"-"));
            }
            catch (UserInputException e)
            {
                Assert.AreEqual($"Cannot specify '-' by itself. Please use a switch name", e.Message);
            }
        }
        [TestMethod]
        public void ProcessCommandLineSlashByItself()
        {
            try
            {
                CommandLineProcessor cl = CommandLineProcessor.Create(
                        new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.NamedTrueFalse, false, "A"),
                        new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.NamedVariable, false, "B"),
                        new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.UnNamed, false));
                cl.ProcessCommandLine(GoodTests.GetArgs($"/"));
            }
            catch (UserInputException e)
            {
                Assert.AreEqual($"Cannot specify '/' by itself. Please use a switch name", e.Message);
            }
        }
        [TestMethod]
        public void ProcessDuplicateSwitch()
        {
            try
            {
                CommandLineProcessor cl = CommandLineProcessor.Create(
                        new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.NamedTrueFalse, false, "A"),
                        new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.NamedVariable, false, "B"),
                        new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.UnNamed, false));
                cl.ProcessCommandLine(GoodTests.GetArgs($"-a /A"));
            }
            catch (UserInputException e)
            {
                Assert.AreEqual($"The /A switch has already been declared", e.Message);
            }
        }
        [TestMethod]
        public void ProcessDuplicateSlashSwitch()
        {
            try
            {
                CommandLineProcessor cl = CommandLineProcessor.Create(
                        new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.NamedTrueFalse, false, "A"),
                        new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.NamedVariable, false, "B"),
                        new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.UnNamed, false));
                cl.ProcessCommandLine(GoodTests.GetArgs($"/A -a"));
            }
            catch (UserInputException e)
            {
                Assert.AreEqual($"The -a switch has already been declared", e.Message);
            }
        }
        [TestMethod]
        public void ProcessNoValue()
        {
            try
            {
                CommandLineProcessor cl = CommandLineProcessor.Create(
                        new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.NamedTrueFalse, false, "A"),
                        new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.NamedVariable, false, "B"),
                        new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.UnNamed, false));
                cl.ProcessCommandLine(GoodTests.GetArgs($"/b"));
            }
            catch (UserInputException e)
            {
                Assert.AreEqual($"This switch requires a value to be specified. e.g. /b 'somevalue'", e.Message);
            }
        }
        [TestMethod]
        public void ProcessSwitchNotDefined()
        {
            try
            {
                CommandLineProcessor cl = CommandLineProcessor.Create(
                        new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.NamedTrueFalse, false, "A"),
                        new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.NamedVariable, false, "B"),
                        new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.UnNamed, false));
                cl.ProcessCommandLine(GoodTests.GetArgs($"/X"));
            }
            catch (UserInputException e)
            {
                Assert.AreEqual($"The switch /X is not a valid switch", e.Message);
            }
        }
        [TestMethod]
        public void ProcessToManyUnnamed()
        {
            try
            {
                CommandLineProcessor cl = CommandLineProcessor.Create(
                        new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.UnNamed, false),
                        new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.UnNamed, false),
                        new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.UnNamed, false));
                cl.ProcessCommandLine(GoodTests.GetArgs($"Testing 1 2 3"));
            }
            catch (UserInputException e)
            {
                Assert.AreEqual($"The item \"3\" exceeds the number of allowed values", e.Message);
            }
        }
        [TestMethod]
        public void ProcessToManyRequiredUnnamed()
        {
            try
            {
                CommandLineProcessor cl = CommandLineProcessor.Create(
                        new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.UnNamed, true),
                        new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.UnNamed, true),
                        new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.UnNamed, true));
                cl.ProcessCommandLine(GoodTests.GetArgs($"3 2 1 Testing"));
            }
            catch (UserInputException e)
            {
                Assert.AreEqual($"The item \"Testing\" exceeds the number of allowed values", e.Message);
            }
        }
        [TestMethod]
        public void ProcessToFewRequiredUnnamed()
        {
            try
            {
                CommandLineProcessor cl = CommandLineProcessor.Create(
                        new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.UnNamed, true),
                        new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.UnNamed, true),
                        new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.UnNamed, true));
                cl.ProcessCommandLine(GoodTests.GetArgs($"Black Box"));
            }
            catch (UserInputException e)
            {
                Assert.AreEqual($"There must be 3 parameters without switches", e.Message);
            }
        }
        [TestMethod]
        public void ProcessMissingRequiredSwitches()
        {
            try
            {
                CommandLineProcessor cl = CommandLineProcessor.Create(
                        new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.NamedTrueFalse, true, "A"),
                        new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.NamedVariable, true, "B"),
                        new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.UnNamed, true));
                cl.ProcessCommandLine(GoodTests.GetArgs($"-A \"This looks like part of /A but it isn't\""));
            }
            catch (UserInputException e)
            {
                Assert.AreEqual($"The required value switch -B is not defined", e.Message);
            }
        }
        [TestMethod]
        public void ProcessMissingRequiredNamedVariable()
        {
            try
            {
                CommandLineProcessor cl = CommandLineProcessor.Create(
                        new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.NamedTrueFalse, true, "A"),
                        new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.NamedVariable, true, "B"),
                        new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.UnNamed, true));
                cl.ProcessCommandLine(GoodTests.GetArgs($"UnNamed -B \"This is a part of -B\""));
            }
            catch (UserInputException e)
            {
                Assert.AreEqual($"The required boolean switch -A is not defined", e.Message);
            }
        }
        [TestMethod]
        public void ProcessMissingUnNamed()
        {
            try
            {
                CommandLineProcessor cl = CommandLineProcessor.Create(
                        new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.NamedTrueFalse, true, "A"),
                        new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.NamedTrueFalse, true, "B"),
                        new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.UnNamed, true));
                cl.ProcessCommandLine(GoodTests.GetArgs($"-A -B"));
            }
            catch (UserInputException e)
            {
                Assert.AreEqual($"There must be 1 parameters without switches", e.Message);
            }
        }
        [TestMethod]
        public void ProcessMissingValueUnNamed()
        {
            try
            {
                CommandLineProcessor cl = CommandLineProcessor.Create(
                        new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.NamedVariable, true, "A"),
                        new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.NamedVariable, true, "B"),
                        new ArgumentDescription(ArgumentDescription.ParameterTypeEnum.UnNamed, true));
                cl.ProcessCommandLine(GoodTests.GetArgs($"-A Test -B 123"));
            }
            catch (UserInputException e)
            {
                Assert.AreEqual($"There must be 1 parameters without switches", e.Message);
            }
        }
    }
}
