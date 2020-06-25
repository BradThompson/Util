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
    public class TestStoredProcedures
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
            // NOTE! The stored proceudre uspTestParameters must be installed on DbaAdmin
            // for the tests.
        }

        [TestMethod]
        public void SimpleCall()
        {
            SqlProcessor.ExecuteReader("uspTestParameters 0");
            Assert.AreEqual(0, SqlProcessor.Rows.Count);
        }
        [TestMethod]
        [ExpectedException(typeof(SqlException))]
        public void SimpleCallAsProc()
        {
            // When specifying the string is a stored procedure, parameters must be called explicitly.
            SqlProcessor.ExecuteReader("uspTestParameters 2", CommandType.StoredProcedure);
            Assert.Fail("An exception should have occured.");
        }
        [TestMethod]
        [ExpectedException(typeof(SqlException))]
        public void CallProcMissingRequiredParameter()
        {
            SqlProcessor.ExecuteReader("uspTestParameters", CommandType.StoredProcedure);
            Assert.Fail("An exception should have occured.");
        }
        [TestMethod]
        public void RequiredParametersDefined()
        {
            int expected = 3;
            List<SqlParameter> parms = new List<SqlParameter>();
            SqlParameter parm = new SqlParameter("@RequiredParameter", expected);
            parms.Add(parm);
            SqlProcessor.ExecuteReader("uspTestParameters", CommandType.StoredProcedure, parms);
            Assert.AreEqual(expected, SqlProcessor.Rows.Count);
        }
        [TestMethod]
        public void BothParametersDefined()
        {
            int expected = 4;
            List<SqlParameter> parms = new List<SqlParameter>();
            SqlParameter parm = new SqlParameter("@RequiredParameter", expected);
            parms.Add(parm);
            parm = new SqlParameter("@OptionalParameter", "Ignore");
            parms.Add(parm);
            SqlProcessor.ExecuteReader("uspTestParameters", CommandType.StoredProcedure, parms);
            Assert.AreEqual(expected, SqlProcessor.Rows.Count);
        }
        [TestMethod]
        public void VerifyResults()
        {
            int expected = 1;
            List<SqlParameter> parms = new List<SqlParameter>();
            SqlParameter parm = new SqlParameter("@RequiredParameter", expected);
            parms.Add(parm);
            parm = new SqlParameter("@OptionalParameter", "Ignore");
            parms.Add(parm);
            SqlProcessor.ExecuteReader("uspTestParameters", CommandType.StoredProcedure, parms);
            Assert.AreEqual(expected, SqlProcessor.Rows.Count);
            SqlRow row = SqlProcessor.Rows[0];
            Assert.AreEqual(2, row.Columns.Count);
            Assert.AreEqual(1, row.ColumnOrdinal[0].IntValue);
            Assert.AreEqual("ReturnedInt", row.ColumnOrdinal[0].ColumnName);
            Assert.IsTrue(row.ColumnOrdinal[1].IsNull);
            Assert.AreEqual("ReturnedVarChar", row.ColumnOrdinal[1].ColumnName);
        }
        public void VerifyResultsWithEmptyColumnName()
        {
            int expectedOne = 99;
            string expectedTwo = "OneResult";
            int expectedThree = 42;
            List<SqlParameter> parms = new List<SqlParameter>();
            SqlParameter parm = new SqlParameter("@RequiredParameter", expectedOne);
            parms.Add(parm);
            parm = new SqlParameter("@OptionalParameter", "OneResult");
            parms.Add(parm);
            SqlProcessor.ExecuteReader("uspTestParameters", CommandType.StoredProcedure, parms);
            Assert.AreEqual(1, SqlProcessor.Rows.Count);
            SqlRow row = SqlProcessor.Rows[0];
            Assert.AreEqual(3, row.Columns.Count);

            Assert.AreEqual(expectedOne, row.ColumnOrdinal[0].IntValue);
            Assert.AreEqual("ReturnedInt", row.ColumnOrdinal[0].ColumnName);

            Assert.AreEqual(expectedTwo, row.ColumnOrdinal[1].StringValue);
            Assert.AreEqual("ReturnedVarChar", row.ColumnOrdinal[1].ColumnName);

            Assert.AreEqual(expectedThree, row.ColumnOrdinal[2].IntValue);
            Assert.AreEqual("", row.ColumnOrdinal[2].ColumnName);
        }
    }
}
