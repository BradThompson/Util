using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FixWhite;
using System.Text;

namespace FixwhiteUnitTests
{
    [TestClass]
    public class FixWhiteTests
    {
        const string DirectoryName = "TestFiles";
        const string ComparisonDirectoryName = "ComparisonFiles";
        const string SourceFile = "SourceFile.txt";
        const string ResultsFileName = "ResultFile.txt";
        static string LineFeed = null;
        static string CarriageReturn = null;
        static string CarriageReturnLineFeed = null;
        static string LineFeedCarriageReturn = null;
        static FileInfo ResultsFile = null;
        static DirectoryInfo TestFileDirectory = null;
        static DirectoryInfo ComparisonDirectory = null;

        [TestInitialize]
        public void TestInit()
        {
            TestFileDirectory = new DirectoryInfo(DirectoryName);
            ComparisonDirectory = new DirectoryInfo(ComparisonDirectoryName);
            ResultsFile = new FileInfo(Path.Combine(TestFileDirectory.FullName, ResultsFileName));
            if (!TestFileDirectory.Exists)
            {
                TestFileDirectory.Create();
            }
            if (File.Exists(ResultsFile.FullName))
            {
                File.Delete(ResultsFile.FullName);
            }
            byte[] byteLFArray = { 10 };
            LineFeed = Encoding.UTF8.GetString(byteLFArray, 0, byteLFArray.Length);
            byte[] byteCRArray = { 13 };
            CarriageReturn = Encoding.UTF8.GetString(byteCRArray, 0, byteCRArray.Length);
            byte[] byteCRLFArray = { 13, 10 };
            CarriageReturnLineFeed = Encoding.UTF8.GetString(byteCRLFArray, 0, byteCRLFArray.Length);
            byte[] byteLFCRArray = { 10, 13 };
            LineFeedCarriageReturn = Encoding.UTF8.GetString(byteLFCRArray, 0, byteLFCRArray.Length);
        }

        [TestMethod]
        public void SimpleUsage()
        {
            Assert.AreEqual(1, CallMain(""));
        }

        [TestMethod]
        public void AsciiNoChange()
        {
            FileInfo fi = CreateFile("AsciiNoChange.txt", $"This is a test:{Environment.NewLine}");
            Assert.AreEqual(0, CallMain("", fi.FullName, ResultsFile.FullName));
            AssertFilesEqual(fi.FullName, ResultsFile.FullName);
        }

        [TestMethod]
        public void TestTabsWithNoChange()
        {
            if (File.Exists(ResultsFile.FullName))
                File.Delete(ResultsFile.FullName);
            // Strings should not change:
            string expected = $"This is a test:\t";
            FileInfo sourceFile = CreateFile(SourceFile, expected);
            int actual = CallMain("", sourceFile.FullName, ResultsFile.FullName);
            Assert.AreEqual(0, actual);
            AssertTextEqual(ResultsFile, expected);

            File.Delete(ResultsFile.FullName);
            expected = $"Tabs\tin\tthe\tmiddle";
            sourceFile = CreateFile(SourceFile, expected);
            actual = CallMain("", sourceFile.FullName, ResultsFile.FullName);
            Assert.AreEqual(0, actual);
            AssertTextEqual(ResultsFile, expected);

            File.Delete(ResultsFile.FullName);
            expected = $"\tTab at the front";
            sourceFile = CreateFile(SourceFile, expected);
            actual = CallMain("", sourceFile.FullName, ResultsFile.FullName);
            Assert.AreEqual(0, actual);
            AssertTextEqual(ResultsFile, expected);
        }

