using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace TBTT
{
    public class SqlProcessor
    {
        public static bool SuppressRowCount { get; set; }
        public static bool SuppressPrint { get; set; }

        private static bool catchPrint = false;
        private static string caughtPrint = "";
        public string ServerName { get; private set; }
        public string DatabaseName { get; private set; }
        public List<SqlRow> Rows { get; private set; }
        static public int? Timeout { get; set; }

        public SqlProcessor(string serverName, string databaseName)
        {
            ServerName = serverName;
            DatabaseName = databaseName;
            Rows = new List<SqlRow>();
        }

        public void ExecuteReader(string query, CommandType commandType = CommandType.Text, List<SqlParameter> parameters = null)
        {
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(GetConnectionString(ServerName, DatabaseName)))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        if (Timeout != null)
                        {
                            command.CommandTimeout = (int)Timeout;
                        }
                        connection.InfoMessage += OnInfoMessageGenerated;
                        connection.Open();
                        command.StatementCompleted += OnStatementCompleted;
                        command.CommandType = commandType;
                        if (parameters != null)
                        {
                            command.Parameters.AddRange(parameters.ToArray());
                        }
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                SqlRow row = new SqlRow();
                                row.GetReaderRow(reader);
                                Rows.Add(row);
                            }
                        }
                    }
                }
            }
            catch (SqlException se)
            {
                ProcessSqlError(se);
                throw;
            }
            catch (Exception e)
            {
                MessageLogging.WriteLine(string.Format("Exception {0}", e.Message));
            }
        }

        public void ExecuteScalar(string query)
        {
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(GetConnectionString(ServerName, DatabaseName)))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        if (Timeout != null)
                        {
                            command.CommandTimeout = (int)Timeout;
                        }
                        connection.InfoMessage += OnInfoMessageGenerated;
                        connection.Open();
                        command.StatementCompleted += OnStatementCompleted;
                        Object value = command.ExecuteScalar();
                        SqlRow row = new SqlRow();
                        row.GetScalarValue(value);
                        Rows.Add(row);
                        MessageLogging.WriteLine(string.Format("DataTypeName: {0}, Value: {1}",
                            row.ColumnOrdinal[0].DataTypeName, row.ColumnOrdinal[0].ToString()));
                    }
                }
            }
            catch (SqlException se)
            {
                ProcessSqlError(se);
                throw;
            }
            catch (Exception e)
            {
                MessageLogging.WriteLine(string.Format("Exception {0}", e.Message));
                throw;
            }
        }

        public void ExecuteNonQuery(string query)
        {
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(GetConnectionString(ServerName, DatabaseName)))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        if (Timeout != null)
                        {
                            command.CommandTimeout = (int)Timeout;
                        }
                        connection.InfoMessage += OnInfoMessageGenerated;
                        connection.Open();
                        command.StatementCompleted += OnStatementCompleted;
                        int rowCount = command.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException se)
            {
                ProcessSqlError(se);
                throw;
            }
            catch (Exception e)
            {
                MessageLogging.WriteLine(string.Format("Exception {0}", e.Message));
                throw;
            }
        }

        public void ExecuteCatchPrint(string query)
        {
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(GetConnectionString(ServerName, DatabaseName)))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        if (Timeout != null)
                        {
                            command.CommandTimeout = (int)Timeout;
                        }
                        connection.InfoMessage += OnInfoMessageGenerated;
                        connection.Open();
                        command.StatementCompleted += OnStatementCompleted;
                        catchPrint = true;
                        command.ExecuteNonQuery();
                        SqlRow row = new SqlRow();
                        row.GetPrintValue(caughtPrint);
                        Rows.Add(row);

                    }
                }
            }
            catch (SqlException se)
            {
                ProcessSqlError(se);
                throw;
            }
            catch (Exception e)
            {
                MessageLogging.WriteLine(string.Format("Exception {0}", e.Message));
                throw;
            }
            finally
            {
                catchPrint = false;
                caughtPrint = "";
            }
        }

        private void ProcessSqlError(SqlException se)
        {
            foreach (SqlError item in se.Errors)
            {
                if (item.Class != 0)
                {
                    MessageLogging.WriteLine(string.Format("SqlError: Msg: {0}, Class: {1}, State: {2}, Line: {3}",
                            item.Message, item.Class, item.State, item.LineNumber));
                }
                else
                {
                    MessageLogging.WriteLine(string.Format("SqlError: Msg: {0}", item.Message));
                }
            }
        }

        private static void OnStatementCompleted(object sender, StatementCompletedEventArgs args)
        {
            if (!SuppressRowCount)
            {
                Console.WriteLine("({0} row(s) affected)", args.RecordCount);
            }
        }

        private static void OnInfoMessageGenerated(object sender, SqlInfoMessageEventArgs args)
        {
            foreach (SqlError err in args.Errors)
            {
                if (err.Class <= 1)
                {
                    if (catchPrint)
                    {
                        caughtPrint += err.Message;
                    }
                    if (!SuppressPrint)
                    {
                        Console.WriteLine("{0}", err.Message);
                    }
                }
                else if (err.Class > 1 && err.Class <= 10)
                {
                    Console.WriteLine("Class {0}: {1}", err.Message);
                }
                else if (err.Class > 10 && err.Class <= 16)
                {
                    Console.WriteLine("USER DEFINED ERROR: Msg {0}, Level {1}, State {2}, Line {3}",
                        err.Number, err.Class, err.State, err.LineNumber);
                }
                else if (err.Class >= 17)
                {
                    Console.WriteLine("SYSTEM DEFINED ERROR: Msg {0}, Level {1}, State {2}, Line {3}",
                        err.Number, err.Class, err.State, err.LineNumber);
                }
            }
        }

        public static string GetConnectionString(string serverName, string databaseName)
        {
            SqlConnectionStringBuilder cb = new SqlConnectionStringBuilder();
            cb.DataSource = serverName;
            cb.PersistSecurityInfo = false;
            cb.IntegratedSecurity = true;
            cb.InitialCatalog = databaseName;
            if (Timeout != null)
            {
                cb.ConnectTimeout = (int)Timeout;
            }
            return cb.ConnectionString;
        }
    }
}
