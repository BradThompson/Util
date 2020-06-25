using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AddPathExe;

namespace AddPathTest
{
    [TestClass]
    public class UnitTest
    {
        public static string OriginalPath = "";
        [ClassInitialize]
        public static void ClassInit(TestContext tc)
        {
            OriginalPath = Environment.GetEnvironmentVariable("PATH");
        }

        [ClassCleanup]
        public static void ClassClean()
        {
            Environment.SetEnvironmentVariable("PATH", OriginalPath);
            DeleteDirectories(new string[] { "George", "Paul", "Ringo", "John", "AtOne", "AtTwo", "AtThree", "AtFour", "AtFive", "AtSix" });
        }

        [TestInitialize]
        public void TestInit()
        {
            ClearAddPath();
        }

        public static void DeleteDirectories(string[] dirsToDelete)
        {
            foreach (string dir in dirsToDelete)
            {
                if (Directory.Exists(dir))
                    Directory.Delete(dir);
            }
        }

        [TestMethod]
        public void AddOne()
        {
            Environment.SetEnvironmentVariable("PATH", null);
            string one = CreateDirectory("George");
            Assert.AreEqual(1, AddPathExe.AddPathExe.Main(new string[] { "George" }));
            TestPath("SET Path=" + one);
        }

        [TestMethod]
        public void AddOneChangingPosition()
        {
            Environment.SetEnvironmentVariable("PATH", "this;is;one;way;c:\\myDir;to;do;this");
            string myDir = new DirectoryInfo("c:\\myDir").FullName;
            Assert.AreEqual(1, AddPathExe.AddPathExe.Main(new string[] { "-s", myDir, "-i5" }));
            TestPath("SET Path=this;is;one;way;to;" + myDir + ";do");
        }

        [TestMethod]
        public void AddOneWithInsertNoCheck()
        {
            Environment.SetEnvironmentVariable("PATH", null);
            string oneDir = new DirectoryInfo("Johnson").FullName;
            Assert.AreEqual(1, AddPathExe.AddPathExe.Main(new string[] { oneDir, "/I0", "-S" }));
            TestPath("SET Path=" + oneDir);
        }

        [TestMethod]
        public void TwoDirs()
        {
            Environment.SetEnvironmentVariable("PATH", null);
            Assert.AreEqual(0, AddPathExe.AddPathExe.Main(new string[] { "Upto", "NoGood" }));
        }

        [TestMethod]
        public void Semicolon()
        {
            Environment.SetEnvironmentVariable("PATH", null);
            Assert.AreEqual(0, AddPathExe.AddPathExe.Main(new string[] { "semi;colon", "-s" }));
        }

        [TestMethod]
        public void WithQuotes()
        {
            Environment.SetEnvironmentVariable("PATH", null);
            string oneDir = new DirectoryInfo("WithQuotes").FullName;
            Assert.AreEqual(1, AddPathExe.AddPathExe.Main(new string[] { "\"WithQuotes\"", "-s" }));
            TestPath("SET Path=" + oneDir);
        }

        [TestMethod]
        public void WithSpacesAndQuotes()
        {
            Environment.SetEnvironmentVariable("PATH", null);
            string oneDir = new DirectoryInfo("With Quotes").FullName;
            Assert.AreEqual(1, AddPathExe.AddPathExe.Main(new string[] { "\"With Quotes\"", "-s" }));
            TestPath("SET Path=" + oneDir);
        }

        [TestMethod]
        public void WeirdExistingPath()
        {
            Environment.SetEnvironmentVariable("PATH", "   what   ;   should ; be   ; done");
            string oneDir = new DirectoryInfo("With Quotes").FullName;
            Assert.AreEqual(1, AddPathExe.AddPathExe.Main(new string[] { "\"With Quotes\"", "-s" }));
            TestPath("SET Path=what;should;be;done;" + oneDir);
        }

        [TestMethod]
        public void CurrentPath()
        {
            string original = @"C:\Program Files\Common Files\Microsoft Shared\Microsoft Online Services;C:\Program Files (x86)\Common Files\Microsoft Shared\Microsoft Online Services;C:\WINDOWS\system32;C:\WINDOWS;C:\WINDOWS\System32\Wbem;C:\WINDOWS\System32\WindowsPowerShell\v1.0;C:\Program Files\nodejs;C:\Program Files\Microsoft SQL Server\120\Tools\Binn;C:\Program Files\Microsoft SQL Server\130\Tools\Binn;c:\Util;C:\Program Files\Git\cmd;C:\Program Files\Git\bin;C:\Python27;C:\Util;C:\PhantomJs\bin;C:\Program Files\Git\mingw64\bin;C:\Program Files\Microsoft SQL Server\110\Tools\Binn;C:\Program Files\dotnet;C:\Program Files\Git\usr\bin;C:\Users\V-BRTH\AppData\Roaming\npm";
            Environment.SetEnvironmentVariable("PATH", original);
            string oneDir = new DirectoryInfo("With Quotes and (parens)").FullName;
            Assert.AreEqual(1, AddPathExe.AddPathExe.Main(new string[] { "\"With Quotes and (parens)\"", "-s" }));
            TestPath("SET Path=" + original + ";" + oneDir);
        }

