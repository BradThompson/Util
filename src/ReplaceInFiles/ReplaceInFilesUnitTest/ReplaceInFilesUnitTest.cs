using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solstice;

namespace ReplaceInFilesUnitTest
{
    /// <summary>
    /// Summary description for ReplaceInFilesUnitTest
    /// </summary>
    [TestClass]
    public class ReplaceInFilesUnitTest
    {
        #region Setup / takedown

        public const string testOut = "TestOut";
        public const string failureTestFiles = "FailureReference";

        static string savedCurrentDirectory;

        #region Additional test attributes

        [ClassInitialize]
        public static void MyClassInitialize(TestContext ct)
        {
            savedCurrentDirectory = Directory.GetCurrentDirectory();
        }

        // Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void MyTestInitialize()
        {
            Directory.SetCurrentDirectory(savedCurrentDirectory);
            //\\ C: \util\src\ReplaceInFiles\ReplaceInFilesUnitTest\Test\SavedFiles\repeat.txt
            File.Copy(@"Test\SavedFiles\repeat.txt", @"Test\ActualFiles\repeat.txt", true);
        }

        #endregion

        #endregion
        #region Tests

        [TestMethod]
        public void TestUsage()
        {
            Solstice.ReplaceInFiles.Usage();
        }

        [TestMethod]
        public void GetSearchOptionFalse()
        {
            string[] args = new string[3];
            SearchOption so;
            args[0] = "X"; args[1] = "Y"; args[2] = "Z";
            Assert.IsFalse(Solstice.ReplaceInFiles.TryGetFileSearchOption(args, out so));
            args[0] = "/rick";
            Assert.IsFalse(Solstice.ReplaceInFiles.TryGetFileSearchOption(args, out so));
            args[0] = "/Rick";
            Assert.IsFalse(Solstice.ReplaceInFiles.TryGetFileSearchOption(args, out so));
            args[0] = "-rick";
            Assert.IsFalse(Solstice.ReplaceInFiles.TryGetFileSearchOption(args, out so));
            args[0] = "-Rick";
            Assert.IsFalse(Solstice.ReplaceInFiles.TryGetFileSearchOption(args, out so));
        }

        [TestMethod]
        public void GetSearchOptionTrue()
        {
            string[] args = { "X", "Y", "Z" };
            SearchOption so;
            for (int i = 0; i < args.Length; i++)
            {
                args[0] = "X"; args[1] = "Y"; args[2] = "Z";
                args[i] = "/x";
                Assert.IsTrue(Solstice.ReplaceInFiles.TryGetFileSearchOption(args, out so));
            }
            for (int i = 0; i < args.Length; i++)
            {
                args[0] = "X"; args[1] = "Y"; args[2] = "Z";
                args[i] = "/X";
                Assert.IsTrue(Solstice.ReplaceInFiles.TryGetFileSearchOption(args, out so));
            }
            for (int i = 0; i < args.Length; i++)
            {
                args[0] = "X"; args[1] = "Y"; args[2] = "Z";
                args[i] = "-x";
                Assert.IsTrue(Solstice.ReplaceInFiles.TryGetFileSearchOption(args, out so));
            }
            for (int i = 0; i < args.Length; i++)
            {
                args[0] = "X"; args[1] = "Y"; args[2] = "Z";
                args[i] = "-X";
                Assert.IsTrue(Solstice.ReplaceInFiles.TryGetFileSearchOption(args, out so));
            }
        }

        [TestMethod]
        public void GetDoChangeFalse()
        {
            string[] args = new string[3];
            Solstice.ReplaceInFiles.YesorNo dc = Solstice.ReplaceInFiles.YesorNo.No;
            args[0] = "X"; args[1] = "Y"; args[2] = "Z";
            Assert.IsFalse(Solstice.ReplaceInFiles.TryGetDoChange(args, out dc));
            args[0] = "/Gordon";
            Assert.IsFalse(Solstice.ReplaceInFiles.TryGetDoChange(args, out dc));
            args[0] = "/golly";
            Assert.IsFalse(Solstice.ReplaceInFiles.TryGetDoChange(args, out dc));
            args[0] = "-Guy";
            Assert.IsFalse(Solstice.ReplaceInFiles.TryGetDoChange(args, out dc));
            args[0] = "-gruesome";
            Assert.IsFalse(Solstice.ReplaceInFiles.TryGetDoChange(args, out dc));
        }

