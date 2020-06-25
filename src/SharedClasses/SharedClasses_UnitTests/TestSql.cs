using EdmondsCommunityCollege;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace UnitTests
{
    [TestClass]
    public class TestSql
    {
        static string TestServerName;
        static string TestDatabaseName;
        SqlProcessor SqlProcessor;

        [TestInitialize]
        public void TestInit()
        {
            MessageLogging.SetLogFileName(MessageLogging.WriteType.OverWrite, true);
            TestServerName = Environment.GetEnvironmentVariable("COMPUTERNAME");
            TestDatabaseName = "DbaAdmin";
            SqlProcessor = new SqlProcessor(TestServerName, TestDatabaseName);
        }

        [TestMethod]
        public void ConstructorTest()
        {
            SqlProcessor sp = new SqlProcessor("server", "dbName");
            Assert.AreEqual("server", sp.ServerName);
            Assert.AreEqual("dbName", sp.DatabaseName);
            Assert.AreEqual(0, sp.Rows.Count);
        }

        [TestMethod]
        public void GetConnectionStringTest()
        {
            Assert.AreEqual("Data Source=myServer;Initial Catalog=myDatabase;Integrated Security=True;Persist Security Info=False", SqlProcessor.GetConnectionString("myServer", "myDatabase"));
        }

        [TestMethod]
        public void SimpleCatchPrintTest()
        {
            SqlProcessor.ExecuteCatchPrint("print 'Testing'");
            Assert.AreEqual(1, SqlProcessor.Rows.Count);
            Assert.AreEqual("Testing", SqlProcessor.Rows[0].Columns[SqlRow.PrintColumnName].StringValue);
        }

        [TestMethod]
        public void SimpleCatchPrintIndexedTest()
        {
            SqlProcessor.ExecuteCatchPrint("print 'Testing'");
            Assert.AreEqual(1, SqlProcessor.Rows.Count);
            Assert.AreEqual("Testing", SqlProcessor.Rows[0].ColumnOrdinal[0].StringValue);
        }

        [TestMethod]
        [ExpectedException(typeof(SqlException))]
        public void CatchPrintBadQueryTest()
        {
            SqlProcessor.ExecuteCatchPrint("print Testing'");
        }

        [TestMethod]
        public void SimpleScalarTest()
        {
            SqlProcessor.ExecuteScalar("select 'Testing'");
            Assert.AreEqual(1, SqlProcessor.Rows.Count);
            Assert.AreEqual("System.String", SqlProcessor.Rows[0].Columns[SqlRow.ScalarColumnName].DataTypeName);
            Assert.AreEqual("Scalar Value", SqlProcessor.Rows[0].Columns[SqlRow.ScalarColumnName].ColumnName);
            Assert.AreEqual("Testing", SqlProcessor.Rows[0].Columns[SqlRow.ScalarColumnName].StringValue);
        }

        [TestMethod]
        public void SimpleScalarOrdinalTest()
        {
            SqlProcessor.ExecuteScalar("select 'Testing'");
            Assert.AreEqual(1, SqlProcessor.Rows.Count);
            Assert.AreEqual("System.String", SqlProcessor.Rows[0].ColumnOrdinal[0].DataTypeName);
            Assert.AreEqual("Scalar Value", SqlProcessor.Rows[0].ColumnOrdinal[0].ColumnName);
            Assert.AreEqual("Testing", SqlProcessor.Rows[0].ColumnOrdinal[0].StringValue);
        }

        [TestMethod]
        [ExpectedException(typeof(SqlException))]
        public void ScalarBadQueryTest()
        {
            SqlProcessor.ExecuteScalar("select 'Testing");
        }

        [TestMethod]
        public void ScalarTestWithOrdinalName()
        {
            SqlProcessor.ExecuteScalar("select 'Testing Again' as OnlyValue");
            Assert.AreEqual(1, SqlProcessor.Rows.Count);
            Assert.AreEqual("System.String", SqlProcessor.Rows[0].ColumnOrdinal[0].DataTypeName);
            Assert.AreEqual("Scalar Value", SqlProcessor.Rows[0].ColumnOrdinal[0].ColumnName);
            Assert.AreEqual("Testing Again", SqlProcessor.Rows[0].ColumnOrdinal[0].StringValue);
        }

        public void ScalarTestWithColumnName()
        {
            SqlProcessor.ExecuteScalar("select 'Testing Again' as OnlyValue");
            Assert.AreEqual(1, SqlProcessor.Rows.Count);
            Assert.AreEqual("System.String", SqlProcessor.Rows[0].ColumnOrdinal[0].DataTypeName);
            Assert.AreEqual("Scalar Value", SqlProcessor.Rows[0].ColumnOrdinal[0].ColumnName);
            Assert.AreEqual("Testing Again", SqlProcessor.Rows[0].Columns["OnlyValue"].StringValue);
        }

        [TestMethod]
        public void ScalarTestWithIgnoredColumns()
        {
            SqlProcessor.ExecuteScalar("select 'There can be only one' as OnlyValue, 'Ignored', 'Ignored2'");
            Assert.AreEqual(1, SqlProcessor.Rows.Count);
            Assert.AreEqual("System.String", SqlProcessor.Rows[0].ColumnOrdinal[0].DataTypeName);
            Assert.AreEqual("Scalar Value", SqlProcessor.Rows[0].ColumnOrdinal[0].ColumnName);
            Assert.AreEqual("There can be only one", SqlProcessor.Rows[0].ColumnOrdinal[0].StringValue);
        }

        [TestMethod]
        public void SimpleExecuteNonQueryTest()
        {
            SqlProcessor.ExecuteNonQuery("select 0");
            Assert.AreEqual(0, SqlProcessor.Rows.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(SqlException))]
        public void ExecuteNonQueryBadQueryTest()
        {
            SqlProcessor.ExecuteNonQuery("select '");
        }

        [TestMethod]
        public void SimpleExecuteReaderTest()
        {
            string columnName = "MyValue";
            SqlProcessor.ExecuteReader("select 0 As " + columnName);
            Assert.AreEqual(1, SqlProcessor.Rows.Count);
            Assert.AreEqual(1, SqlProcessor.Rows[0].Columns.Count);
            Assert.AreEqual("MyValue", SqlProcessor.Rows[0].Columns[columnName].ColumnName);
            Assert.AreEqual("System.Int32", SqlProcessor.Rows[0].Columns[columnName].DataTypeName);
            Assert.AreEqual(0, SqlProcessor.Rows[0].Columns[columnName].IntValue);
        }

        [TestMethod]
        public void SimpleExecuteReaderIndexedTest()
        {
            SqlProcessor.ExecuteReader("select 0 As MyValue");
            Assert.AreEqual(1, SqlProcessor.Rows.Count);
            Assert.AreEqual(1, SqlProcessor.Rows[0].ColumnOrdinal.Count);
            Assert.AreEqual("MyValue", SqlProcessor.Rows[0].ColumnOrdinal[0].ColumnName);
            Assert.AreEqual("System.Int32", SqlProcessor.Rows[0].ColumnOrdinal[0].DataTypeName);
            Assert.AreEqual(0, SqlProcessor.Rows[0].ColumnOrdinal[0].IntValue);
        }

        [TestMethod]
        [ExpectedException(typeof(SqlException))]
        public void ExecuteReaderBadQueryTest()
        {
            SqlProcessor.ExecuteReader("select '");
        }

        [TestMethod]
        public void SingleRowExecuteReaderTest()
        {
            string columnName1 = "MyFirstValue";
            string columnName2 = "SecondValue";
            string columnName3 = "TextValue";
            string columnName4 = "FloatValue";
            string columnName5 = "DecimalValue";
            string query = string.Format("select 0 As {0}, 1 as {1}, 'MyTextValue' as {2}, CONVERT(float, 33.22) as {3}, CONVERT(decimal, 1.2) as {4}",
                columnName1, columnName2, columnName3, columnName4, columnName5);
            SqlProcessor.ExecuteReader(query);
            Assert.AreEqual(1, SqlProcessor.Rows.Count);
            Assert.AreEqual(5, SqlProcessor.Rows[0].Columns.Count);

            Assert.AreEqual("MyFirstValue", SqlProcessor.Rows[0].Columns[columnName1].ColumnName);
            Assert.AreEqual("System.Int32", SqlProcessor.Rows[0].Columns[columnName1].DataTypeName);
            Assert.AreEqual(0, SqlProcessor.Rows[0].Columns[columnName1].IntValue);

            Assert.AreEqual("SecondValue", SqlProcessor.Rows[0].Columns[columnName2].ColumnName);
            Assert.AreEqual("System.Int32", SqlProcessor.Rows[0].Columns[columnName2].DataTypeName);
            Assert.AreEqual(1, SqlProcessor.Rows[0].Columns[columnName2].IntValue);

            Assert.AreEqual("TextValue", SqlProcessor.Rows[0].Columns[columnName3].ColumnName);
            Assert.AreEqual("System.String", SqlProcessor.Rows[0].Columns[columnName3].DataTypeName);
            Assert.AreEqual("MyTextValue", SqlProcessor.Rows[0].Columns[columnName3].StringValue);

            Assert.AreEqual("FloatValue", SqlProcessor.Rows[0].Columns[columnName4].ColumnName);
            Assert.AreEqual("System.Double", SqlProcessor.Rows[0].Columns[columnName4].DataTypeName);
            Assert.AreEqual(33.22, SqlProcessor.Rows[0].Columns[columnName4].DoubleValue);

            Assert.AreEqual("DecimalValue", SqlProcessor.Rows[0].Columns[columnName5].ColumnName);
            Assert.AreEqual("System.Decimal", SqlProcessor.Rows[0].Columns[columnName5].DataTypeName);
            Debug.WriteLine("Object value: {0}", SqlProcessor.Rows[0].Columns[columnName5].ObjectValue);
        }

        [TestMethod]
        public void SingleRowExecuteReaderOrdinalTest()
        {
            SqlProcessor.ExecuteReader("select 0 As MyFirstValue, 1 as SecondValue, 'MyTextValue' as TextValue, CONVERT(float, 33.22) as FloatValue, CONVERT(decimal, 1.2) as DecimalValue");
            Assert.AreEqual(1, SqlProcessor.Rows.Count);
            Assert.AreEqual(5, SqlProcessor.Rows[0].ColumnOrdinal.Count);

            Assert.AreEqual("MyFirstValue", SqlProcessor.Rows[0].ColumnOrdinal[0].ColumnName);
            Assert.AreEqual("System.Int32", SqlProcessor.Rows[0].ColumnOrdinal[0].DataTypeName);
            Assert.AreEqual(0, SqlProcessor.Rows[0].ColumnOrdinal[0].IntValue);

            Assert.AreEqual("SecondValue", SqlProcessor.Rows[0].ColumnOrdinal[1].ColumnName);
            Assert.AreEqual("System.Int32", SqlProcessor.Rows[0].ColumnOrdinal[1].DataTypeName);
            Assert.AreEqual(1, SqlProcessor.Rows[0].ColumnOrdinal[1].IntValue);

            Assert.AreEqual("TextValue", SqlProcessor.Rows[0].ColumnOrdinal[2].ColumnName);
            Assert.AreEqual("System.String", SqlProcessor.Rows[0].ColumnOrdinal[2].DataTypeName);
            Assert.AreEqual("MyTextValue", SqlProcessor.Rows[0].ColumnOrdinal[2].StringValue);

            Assert.AreEqual("FloatValue", SqlProcessor.Rows[0].ColumnOrdinal[3].ColumnName);
            Assert.AreEqual("System.Double", SqlProcessor.Rows[0].ColumnOrdinal[3].DataTypeName);
            Assert.AreEqual(33.22, SqlProcessor.Rows[0].ColumnOrdinal[3].DoubleValue);

            Assert.AreEqual("DecimalValue", SqlProcessor.Rows[0].ColumnOrdinal[4].ColumnName);
            Assert.AreEqual("System.Decimal", SqlProcessor.Rows[0].ColumnOrdinal[4].DataTypeName);
            Debug.WriteLine("Object value: {0}", SqlProcessor.Rows[0].ColumnOrdinal[4].ObjectValue);
        }

        [TestMethod]
        public void ExecuteReaderNullTest()
        {
            SqlProcessor.ExecuteReader("select null as MyNull");
            Assert.AreEqual(1, SqlProcessor.Rows.Count);
            Assert.AreEqual(1, SqlProcessor.Rows[0].Columns.Count);

            Assert.AreEqual("MyNull", SqlProcessor.Rows[0].Columns["MyNull"].ColumnName);
            Debug.WriteLine("DataTypeName: " + SqlProcessor.Rows[0].Columns["MyNull"].DataTypeName);
        }

        [TestMethod]
        public void ExecuteReaderNullIndexedTest()
        {
            SqlProcessor.ExecuteReader("select null as MyNull");
            Assert.AreEqual(1, SqlProcessor.Rows.Count);
            Assert.AreEqual(1, SqlProcessor.Rows[0].ColumnOrdinal.Count);

            Assert.AreEqual("MyNull", SqlProcessor.Rows[0].ColumnOrdinal[0].ColumnName);
            Debug.WriteLine("DataTypeName: " + SqlProcessor.Rows[0].ColumnOrdinal[0].DataTypeName);
        }

        [TestMethod]
        public void MultipleRowsExecuteReaderTest()
        {
            SqlProcessor.ExecuteReader("select * from sys.databases");
            Assert.IsTrue(SqlProcessor.Rows.Count > 5);
            Assert.IsTrue(SqlProcessor.Rows[0].Columns.Count > 5);
            Assert.IsTrue(SqlProcessor.Rows[0].ColumnOrdinal.Count > 5);
            Assert.AreEqual(SqlProcessor.Rows[0].ColumnOrdinal.Count, SqlProcessor.Rows[0].Columns.Count);
        }
        [TestMethod]
        public void ExecuteReaderWithSuppressTest()
        {
            SqlProcessor.SuppressPrint = true;
            SqlProcessor.SuppressRowCount = true;
            SqlProcessor.ExecuteReader("select * from sys.databases");
            Assert.IsTrue(SqlProcessor.Rows.Count > 5);
            Assert.IsTrue(SqlProcessor.Rows[0].ColumnOrdinal.Count > 5);

            SqlProcessor.SuppressPrint = false;
            SqlProcessor.SuppressRowCount = false;
            SqlProcessor.ExecuteReader("select * from sys.databases");
            Assert.IsTrue(SqlProcessor.Rows.Count > 5);
            Assert.IsTrue(SqlProcessor.Rows[0].ColumnOrdinal.Count > 5);
        }
        [TestMethod]
        public void SqlParameterTest()
        {
            SqlProcessor.ExecuteReader("select * from sys.databases");
            Assert.IsTrue(SqlProcessor.Rows.Count > 5);
            Assert.IsTrue(SqlProcessor.Rows[0].ColumnOrdinal.Count > 5);

            SqlProcessor.SuppressPrint = false;
            SqlProcessor.SuppressRowCount = false;
            SqlProcessor.ExecuteReader("select * from sys.databases");
            Assert.IsTrue(SqlProcessor.Rows.Count > 5);
            Assert.IsTrue(SqlProcessor.Rows[0].ColumnOrdinal.Count > 5);
        }
    }
}