        [TestMethod]
        public void AddNoCheck()
        {
            Environment.SetEnvironmentVariable("PATH", null);
            string one = "King George";
            string two = "Thompson";
            string oneDir = new DirectoryInfo(one).FullName;
            string twoDir = new DirectoryInfo(two).FullName;

            Assert.AreEqual(1, AddPathExe.AddPathExe.Main(new string[] { "/s", one }));
            TestPath("SET Path=" + oneDir);
            ClearAddPath();
            Environment.SetEnvironmentVariable("PATH", oneDir);
            Assert.AreEqual(1, AddPathExe.AddPathExe.Main(new string[] { "/s", twoDir }));
            TestPath("SET Path=" + oneDir + ";" + twoDir);
        }

        [TestMethod]
        public void AddTwo()
        {
            Environment.SetEnvironmentVariable("PATH", null);
            string one = CreateDirectory("George");
            string two = CreateDirectory("Ringo");

            Assert.AreEqual(1, AddPathExe.AddPathExe.Main(new string[] { "George" }));
            ClearAddPath();
            Environment.SetEnvironmentVariable("PATH", one);
            Assert.AreEqual(1, AddPathExe.AddPathExe.Main(new string[] { "Ringo" }));
            TestPath("SET Path=" + one + ";" + two);
        }

        [TestMethod]
        public void AddFour()
        {
            Environment.SetEnvironmentVariable("PATH", null);

            string one = "John";
            string two = "George";
            string three = "Paul";
            string four = "Ringo";
            string oneDir = CreateDirectory(one);
            string twoDir = CreateDirectory(two);
            string threeDir = CreateDirectory(three);
            string fourDir = CreateDirectory(four);

            Assert.AreEqual(1, AddPathExe.AddPathExe.Main(new string[] { one }));
            ClearAddPath();
            Environment.SetEnvironmentVariable("PATH", oneDir);

            Assert.AreEqual(1, AddPathExe.AddPathExe.Main(new string[] { two }));
            ClearAddPath();
            Environment.SetEnvironmentVariable("PATH", oneDir + ";" + twoDir);

            Assert.AreEqual(1, AddPathExe.AddPathExe.Main(new string[] { three }));
            ClearAddPath();
            Environment.SetEnvironmentVariable("PATH", oneDir + ";" + twoDir + ";" + threeDir);

            Assert.AreEqual(1, AddPathExe.AddPathExe.Main(new string[] { four }));
            ClearAddPath();
            Environment.SetEnvironmentVariable("PATH", oneDir + ";" + twoDir + ";" + threeDir + ";" + fourDir);

            TestPath("SET Path=" + oneDir + ";" + twoDir + ";" + threeDir + ";" + fourDir);
        }