        [TestMethod]
        public void GetDoChangeTrue()
        {
            string[] args = { "X", "Y", "Z" };
            Solstice.ReplaceInFiles.YesorNo dc = Solstice.ReplaceInFiles.YesorNo.No;
            for (int i = 0; i < args.Length; i++)
            {
                args[0] = "X"; args[1] = "Y"; args[2] = "Z";
                args[i] = "/go";
                Assert.IsTrue(Solstice.ReplaceInFiles.TryGetDoChange(args, out dc));
            }
            for (int i = 0; i < args.Length; i++)
            {
                args[0] = "X"; args[1] = "Y"; args[2] = "Z";
                args[i] = "/GO";
                Assert.IsTrue(Solstice.ReplaceInFiles.TryGetDoChange(args, out dc));
            }
            for (int i = 0; i < args.Length; i++)
            {
                args[0] = "X"; args[1] = "Y"; args[2] = "Z";
                args[i] = "-go";
                Assert.IsTrue(Solstice.ReplaceInFiles.TryGetDoChange(args, out dc));
            }
            for (int i = 0; i < args.Length; i++)
            {
                args[0] = "X"; args[1] = "Y"; args[2] = "Z";
                args[i] = "-GO";
                Assert.IsTrue(Solstice.ReplaceInFiles.TryGetDoChange(args, out dc));
            }
        }

        [TestMethod]
        public void GetPathNoPath()
        {
            string[] args = new string[0];
            string path;
            Assert.IsFalse(Solstice.ReplaceInFiles.TryGetPath(args, out path));
            args = new string[3];
            args[0] = "/r"; args[1] = "/x"; args[2] = "/p";
            Assert.IsFalse(Solstice.ReplaceInFiles.TryGetPath(args, out path));
            args = new string[2];
            args[0] = "-"; args[1] = "/";
            Assert.IsFalse(Solstice.ReplaceInFiles.TryGetPath(args, out path));
            args = new string[1];
            args[0] = "";
            Assert.IsFalse(Solstice.ReplaceInFiles.TryGetPath(args, out path));
            args[0] = "    ";
            Assert.IsFalse(Solstice.ReplaceInFiles.TryGetPath(args, out path));
            args[0] = "/f";
            Assert.IsFalse(Solstice.ReplaceInFiles.TryGetPath(args, out path));
            args[0] = "/f:";
            Assert.IsFalse(Solstice.ReplaceInFiles.TryGetPath(args, out path));
            args[0] = "-f";
            Assert.IsFalse(Solstice.ReplaceInFiles.TryGetPath(args, out path));
            args[0] = "-f:";
            Assert.IsFalse(Solstice.ReplaceInFiles.TryGetPath(args, out path));
            args[0] = "    ";
            Assert.IsFalse(Solstice.ReplaceInFiles.TryGetPath(args, out path));
            args[0] = "-f:    ";
            Assert.IsFalse(Solstice.ReplaceInFiles.TryGetPath(args, out path));
        }

        [TestMethod]
        public void GetPathGoodPath()
        {
            string expected = "Testing";
            string[] args = new string[1];
            string path;
            args[0] = "/f:" + expected;
            bool check = Solstice.ReplaceInFiles.TryGetPath(args, out path);
            Assert.IsTrue(check);
            args[0] = "-f:" + expected;
            check = Solstice.ReplaceInFiles.TryGetPath(args, out path);
            Assert.IsTrue(check);
            Assert.AreEqual(expected, path);
            args[0] = "-f:" + expected + "    ";
            check = Solstice.ReplaceInFiles.TryGetPath(args, out path);
            Assert.IsTrue(check);
            Assert.AreEqual(expected, path);
            args[0] = "/f:" + "    " + expected;
            check = Solstice.ReplaceInFiles.TryGetPath(args, out path);
            Assert.IsTrue(check);
            Assert.AreEqual(expected, path);
        }

