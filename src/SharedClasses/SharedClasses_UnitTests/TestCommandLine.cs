using EdmondsCommunityCollege;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UnitTests
{
    [TestClass]
    public class TestCommandLine : BaseCommandLineProcessor
    {
        [TestMethod]
        public void SimpleBaseCLPTest()
        {
            TestCommandLine blp = new TestCommandLine();
            string[] cmdLine = { };
            blp.ProcessCommandLine(cmdLine);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void SimpleBaseInvalidTest()
        {
            TestCommandLine blp = new TestCommandLine();
            string[] cmdLine = { };
            blp.ProcessCommandLine(cmdLine);
            Assert.IsTrue(blp.GetSwitchBool("?"));
        }

        [TestMethod]
        public void BaseCLPTest()
        {
            TestCommandLine blp = new TestCommandLine();
            blp.AddSwitch("?", new SwitchDescription(SwitchDescription.SwitchTypeOption.TrueFalse));
            string[] cmdLine = { "-?" };
            blp.ProcessCommandLine(cmdLine);
            Assert.IsTrue(blp.GetSwitchBool("?"));
        }

        [TestMethod]
        public void NotSoAddedSwitches()
        {
            TestCommandLine blp = new TestCommandLine();
            blp.AddSwitch("?", new SwitchDescription(SwitchDescription.SwitchTypeOption.TrueFalse));
            blp.AddSwitch("A", new SwitchDescription(SwitchDescription.SwitchTypeOption.TrueFalse));
            string[] cmdLine = { "-?" };
            blp.ProcessCommandLine(cmdLine);
            Assert.IsFalse(blp.GetSwitchBool("a"));
            Assert.IsTrue(blp.GetSwitchBool("?"));
        }

        [TestMethod]
        public void AddedSwitches()
        {
            TestCommandLine blp = new TestCommandLine();
            blp.AddSwitch("?", new SwitchDescription(SwitchDescription.SwitchTypeOption.TrueFalse));
            blp.AddSwitch("A", new SwitchDescription(SwitchDescription.SwitchTypeOption.TrueFalse));
            string[] cmdLine = { "/a" };
            blp.ProcessCommandLine(cmdLine);
            Assert.IsTrue(blp.GetSwitchBool("a"));
            Assert.IsFalse(blp.GetSwitchBool("?"));
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void BaseBadSwitch()
        {
            TestCommandLine blp = new TestCommandLine();
            blp.AddSwitch("?", new SwitchDescription(SwitchDescription.SwitchTypeOption.TrueFalse));
            blp.AddSwitch("A", new SwitchDescription(SwitchDescription.SwitchTypeOption.TrueFalse));
            string[] cmdLine = { "/bad" };
            blp.ProcessCommandLine(cmdLine);
        }

        [TestMethod]
        public void BaseGetSimpleValue()
        {
            string expected = "TheValue";
            TestCommandLine blp = new TestCommandLine();
            blp.AddSwitch("AValue", new SwitchDescription(SwitchDescription.SwitchTypeOption.ValueSwitch));
            string[] cmdLine = { "-avalue", expected };
            blp.ProcessCommandLine(cmdLine);
            string actual = blp.GetSwitchValue("AVALUE");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void BaseGetDoubleValue()
        {
            string expectedA = "TheValue";
            string expectedB = null;
            TestCommandLine blp = new TestCommandLine();
            blp.AddSwitch("AValue", new SwitchDescription(SwitchDescription.SwitchTypeOption.ValueSwitch));
            blp.AddSwitch("BValue", new SwitchDescription(SwitchDescription.SwitchTypeOption.ValueSwitch));
            string[] cmdLine = { "-avalue", expectedA };
            blp.ProcessCommandLine(cmdLine);
            string actual = blp.GetSwitchValue("AVALUE");
            Assert.AreEqual(expectedA, actual);
            actual = blp.GetSwitchValue("BValue");
            Assert.AreEqual(expectedB, actual);
        }

        [TestMethod]
        public void BaseGetTripleValueWithDefault()
        {
            string server = "TheValue";
            string database = "Decon 1";
            string directory = "Defcon 2";
            TestCommandLine blp = new TestCommandLine();
            blp.AddSwitch("Server", new SwitchDescription(SwitchDescription.SwitchTypeOption.ValueSwitch));
            blp.AddSwitch("Database", new SwitchDescription(SwitchDescription.SwitchTypeOption.ValueSwitch, database));
            blp.AddSwitch("Directory", new SwitchDescription(SwitchDescription.SwitchTypeOption.ValueSwitch, directory));
            string[] cmdLine = { "-server", server };
            blp.ProcessCommandLine(cmdLine);
            string actual = blp.GetSwitchValue("server");
            Assert.AreEqual(server, actual);
            actual = blp.GetSwitchValue("database");
            Assert.AreEqual(database, actual);
            actual = blp.GetSwitchValue("directory");
            Assert.AreEqual(directory, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void BaseGetInvalidWithSwitch()
        {
            TestCommandLine blp = new TestCommandLine();
            blp.AddSwitch("OKGO", new SwitchDescription(SwitchDescription.SwitchTypeOption.ValueSwitch));
            blp.AddSwitch("v", new SwitchDescription(SwitchDescription.SwitchTypeOption.TrueFalse));
            string[] cmdLine = { "-okgo", "/v" };
            blp.ProcessCommandLine(cmdLine);
        }

        [TestMethod]
        public void GetValueOfSwitchNotSpecified()
        {
            TestCommandLine blp = new TestCommandLine();
            blp.AddSwitch("OKGO", new SwitchDescription(SwitchDescription.SwitchTypeOption.ValueSwitch));
            blp.AddSwitch("MoreToTest", new SwitchDescription(SwitchDescription.SwitchTypeOption.ValueSwitch));
            blp.AddSwitch("v", new SwitchDescription(SwitchDescription.SwitchTypeOption.TrueFalse));
            string[] cmdLine = { "-okgo", "test", "/v" };
            blp.ProcessCommandLine(cmdLine);
            string actual = blp.GetSwitchValue("MoreToTest");
            Assert.AreEqual(null, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void BaseGetInvalidWithEOL()
        {
            TestCommandLine blp = new TestCommandLine();
            blp.AddSwitch("OKGO", new SwitchDescription(SwitchDescription.SwitchTypeOption.ValueSwitch));
            string[] cmdLine = { "-okgo" };
            blp.ProcessCommandLine(cmdLine);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void BaseDoubleSwitch()
        {
            TestCommandLine blp = new TestCommandLine();
            blp.AddSwitch("BAD", new SwitchDescription(SwitchDescription.SwitchTypeOption.TrueFalse));
            string[] cmdLine = { "--BAD" };
            blp.ProcessCommandLine(cmdLine);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void BaseDoubleValue()
        {
            TestCommandLine blp = new TestCommandLine();
            blp.AddSwitch("BAD", new SwitchDescription(SwitchDescription.SwitchTypeOption.ValueSwitch));
            string[] cmdLine = { "//bad nodifferance" };
            blp.ProcessCommandLine(cmdLine);
        }

        [TestMethod]
        public void BaseValuesAndSwitchValuesAndTrueFalse()
        {
            string ok = "Nice Band";
            string go = "go";
            string NoSwitch = "NoSwitch";
            bool flag = true;
            TestCommandLine blp = new TestCommandLine();
            blp.AddSwitch("ok", new SwitchDescription(SwitchDescription.SwitchTypeOption.ValueSwitch));
            blp.AddSwitch("go", new SwitchDescription(SwitchDescription.SwitchTypeOption.ValueSwitch, go));
            blp.AddSwitch("flag", new SwitchDescription(SwitchDescription.SwitchTypeOption.TrueFalse));
            string[] cmdLine = { "-ok", "Nice Band", "NoSwitch", "-flag" };
            blp.ProcessCommandLine(cmdLine);
            Assert.AreEqual(ok, blp.GetSwitchValue("ok"));
            Assert.AreEqual(go, blp.GetSwitchValue("go"));
            Assert.AreEqual(flag, blp.GetSwitchBool("flag"));
            Assert.AreEqual(1, blp.NonSwitchValues.Count);
            Assert.AreEqual(NoSwitch, blp.NonSwitchValues[0]);
        }

        [TestMethod]
        public void OddFailureNumberOne()
        {
            string server = "1FP0VF2";
            string database = "DbaAdmin";
            string directory = @"c:\tmp\DbaAdmin";
            bool overWrite = true;
            bool ignore = true;
            TestCommandLine blp = new TestCommandLine();
            blp.AddSwitch("F", new SwitchDescription(SwitchDescription.SwitchTypeOption.TrueFalse));
            blp.AddSwitch("I", new SwitchDescription(SwitchDescription.SwitchTypeOption.TrueFalse));
            string[] cmdLine = { "1FP0VF2", "DbaAdmin", @"c:\tmp\DbaAdmin", "-i", "-f" };
            blp.ProcessCommandLine(cmdLine);
            Assert.AreEqual(3, blp.NonSwitchValues.Count);
            Assert.AreEqual(server, blp.NonSwitchValues[0]);
            Assert.AreEqual(database, blp.NonSwitchValues[1]);
            Assert.AreEqual(directory, blp.NonSwitchValues[2]);
            Assert.AreEqual(overWrite, blp.GetSwitchBool("f"));
            Assert.AreEqual(ignore, blp.GetSwitchBool("i"));
        }
    }
}