        [TestMethod]
        public void AddOneAtIndex()
        {
            Environment.SetEnvironmentVariable("PATH", @"George;Ringo;John;Paul;Stuart;Roag");
            string one = "AtOne";
            string oneDir = CreateDirectory(one);

            Assert.AreEqual(1, AddPathExe.AddPathExe.Main(new string[] { "/I0", oneDir }));
            TestPath("SET Path=" + oneDir + ";George;Ringo;John;Paul;Stuart;Roag");
            Environment.SetEnvironmentVariable("PATH", @"George;Ringo;John;Paul;Stuart;Roag");
            ClearAddPath();

            Assert.AreEqual(1, AddPathExe.AddPathExe.Main(new string[] { "/I1", one }));
            TestPath("SET Path=George;" + oneDir + ";Ringo;John;Paul;Stuart;Roag");
            Environment.SetEnvironmentVariable("PATH", @"George;Ringo;John;Paul;Stuart;Roag");
            ClearAddPath();

            Assert.AreEqual(1, AddPathExe.AddPathExe.Main(new string[] { "/I2", oneDir }));
            TestPath("SET Path=George;Ringo;" + oneDir + ";John;Paul;Stuart;Roag");
            Environment.SetEnvironmentVariable("PATH", @"George;Ringo;John;Paul;Stuart;Roag");
            ClearAddPath();

            Assert.AreEqual(1, AddPathExe.AddPathExe.Main(new string[] { "/I3", oneDir }));
            TestPath("SET Path=George;Ringo;John;" + oneDir + ";Paul;Stuart;Roag");
            Environment.SetEnvironmentVariable("PATH", @"George;Ringo;John;Paul;Stuart;Roag");
            ClearAddPath();

            Assert.AreEqual(1, AddPathExe.AddPathExe.Main(new string[] { "/I4", oneDir }));
            TestPath("SET Path=George;Ringo;John;Paul;" + oneDir + ";Stuart;Roag");
            Environment.SetEnvironmentVariable("PATH", @"George;Ringo;John;Paul;Stuart;Roag");
            ClearAddPath();

            Assert.AreEqual(1, AddPathExe.AddPathExe.Main(new string[] { "/I5", oneDir }));
            TestPath("SET Path=George;Ringo;John;Paul;Stuart;" + oneDir + ";Roag");
            Environment.SetEnvironmentVariable("PATH", @"George;Ringo;John;Paul;Stuart;Roag");
            ClearAddPath();

            Assert.AreEqual(1, AddPathExe.AddPathExe.Main(new string[] { "/I6", oneDir }));
            TestPath("SET Path=George;Ringo;John;Paul;Stuart;Roag;" + oneDir);
            Environment.SetEnvironmentVariable("PATH", @"George;Ringo;John;Paul;Stuart;Roag");
            ClearAddPath();

            Assert.AreEqual(1, AddPathExe.AddPathExe.Main(new string[] { "/I9999", oneDir }));
            TestPath("SET Path=George;Ringo;John;Paul;Stuart;Roag;" + oneDir);
            Environment.SetEnvironmentVariable("PATH", @"George;Ringo;John;Paul;Stuart;Roag");
            ClearAddPath();

            Assert.AreEqual(1, AddPathExe.AddPathExe.Main(new string[] { oneDir }));
            TestPath("SET Path=George;Ringo;John;Paul;Stuart;Roag;" + oneDir);
            Environment.SetEnvironmentVariable("PATH", @"George;Ringo;John;Paul;Stuart;Roag");
            ClearAddPath();
        }

        [TestMethod]
        public void AddDuplicate()
        {
            Environment.SetEnvironmentVariable("PATH", null);
            string tmpDir = CreateDirectory("C:\\Tmp");
            Assert.AreEqual(1, AddPathExe.AddPathExe.Main(new string[] { "C:\\Tmp" }));
            TestPath("SET Path=C:\\Tmp");
            Environment.SetEnvironmentVariable("PATH", "C:\\Tmp");
            ClearAddPath();
            Assert.AreEqual(1, AddPathExe.AddPathExe.Main(new string[] { "C:\\Tmp" }));
            TestPath("SET Path=C:\\Tmp");
        }

        [TestMethod]
        public void AddExistingDuplicate()
        {
            Environment.SetEnvironmentVariable("PATH", "C:\\Util");
            string tmpDir = CreateDirectory("C:\\Tmp");
            Assert.AreEqual(1, AddPathExe.AddPathExe.Main(new string[] { "C:\\Tmp" }));
            TestPath("SET Path=C:\\Util;C:\\Tmp");
            Environment.SetEnvironmentVariable("PATH", "C:\\Util;C:\\Tmp");
            ClearAddPath();
            Assert.AreEqual(1, AddPathExe.AddPathExe.Main(new string[] { "C:\\Tmp" }));
            TestPath("SET Path=C:\\Util;C:\\Tmp");
        }

        [TestMethod]
        public void RemoveDuplicates()
        {
            Environment.SetEnvironmentVariable("PATH", "C:\\Util;C:\\Util;C:\\Util;C:\\Util");
            string tmpDir = CreateDirectory("C:\\Tmp");
            Assert.AreEqual(1, AddPathExe.AddPathExe.Main(new string[] { "C:\\Tmp" }));
            TestPath("SET Path=C:\\Util;C:\\Tmp");
        }

        [TestMethod]
        public void RemoveOne()
        {
            Environment.SetEnvironmentVariable("PATH", "John;Paul;George;Ringo");
            string one = CreateDirectory("George");
            Assert.AreEqual(1, AddPathExe.AddPathExe.Main(new string[] { "/R", "George" }));
            TestPath("SET Path=" + "John;Paul;Ringo");
        }