        [TestMethod]
        public void GetSearchStringFail()
        {
            string[] args = new string[0];
            string searchString;
            Assert.IsFalse(Solstice.ReplaceInFiles.TryGetSearchString(args, out searchString));
            args = new string[1];
            args[0] = "Malformed parameter";
            Assert.IsFalse(Solstice.ReplaceInFiles.TryGetSearchString(args, out searchString));
            args[0] = "/f";
            Assert.IsFalse(Solstice.ReplaceInFiles.TryGetSearchString(args, out searchString));
            args[0] = "-f";
            Assert.IsFalse(Solstice.ReplaceInFiles.TryGetSearchString(args, out searchString));
            args[0] = "-f:";
            Assert.IsFalse(Solstice.ReplaceInFiles.TryGetSearchString(args, out searchString));
        }

        [TestMethod]
        public void GetSearchStringGood()
        {
            string expected = "This is the search string";
            string[] args = new string[2];
            string searchString;
            args[0] = "This is the path";
            args[1] = "/s:" + expected;
            bool isCheck = Solstice.ReplaceInFiles.TryGetSearchString(args, out searchString);
            Assert.IsTrue(isCheck);
            Assert.AreEqual(expected, searchString);
            expected = "    spaces before.";
            args[1] = "-s:" + expected;
            isCheck = Solstice.ReplaceInFiles.TryGetSearchString(args, out searchString);
            Assert.IsTrue(isCheck);
            Assert.AreEqual(expected, searchString);
            expected = "spaces after   ";
            args[1] = "-s:" + expected;
            isCheck = Solstice.ReplaceInFiles.TryGetSearchString(args, out searchString);
            Assert.IsTrue(isCheck);
            Assert.AreEqual(expected, searchString);
        }

        [TestMethod]
        public void GetReplacementStringFail()
        {
            string[] args = new string[0];
            string replacementString;
            Assert.IsFalse(Solstice.ReplaceInFiles.TryGetReplacementString(args, out replacementString));
            args = new string[1];
            args[0] = "Only a path";
            Assert.IsFalse(Solstice.ReplaceInFiles.TryGetReplacementString(args, out replacementString));
            args = new string[2];
            args[0] = "The path";
            args[1] = "The search string";
            Assert.IsFalse(Solstice.ReplaceInFiles.TryGetReplacementString(args, out replacementString));
        }

        [TestMethod]
        public void GetReplacementStringGood()
        {
            string expected = "This is the replacement string";
            string[] args = new string[3];
            string replacementString;
            args[0] = "This is the path";
            args[1] = "The search string";
            args[2] = "/r:" + expected;
            bool isCheck = Solstice.ReplaceInFiles.TryGetReplacementString(args, out replacementString);
            Assert.IsTrue(isCheck);
            Assert.AreEqual(expected, replacementString);
            expected = "    With spaces before";
            args[1] = "/r:" + expected;
            isCheck = Solstice.ReplaceInFiles.TryGetReplacementString(args, out replacementString);
            Assert.IsTrue(isCheck);
            Assert.AreEqual(expected, replacementString);
            expected = "With spaces after    ";
            args[1] = "/r:" + expected;
            isCheck = Solstice.ReplaceInFiles.TryGetReplacementString(args, out replacementString);
            Assert.IsTrue(isCheck);
            Assert.AreEqual(expected, replacementString);
        }

        [TestMethod]
        public void FindStringInFileFound()
        {
            GetSetup();
            FileInfo fi = new FileInfo("repeat.txt");
            Assert.IsTrue(fi.Exists);
            bool isCheck = Solstice.ReplaceInFiles.FindStringInFile(fi, "repeating", true);
            Assert.IsTrue(isCheck);
        }

        [TestMethod]
        public void FindStringInFileCaseSensitiveSearch()
        {
            GetSetup();
            FileInfo fi = new FileInfo("testing");
            Assert.IsTrue(fi.Exists);
            bool isCheck = Solstice.ReplaceInFiles.FindStringInFile(fi, "TEST", false);
            Assert.IsFalse(isCheck);
        }

