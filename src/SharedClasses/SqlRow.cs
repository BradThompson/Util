using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace TBTT
{
    public class SqlRow
    {
        public const string PrintColumnName = "Print Value";
        public const string ScalarColumnName = "Scalar Value";

        public Dictionary<string, SqlColumn> Columns;
        public Dictionary<int, SqlColumn> ColumnOrdinal;

        public SqlRow()
        {
            Columns = new Dictionary<string, SqlColumn>();
            ColumnOrdinal = new Dictionary<int, SqlColumn>();
        }

        public void GetReaderRow(SqlDataReader reader)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                SqlColumn se = new SqlColumn(null, i);
                se.GetReaderValue(reader);
                Columns.Add(se.ColumnName, se);
                ColumnOrdinal.Add(i, se);
            }
        }

        public void GetScalarValue(object value)
        {
            SqlColumn se = new SqlColumn(ScalarColumnName);
            se.GetScalarValue(value);
            Columns.Add(se.ColumnName, se);
            ColumnOrdinal.Add(0, se);
        }

        public void GetPrintValue(string value)
        {
            SqlColumn se = new SqlColumn(PrintColumnName);
            se.GetPrintValue(value);
            Columns.Add(se.ColumnName, se);
            ColumnOrdinal.Add(0, se);
        }
    }
}