        [TestMethod]
        public void RemoveFour()
        {
            Environment.SetEnvironmentVariable("PATH", "John;Paul;George;Ringo");
            Assert.AreEqual(1, AddPathExe.AddPathExe.Main(new string[] { "/R", "George" }));
            TestPath("SET Path=" + "John;Paul;Ringo");
            Environment.SetEnvironmentVariable("PATH", "John;Paul;Ringo");
            ClearAddPath();
            Assert.AreEqual(1, AddPathExe.AddPathExe.Main(new string[] { "/R", "Ringo" }));
            TestPath("SET Path=" + "John;Paul");
            Environment.SetEnvironmentVariable("PATH", "John;Paul");
            ClearAddPath();
            Assert.AreEqual(1, AddPathExe.AddPathExe.Main(new string[] { "/R", "Paul" }));
            TestPath("SET Path=" + "John");
            Environment.SetEnvironmentVariable("PATH", "John");
            ClearAddPath();
            Assert.AreEqual(1, AddPathExe.AddPathExe.Main(new string[] { "/R", "John" }));
            TestPath("SET Path=");
        }

        [TestMethod]
        public void RemoveAndInsert()
        {
            Environment.SetEnvironmentVariable("PATH", "Testing;1;2;3");
            string one = "John Glenn";
            string oneDir = new DirectoryInfo(one).FullName;

            Assert.AreEqual(0, AddPathExe.AddPathExe.Main(new string[] { "/s", "/i9", "-r", "Nothing" }));
        }

        [TestMethod]
        public void RemoveNoDir()
        {
            Environment.SetEnvironmentVariable("PATH", "Testing;1;2;3");
            Assert.AreEqual(0, AddPathExe.AddPathExe.Main(new string[] { "/s", "-r" }));
        }

        [TestMethod]
        public void InsertNoDir()
        {
            Environment.SetEnvironmentVariable("PATH", "Testing;1;2;3");
            Assert.AreEqual(0, AddPathExe.AddPathExe.Main(new string[] { "-i9" }));
        }

        [TestMethod]
        public void AddWithCheck()
        {
            Environment.SetEnvironmentVariable("PATH", null);
            string one = "King George";
            string two = "Thompson";
            string oneDir = new DirectoryInfo(one).FullName;
            string twoDir = new DirectoryInfo(two).FullName;

            Assert.AreEqual(0, AddPathExe.AddPathExe.Main(new string[] { one }));
            ClearAddPath();
            Assert.AreEqual(0, AddPathExe.AddPathExe.Main(new string[] { twoDir }));
        }

        [TestMethod]
        public void RemoveWithCheck()
        {
            Environment.SetEnvironmentVariable("PATH", "King George");
            string one = "King George";
            string oneDir = new DirectoryInfo(one).FullName;

            Assert.AreEqual(1, AddPathExe.AddPathExe.Main(new string[] { "-r", one }));
            TestPath("SET Path=");
        }

        [TestMethod]
        public void RemoveWithCheckAndPath()
        {
            Environment.SetEnvironmentVariable("PATH", "King;George");
            string one = "King George";
            string oneDir = new DirectoryInfo(one).FullName;

            Assert.AreEqual(1, AddPathExe.AddPathExe.Main(new string[] { "-r", one }));
            TestPath("SET Path=King;George");
        }

        public void ClearAddPath()
        {
            AddPathExe.AddPathExe.RemovePath = false;
            AddPathExe.AddPathExe.SkipDirectoryTest = false;
            AddPathExe.AddPathExe.InsertLocation = int.MaxValue;
            AddPathExe.AddPathExe.PathToAdd = "";
        }

        public string CreateDirectory(string newPath)
        {
            DirectoryInfo di = new DirectoryInfo(newPath);
            if (!di.Exists)
                di.Create();
            return di.FullName;
        }

        public void TestPath(string expected)
        {
            Assert.IsTrue(AddPathExe.AddPathExe.BatchFullPath.Length > 0, "BatchFullPath is set");
            Assert.IsTrue(File.Exists(AddPathExe.AddPathExe.BatchFullPath), "BatchFullPath exists");
            string[] actualLines = File.ReadAllLines(AddPathExe.AddPathExe.BatchFullPath);
            Assert.AreEqual(1, actualLines.Length);
            string actual = actualLines[0].Trim();
            Console.WriteLine("Expected: {0}", expected);
            if (expected != actual)
                Console.WriteLine("Actual: {0}", actual);
            Assert.AreEqual(expected, actual);
        }
    }
}