        [TestMethod]
        public void ReplaceStringInFileRepeatTxt()
        {
            GetSetup();
            FileInfo fiActual = new FileInfo("repeat.txt");
            Solstice.ReplaceInFiles.ReplaceStringInFile(fiActual, "repeat", "test", false);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("testing");
            sb.AppendLine("testing");
            sb.AppendLine("testing");
            sb.AppendLine("testing");
            sb.AppendLine("testing testing testing");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("");
            string actual = File.ReadAllText(fiActual.FullName);
            string expected = sb.ToString();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ReplaceStringInFileUnicode()
        {
            GetSetup();
            FileInfo fiDestination = new FileInfo("Unicode.txt");
            FileInfo referenceFile = new FileInfo("Unicode_Expected.txt");
            Solstice.ReplaceInFiles.ReplaceStringInFile(fiDestination, "Unicode", "test", false);
            byte[] actual = File.ReadAllBytes(fiDestination.FullName);
            byte[] expected = File.ReadAllBytes(referenceFile.FullName);
            Assert.AreEqual(fiDestination.Length, referenceFile.Length);
            for (int i = 0; i < referenceFile.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i]);
            }
        }

        [TestMethod]
        public void ReplaceStringInFileUTF8()
        {
            GetSetup();
            FileInfo fiDestination = new FileInfo("UTF-8.txt");
            FileInfo referenceFile = new FileInfo("UTF-8_Expected.txt");
            Solstice.ReplaceInFiles.ReplaceStringInFile(fiDestination, "encoded", "test", false);
            byte[] actual = File.ReadAllBytes(fiDestination.FullName);
            byte[] expected = File.ReadAllBytes(referenceFile.FullName);
            Assert.AreEqual(fiDestination.Length, referenceFile.Length);
            for (int i = 0; i < referenceFile.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i]);
            }
        }

        [TestMethod]
        public void ReplaceStringInFileUnicodeBigEndian()
        {
            GetSetup();
            FileInfo fiDestination = new FileInfo("UnicodeBigEndian.txt");
            FileInfo referenceFile = new FileInfo("UnicodeBigEndian_Expected.txt");
            Solstice.ReplaceInFiles.ReplaceStringInFile(fiDestination, "encoded", "test", false);
            byte[] actual = File.ReadAllBytes(fiDestination.FullName);
            byte[] expected = File.ReadAllBytes(referenceFile.FullName);
            Assert.AreEqual(fiDestination.Length, referenceFile.Length);
            for (int i = 0; i < referenceFile.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i]);
            }
        }

        [TestMethod]
        public void ReplaceStringInFileAnsi()
        {
            GetSetup();
            FileInfo fiDestination = new FileInfo("Ansi.txt");
            FileInfo referenceFile = new FileInfo("Ansi_Expected.txt");
            Solstice.ReplaceInFiles.ReplaceStringInFile(fiDestination, "encoded", "test", false);
            byte[] actual = File.ReadAllBytes(fiDestination.FullName);
            byte[] expected = File.ReadAllBytes(referenceFile.FullName);
            Assert.AreEqual(fiDestination.Length, referenceFile.Length);
            for (int i = 0; i < referenceFile.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i]);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ReplaceStringInFileBinary()
        {
            GetSetup("", @"Test\Binaries");
            FileInfo fiDestination = new FileInfo(@"Binary.bin");
            FileInfo referenceFile = new FileInfo(@"Binary_Expected.bin");
            try
            {
                Solstice.ReplaceInFiles.ReplaceStringInFile(fiDestination, "encoded", "test", false);
            }
            finally
            {
                byte[] actual = File.ReadAllBytes(fiDestination.FullName);
                byte[] expected = File.ReadAllBytes(referenceFile.FullName);
                Assert.AreEqual(fiDestination.Length, referenceFile.Length);
                for (int i = 0; i < referenceFile.Length; i++)
                {
                    Assert.AreEqual(expected[i], actual[i]);
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ReplaceStringInFileExecutable()
        {
            GetSetup("", @"Test\Binaries");
            FileInfo fiDestination = new FileInfo(@"Executable.exe");
            FileInfo referenceFile = new FileInfo(@"Executable_Expected.exe");
            try
            {
                Solstice.ReplaceInFiles.ReplaceStringInFile(fiDestination, "encoded", "test", false);
            }
            finally
            {
                byte[] actual = File.ReadAllBytes(fiDestination.FullName);
                byte[] expected = File.ReadAllBytes(referenceFile.FullName);
                Assert.AreEqual(fiDestination.Length, referenceFile.Length);
                for (int i = 0; i < referenceFile.Length; i++)
                {
                    Assert.AreEqual(expected[i], actual[i]);
                }
            }
        }

        [TestMethod]
        public void ReplaceStringInFileCaseInsensitiveRepeatTxt()
        {
            GetSetup();
            FileInfo fiDestination = new FileInfo("repeat.txt");
            Solstice.ReplaceInFiles.ReplaceStringInFile(fiDestination, "REPEAT", "test", true);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("testing");
            sb.AppendLine("testing");
            sb.AppendLine("testing");
            sb.AppendLine("testing");
            sb.AppendLine("testing testing testing");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("");
            string actual = File.ReadAllText(fiDestination.FullName);
            string expected = sb.ToString();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ReplaceStringInFileCaseSensitiveRepeatTxt()
        {
            GetSetup();
            FileInfo fiActual = new FileInfo("repeat.txt");
            FileInfo fiExpected = new FileInfo(@"..\SavedFiles\repeat_AfterCaseSensitiveReplace.txt");
            Solstice.ReplaceInFiles.ReplaceStringInFile(fiActual, "REPEAT", "test", false);
            string actual = File.ReadAllText(fiActual.FullName);
            string expected = File.ReadAllText(fiExpected.FullName);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ReplaceStringInFileNoMatchTxt()
        {
            GetSetup();
            FileInfo fiActual = new FileInfo("repeat.txt");
            FileInfo fiExpected = new FileInfo(@"..\SavedFiles\repeat.txt");
            Solstice.ReplaceInFiles.ReplaceStringInFile(fiActual, "nomatch", "test", false);
            string actual = File.ReadAllText(fiActual.FullName);
            string expected = File.ReadAllText(fiExpected.FullName);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void JustSearchTest()
        {
            int actual = Solstice.ReplaceInFiles.Main(GetSetup("-f:* -s:123"));
            Assert.AreEqual(2, actual);
        }

        [TestMethod]
        public void EndToEndSearchOneFileTest()
        {
            // Should not find "testing" value
            Directory.SetCurrentDirectory(Path.Combine(savedCurrentDirectory, @"Test\ActualFiles"));
            string line = "/f:testing /s:test";
            string[] args = line.Split(new char[] { ' ' });
            int actual = Solstice.ReplaceInFiles.Main(args);
            Assert.AreEqual(1, actual);
        }

        [TestMethod]
        public void EndToEndSearchOneFileNoMatchTest()
        {
            // Should not find "testing" value
            Directory.SetCurrentDirectory(Path.Combine(savedCurrentDirectory, @"Test\ActualFiles"));
            string line = "/f:testing /s:nothing";
            string[] args = line.Split(new char[] { ' ' });
            int actual = Solstice.ReplaceInFiles.Main(args);
            Assert.AreEqual(0, actual);
        }

        [TestMethod]
        public void EndToEndSearchManyFilesTest()
        {
            // Working with 
            // Test\ActualFiles\123.txt
            // Test\ActualFiles\lala\123.txt
            // Test\ActualFiles\myfile.txt
            Directory.SetCurrentDirectory(Path.Combine(savedCurrentDirectory, @"Test\ActualFiles"));
            string line = "/f:* /s:123 /x";
            string[] args = line.Split(new char[] { ' ' });
            int actual = Solstice.ReplaceInFiles.Main(args);
            Assert.AreEqual(3, actual);
        }

        [TestMethod]
        public void EndToEndSearchManyFilesNoMatchTest()
        {
            int actual = Solstice.ReplaceInFiles.Main(GetSetup("/f:* -s:456 /x"));
            Assert.AreEqual(0, actual);
        }

        [TestMethod]
        public void EndToEndSearchMissingFileTest()
        {
            FileInfo fi = new FileInfo("DoesntExist");
            Assert.IsFalse(fi.Exists, "The file {0} must not exist", fi.FullName);

            string[] args = new string[2];
            args[0] = "/f:DoesntExist";
            args[1] = "/s:123";
            int actual = Solstice.ReplaceInFiles.Main(args);
            Assert.AreEqual(-1, actual);
        }

        [TestMethod]
        public void EndToEndSearchSpecificDirectoryTest()
        {
            int actual = Solstice.ReplaceInFiles.Main(GetSetup("/f:Basement /s:123"));
            Assert.AreEqual(-1, actual);
        }

        [TestMethod]
        public void EndToEndSearchWildCardRecursiveTest()
        {
            int actual = Solstice.ReplaceInFiles.Main(GetSetup("/f:* /s:level /i /x"));
            Assert.AreEqual(3, actual);
        }

        [TestMethod]
        public void EndToEndSearchWildCardNoMatchTest()
        {
            int actual = Solstice.ReplaceInFiles.Main(GetSetup("/f:* /s:nothing /x"));
            Assert.AreEqual(0, actual);
        }

        [TestMethod]
        public void EndToEndSearchWildCardRecursiveWithChangeTest()
        {
            string[] args = GetSetup("/f:* /s:level /r:changed /x /i /go");
            DirectoryCopy(@"..\SavedFiles\Basement", "Basement", true);
            int actual = Solstice.ReplaceInFiles.Main(args);
            Assert.AreEqual(3, actual);
            CompareFiles(@"Basement\sub.txt", @"..\SavedFiles\subExpectedBasement.txt");
            CompareFiles(@"Basement\SubLevelA\sub.txt", @"..\SavedFiles\subExpectedSubLevelA.txt");
            CompareFiles(@"Basement\SubLevelA\SubLevelB\sub.txt", @"..\SavedFiles\subExpectedSubLevelB.txt");
            CompareFiles(@"Basement\SubLevelA\SubLevelB\SubLevelC\sub.txt", @"..\SavedFiles\subExpectedSubLevelC.txt");
            DirectoryCopy(@"..\SavedFiles\Basement", "Basement", true);
        }

        [TestMethod]
        public void RecursiveReplaceWithSpecificFileNameTest()
        {
            string[] args = GetSetup("/f:sub.txt /s:level /r:changed /x /i /go");
            DirectoryCopy(@"..\SavedFiles\Basement", "Basement", true);
            int actual = Solstice.ReplaceInFiles.Main(args);
            Assert.AreEqual(3, actual);
            CompareFiles(@"Basement\sub.txt", @"..\SavedFiles\subExpectedBasement.txt");
            CompareFiles(@"Basement\SubLevelA\sub.txt", @"..\SavedFiles\subExpectedSubLevelA.txt");
            CompareFiles(@"Basement\SubLevelA\SubLevelB\sub.txt", @"..\SavedFiles\subExpectedSubLevelB.txt");
            CompareFiles(@"Basement\SubLevelA\SubLevelB\SubLevelC\sub.txt", @"..\SavedFiles\subExpectedSubLevelC.txt");
            DirectoryCopy(@"..\SavedFiles\Basement", "Basement", true);
        }

        [TestMethod]
        public void QuietTest()
        {
            string[] args = GetSetup("/f:* /s:level /x /i /q");
            DirectoryCopy(@"..\SavedFiles\Basement", "Basement", true);
            int actual = Solstice.ReplaceInFiles.Main(args.ToArray());
            Assert.AreEqual(3, actual);
            CompareFiles(@"Basement\sub.txt", @"..\SavedFiles\Basement\sub.txt");
            CompareFiles(@"Basement\SubLevelA\sub.txt", @"..\SavedFiles\Basement\SubLevelA\sub.txt");
            CompareFiles(@"Basement\SubLevelA\SubLevelB\sub.txt", @"..\SavedFiles\Basement\SubLevelA\SubLevelB\sub.txt");
            CompareFiles(@"Basement\SubLevelA\SubLevelB\SubLevelC\sub.txt", @"..\SavedFiles\Basement\SubLevelA\SubLevelB\SubLevelC\sub.txt");
        }

        #endregion
        #region Helper methods

        // From MSDN: http://msdn.microsoft.com/en-us/library/bb762914.aspx
        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory doesn't exist, create it. 
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, true);
            }

            // If copying subdirectories, copy them and their contents to new location. 
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        public string[] GetSetup(string commandLine = "", string directory = @"Test\ActualFiles")
        {
            // With no parameters simply change to the correct directory for testing.
            Directory.SetCurrentDirectory(Path.Combine(savedCurrentDirectory, directory));
            string[] args = commandLine.Split(new char[] { ' ' });
            return args;
        }


        public void CompareFiles(string actual, string expected)
        {
            byte[] bActual = File.ReadAllBytes(actual);
            byte[] bExpected = File.ReadAllBytes(expected);
            Assert.AreEqual(bActual.Length, bExpected.Length);
            for (int i = 0; i<bActual.Length; i++)
            {
                Assert.AreEqual(bExpected[i], bActual[i]);
            }
        }


        #endregion
    }
}