        [TestMethod]
        public void TestTabsWithChange()
        {
            // Strings should not change. Adding -f to test force overwrite, too.
            string inputString = $"This is a test:\t";
            string expected = $"This is a test:    ";
            FileInfo sourceFile = CreateFile(SourceFile, inputString);
            Assert.AreEqual(0, CallMain("-f -t", sourceFile.FullName, ResultsFile.FullName));
            AssertTextEqual(ResultsFile, expected);

            inputString = $"Tabs\tin\tthe\tmiddle";
            expected = $"Tabs    in    the    middle";
            sourceFile = CreateFile(SourceFile, inputString);
            Assert.AreEqual(0, CallMain("/f /t", sourceFile.FullName, ResultsFile.FullName));
            AssertTextEqual(ResultsFile, expected);

            inputString = $"\tTab at the front";
            expected = $"    Tab at the front";
            sourceFile = CreateFile(SourceFile, inputString);
            Assert.AreEqual(0, CallMain("-F -T", sourceFile.FullName, ResultsFile.FullName));
            AssertTextEqual(ResultsFile, expected);

            // One tab
            inputString = $"\t";
            expected = $"    ";
            sourceFile = CreateFile(SourceFile, inputString);
            Assert.AreEqual(0, CallMain("-F -T", sourceFile.FullName, ResultsFile.FullName));
            AssertTextEqual(ResultsFile, expected);

            // Spaces and tabs
            inputString = $"    \t   \t   \t   ";
            expected = $"                         ";
            sourceFile = CreateFile(SourceFile, inputString);
            Assert.AreEqual(0, CallMain("/F /T", sourceFile.FullName, ResultsFile.FullName));
            AssertTextEqual(ResultsFile, expected);
        }

        [TestMethod]
        public void TestEndLineWithNoChange()
        {
            // Strings should not change:
            string expected = $"This is a test";
            FileInfo sourceFile = CreateFile(SourceFile, expected);
            Assert.AreEqual(0, CallMain("-f -e", sourceFile.FullName, ResultsFile.FullName));
            AssertTextEqual(ResultsFile, expected);

            expected = $"Tabs\tin\tthe\tmiddle";
            sourceFile = CreateFile(SourceFile, expected);
            Assert.AreEqual(0, CallMain("-e -f", sourceFile.FullName, ResultsFile.FullName));
            AssertTextEqual(ResultsFile, expected);

            expected = $"\tTab at    the    front";
            sourceFile = CreateFile(SourceFile, expected);
            Assert.AreEqual(0, CallMain("-e -f ", sourceFile.FullName, ResultsFile.FullName));
            AssertTextEqual(ResultsFile, expected);
        }

        [TestMethod]
        public void TestEndLineWithChange()
        {
            // Strings should not change. Adding -f to test force overwrite, too.
            string inputString = $"This is a test:\t";
            string expected = $"This is a test:";
            FileInfo sourceFile = CreateFile(SourceFile, inputString);
            Assert.AreEqual(0, CallMain("-f -e", sourceFile.FullName, ResultsFile.FullName));
            AssertTextEqual(ResultsFile, expected);

            inputString = $"Tabs\tin\tthe\tmiddle    ";
            expected = $"Tabs\tin\tthe\tmiddle";
            sourceFile = CreateFile(SourceFile, inputString);
            Assert.AreEqual(0, CallMain("/f /e", sourceFile.FullName, ResultsFile.FullName));
            AssertTextEqual(ResultsFile, expected);

            inputString = $"    Lot's    of   spaces    ";
            expected = $"    Lot's    of   spaces";
            sourceFile = CreateFile(SourceFile, inputString);
            Assert.AreEqual(0, CallMain("-F -E", sourceFile.FullName, ResultsFile.FullName));
            AssertTextEqual(ResultsFile, expected);

            // One space
            inputString = $" ";
            expected = $"";
            sourceFile = CreateFile(SourceFile, inputString);
            Assert.AreEqual(0, CallMain("-F -E", sourceFile.FullName, ResultsFile.FullName));
            AssertTextEqual(ResultsFile, expected);

            // One tab
            inputString = $"\t";
            expected = $"";
            sourceFile = CreateFile(SourceFile, inputString);
            Assert.AreEqual(0, CallMain("-F -E", sourceFile.FullName, ResultsFile.FullName));
            AssertTextEqual(ResultsFile, expected);

            // Many tabs and spaces
            inputString = $"    \t    \t    \t";
            expected = $"";
            sourceFile = CreateFile(SourceFile, inputString);
            Assert.AreEqual(0, CallMain("/F /E", sourceFile.FullName, ResultsFile.FullName));
            AssertTextEqual(ResultsFile, expected);
        }

