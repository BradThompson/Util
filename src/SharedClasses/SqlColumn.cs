using System;
using System.Data.SqlClient;

namespace TBTT
{
    public class SqlColumn
    {
        public string ColumnName { get; private set; }
        public int? Ordinal { get; private set; }
        public bool IsNull { get; private set; }
        public string DataTypeName { get; private set; }
        public int IntValue { get; private set; }
        public string StringValue { get; private set; }
        public Double DoubleValue { get; private set; }
        public DateTime DateTimeValue { get; private set; }
        public Object ObjectValue { get; private set; }

        public SqlColumn(string columnName = null, int? ordinal = null)
        {
            ColumnName = columnName;
            Ordinal = ordinal;
        }

        public void GetReaderValue(SqlDataReader reader)
        {
            if (string.IsNullOrWhiteSpace(ColumnName) && (Ordinal == null || Ordinal < 0))
            {
                throw new Exception("You must specify either a columnName or an ordinal number.");
            }
            if (!string.IsNullOrWhiteSpace(ColumnName) && Ordinal != null && Ordinal >= 0)
            {
                throw new Exception("Please specify either a column name or an ordinal, not both");
            }
            if (Ordinal == null)
                Ordinal = reader.GetOrdinal(ColumnName);
            else
                ColumnName = reader.GetName((int) Ordinal);
            if (reader.IsDBNull((int) Ordinal))
            {
                IsNull = true;
                IntValue = -1;
                StringValue = "";
                DoubleValue = -1;
                DateTimeValue = DateTime.Today;
                return;
            }
            DataTypeName = reader.GetFieldType((int)Ordinal).ToString();
            switch (DataTypeName)
            {
                case "System.Int32":
                    IntValue = reader.GetInt32((int)Ordinal);
                    break;
                case "System.String":
                    StringValue = reader.GetString((int)Ordinal);
                    break;
                case "System.Double":
                    DoubleValue = reader.GetDouble((int)Ordinal);
                    break;
                case "System.DateTime":
                    DateTimeValue = reader.GetDateTime((int)Ordinal);
                    break;
                default:
                    ObjectValue = reader.GetProviderSpecificValue((int)Ordinal);
                    MessageLogging.WriteLine(string.Format("The data type was unrecognized. The Sql datatype was {0}. The provider thinks it's {1}.",
                        DataTypeName, reader.GetProviderSpecificFieldType((int)Ordinal).ToString()));
                    break;
            }
        }

        public void GetScalarValue(object value)
        {
            ColumnName = "Scalar Value";
            Ordinal = 0;
            if (value == null)
            {
                IsNull = true;
                DataTypeName = "Unknown";
                IntValue = -1;
                StringValue = "";
                DoubleValue = -1;
                DateTimeValue = DateTime.Today;
                return;
            }
            DataTypeName = value.GetType().ToString();
            switch (DataTypeName)
            {
                case "System.Int32":
                    IntValue = Convert.ToInt32(value);
                    break;
                case "System.String":
                    StringValue = Convert.ToString(value);
                    break;
                case "System.Double":
                    DoubleValue = Convert.ToDouble(value);
                    break;
                case "System.DateTime":
                    DateTimeValue = Convert.ToDateTime(value);
                    break;
                default:
                    ObjectValue = value;
                    MessageLogging.WriteLine(string.Format("The data type was unrecognized: {0}", DataTypeName));
                    break;
            }
        }

        public void GetPrintValue(string value)
        {
            ColumnName = "Print Value";
            Ordinal = 0;
            DataTypeName = "System.String";
            if (value == null)
            {
                IsNull = true;
                IntValue = -1;
                StringValue = "";
                DoubleValue = -1;
                DateTimeValue = DateTime.Today;
                return;
            }
            StringValue = value;
        }

        public override string ToString()
        {
            if (IsNull)
            {
                return "";
            }
            switch (DataTypeName)
            {
                case "System.Int32":
                    return IntValue.ToString();
                case "System.String":
                    return StringValue;
                case "System.Double":
                    return DoubleValue.ToString();
                case "System.DateTime":
                    return DateTimeValue.ToString();
                default:
                    return ObjectValue.ToString();
            }
        }
    }
}