        [TestMethod]
        public void TestInPlaceNoChange()
        {
            string inputString = $"\t   This is    a test\t";
            string expected = inputString;
            FileInfo sourceFile = CreateFile(SourceFile, inputString);
            string destFile = $"{sourceFile.FullName}_Comparison";
            File.Copy(sourceFile.FullName, destFile, true);
            // No changes should happen. Files should only change with -T or -E
            int actual = CallMain("-I", sourceFile.FullName);
            Assert.AreEqual(0, actual);
            AssertFilesEqual(sourceFile.FullName, destFile);
        }

        [TestMethod]
        public void TestOverwriteChange()
        {
            string inputString = $"\r\n\t   \nThis \nis    a test\t\r\n";
            string expected = $"\r\n\t   \r\nThis \r\nis    a test\t\r\n";
            FileInfo sourceFile = CreateFile(SourceFile, inputString);
            FileInfo comparisonFile = CreateFile("Comparison " + SourceFile, expected);
            Assert.AreEqual(0, CallMain("-i", sourceFile.FullName));
            AssertFilesEqual(sourceFile.FullName, comparisonFile.FullName);
        }

        [TestMethod]
        public void AsciiYesChange()
        {
            string fileName = "AsciiYesChange.txt";
            FileInfo fi = CreateFile(fileName, $"This is a test:{Environment.NewLine}");
            AppendFile(fi, $"This is a tab: \t");
            AppendFile(fi, $"This is a line feed: {LineFeed}");
            AppendFile(fi, $"This is a carriage return: {CarriageReturn}");
            AppendFile(fi, $"This is a carriage return plus linefeed: {CarriageReturnLineFeed}");
            Assert.AreEqual(0, CallMain("", fi.FullName, ResultsFile.FullName));
            string comparisonFileName = Path.Combine(ComparisonDirectory.FullName, fileName);
            AssertFilesEqual(ResultsFile.FullName, comparisonFileName);
        }

        [TestMethod]
        public void UnicodeYesChange()
        {
            // Create a temporary working file
            string workFile = Path.GetTempFileName();
            FileInfo fi = CreateFile(workFile, $"This is a test:{Environment.NewLine}", Encoding.Unicode);
            AppendFile(fi, $"This is a tab: \t", Encoding.Unicode);
            AppendFile(fi, $"This is a line feed: {LineFeed}", Encoding.Unicode);
            AppendFile(fi, $"This is a carriage return: {CarriageReturn}", Encoding.Unicode);
            AppendFile(fi, $"This is a carriage return plus linefeed: {CarriageReturnLineFeed}", Encoding.Unicode);
            // The resulting file should not be changed in any way, so make a copy.
            File.Copy(fi.FullName, ResultsFile.FullName);
            int actualReturn = CallMain("", fi.FullName, ResultsFile.FullName);
            Assert.AreEqual(1, actualReturn);
            AssertFilesEqual(fi.FullName, ResultsFile.FullName);
        }

        void AssertFilesEqual(string sourceFile, string destFile)
        {
            string source = File.ReadAllText(sourceFile);
            string dest = File.ReadAllText(destFile);
            Assert.AreEqual(source, dest);
        }


        void AssertTextEqual(FileInfo resultsFile, string expected)
        {
            string actual = File.ReadAllText(resultsFile.FullName);
            Assert.AreEqual(expected, actual);
        }

        static FileInfo CreateFile(string testFile, string contents, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.ASCII;
            FileInfo fi = new FileInfo(Path.Combine(TestFileDirectory.FullName, testFile));
            if (fi.Exists)
                fi.Delete();
            Console.WriteLine($"Creating the file: {fi.FullName}");
            Console.WriteLine($"Adding the text: {contents}");
            using (StreamWriter writer = new StreamWriter(fi.FullName, true, encoding))
            {
                writer.Write(contents);
            }
            return fi;
        }

        static void AppendFile(FileInfo testFile, string contents, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.ASCII;
            Console.WriteLine($"Appending the text: {contents}");
            using (StreamWriter writer = new StreamWriter(testFile.FullName, true, encoding))
            {
                writer.Write(contents);
            }
        }

        public int CallMain(string switches, string source = null, string destination = null)
        {
            List<string> cmdLine = new List<string>();
            string[] args = switches.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string a in args)
            {
                cmdLine.Add(a);
            }
            if (source != null)
                cmdLine.Add(source);
            if (destination != null)
                cmdLine.Add(destination);
            return FixWhite.FixupWhiteSpace.Main(cmdLine.ToArray());
        }
    }
}
