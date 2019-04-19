using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EMSDatabase
{
    /// <summary>
    /// Controls the sql connection and has proxy methods for creating queries and stored procedure calls.
    /// </summary>
    public class QueryFactory
    {
        private string _connectionStr;

        private SqlConnection connection;
        
        public string ConnectionString
        {
            get => _connectionStr;

            set
            {
                _connectionStr = value;
                connection?.Close();

                if (_connectionStr != null)
                {
                    Console.WriteLine("Trying to connect to " + _connectionStr);
                    connection = new SqlConnection(_connectionStr);
                    connection.Open();
                }
            }
        }

        public QueryFactory()
        {
            ConnectionString = ConfigurationManager.ConnectionStrings["EMS_Database"]?.ConnectionString;
        }

        /// <summary>
        /// Creates a prepared query from a string and a list of parameters.
        /// The parameters in the query must be the index of each parameter (ie @0, @1, ... @n)
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public SqlCommand CreateQuery(string query, params object[] args)
        {
            SqlCommand cmd = new SqlCommand(query, connection);

            for (int i = 0; i < args.Length; i++)
            {
                SqlParameter param = cmd.CreateParameter();
                param.ParameterName = i.ToString();
                param.Value = args[i] ?? DBNull.Value;
                cmd.Parameters.Add(param);
            }
            
            return cmd;
        }
        
        public SqlCommand CreateProcedureCall(string name, Dictionary<string, object> values)
        {
            SqlCommand cmd = new SqlCommand(name, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            foreach (string key in values.Keys)
            {
                SqlParameter param = cmd.CreateParameter();
                param.ParameterName = key;
                param.Value = values[key] ?? DBNull.Value;
                cmd.Parameters.Add(param);
            }
            
            cmd.Prepare();
            return cmd;
        }
        
        public void Close()
        {
            connection?.Close();
        }
    }

    public abstract class DatabaseRowFactoryBase<T>
    {

        protected readonly QueryFactory queryFactory;
        
        protected bool ignoreExtras = true;
        
        public DatabaseRowFactoryBase(QueryFactory queryFactory)
        {
            this.queryFactory = queryFactory;
        }

        protected T Find(string query, params object[] args)
        {
            using (SqlCommand cmd = queryFactory.CreateQuery(query, args))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!ignoreExtras && reader.RecordsAffected > 1)
                    {
                        throw new ArgumentException("Too many rows returned, and ignoreExtras == false");
                    }
                    
                    return reader.Read() ? CreateObject(reader) : default(T);
                }
            }
        }
        
        /// <summary>
        /// Finds many items.
        /// </summary>
        /// <param name="query">The query to run</param>
        /// <param name="args">The arguments to the query</param>
        /// <returns>The list of all rows returned</returns>
        protected List<T> FindMany(string query, params object[] args)
        {
            using (SqlCommand cmd = queryFactory.CreateQuery(query, args))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    List<T> objs = new List<T>();

                    while(reader.Read())
                    {
                        objs.Add(CreateObject(reader));
                    }
                    
                    return objs;
                }
            }
        }
        
        protected abstract T CreateObject(SqlDataReader reader);
    }
    
    public class DatabaseRowObjectAdapter
    {
        /// <summary>
        /// Fills an object with data from an SQL row.
        /// </summary>
        /// <typeparam name="T">The type of the object to fill</typeparam>
        /// <param name="obj">The object to fill</param>
        /// <param name="reader">The SQL reader to get data from</param>
        /// <param name="ignoreCase">Ignore case when checking column and field names</param>
        /// <param name="errorOnInvalidFields">Throw an exception when a field with no matching column is found</param>
        /// <exception cref="InvalidOperationException">When the object has a field with no matching column,
        /// and <code>errorOnInvalidFields</code> is true</exception>
        /// <returns>obj</returns>
        public static T Fill<T>(T obj, SqlDataReader reader, bool ignoreCase = true, bool errorOnInvalidFields = false)
        {
            foreach(FieldInfo field in obj.GetType().GetFields())
            {
                // Try to find a 'SQLColumnBinding' on the field, but if not found, just use the field name
                string name = field.GetCustomAttribute<SQLColumnBinding>()?.ColumnName ?? field.Name;
                
                int column = FindColumn(name, reader, ignoreCase);

                if(column == -1 && errorOnInvalidFields)
                {
                    throw new InvalidOperationException("Field " + field.Name + " has no matching column");
                }

                if(column != -1)
                {
                    SetField(field, ref obj, reader, column);
                }
            }

            return obj;
        }

        private static int FindColumn(string name, SqlDataReader reader, bool ignoreCase)
        {
            for(int i = 0; i < reader.FieldCount; i++)
            {
                if(name.Equals(reader.GetName(i), ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
                {
                    return i;
                }
            }

            return -1;
        }

        private static void SetField<T>(FieldInfo field, ref T obj, SqlDataReader reader, int column)
        {
            if(reader.IsDBNull(column))
            {
                field.SetValue(obj, null);
                return;
            }

            if(field.GetCustomAttribute<EnumSQLColumn>() != null)
            {
                SetPEnumField(field, obj, reader.GetString(column));
                return;
            }

            Type colType = reader.GetFieldType(column);
            if(colType == typeof(float))
            {
                field.SetValue(obj, reader.GetFloat(column));
            }
            else if(colType == typeof(double))
            {
                field.SetValue(obj, reader.GetDouble(column));
            }
            else if(colType == typeof(int))
            {
                field.SetValue(obj, reader.GetInt32(column));
            }
            else if(colType == typeof(string))
            {
                field.SetValue(obj, reader.GetString(column));
            }
            else if(colType == typeof(char))
            {
                field.SetValue(obj, reader.GetChar(column));
            }
            else if(colType == typeof(DateTime))
            {
                field.SetValue(obj, reader.GetDateTime(column));
            }
            else if (colType == typeof(bool))
            {
                field.SetValue(obj, reader.GetBoolean(column));
            }
            else
            {
                throw new InvalidOperationException("Unknown data type " + colType);
            }
        }

        private static T SetPEnumField<T>(FieldInfo field, T obj, string data)
        {
            FieldInfo[] fields = field.FieldType.GetFields(BindingFlags.Static);

            foreach(FieldInfo f in fields)
            {
                if(f.Name.Equals(data))
                {
                    field.SetValue(obj, f.GetValue(null));
                    break;
                }
            }

            throw new Exception("No constant " + data + " in object " + field.FieldType);
        }
        
    }

    /// <summary>
    /// When used on a field, allows <see cref="DatabaseRowObjectAdapter.Fill{T}(T, SqlDataReader, string, string, bool, bool)"/>
    /// to copy the data from 'ColumnName' into the field.
    /// </summary>
    public class SQLColumnBinding: Attribute
    {
        public readonly string ColumnName;
        public SQLColumnBinding(string columnName)
        {
            ColumnName = columnName;
        }
    }

    /// <summary>
    /// Marks a bound sql column as a pseudo-enum type. A pseudo-enum is a class with multiple constants.
    /// The constants are gotten using the attached field's datatype, and the sql's value (as a case-sensitive name).
    /// </summary>
    public class EnumSQLColumn: Attribute {}

    /// <summary>
    /// Builds an sql where clause
    /// </summary>
    public class SqlWhereClauseBuilder: List<(string, object)>
    {
        public string ClauseSeparator { get; set; } = " AND ";
        
        /// <summary>
        /// Tries to add a condition if the given value doesn't equal the illegalValue.
        /// The variable (@...) in the clause must be replaced with '{0}'
        /// </summary>
        /// <remarks>
        /// Example clause:
        /// <code>
        /// Table.Column <> {0}
        /// </code>
        /// 
        /// Another example:
        /// <code>
        /// Table.Column1 = {0} OR Table.Column2 = {0}
        /// </code>
        /// 
        /// </remarks>
        public void TryAddCondition<T>(string clause, T value, T illegalValue = default(T))
        {
            if(!EqualityComparer<T>.Default.Equals(value, illegalValue))
            {
                Add((clause, value));
            }
        }
        
        /// <summary>
        /// Always adds a condition
        /// </summary>
        /// <param name="clause">The clause (see TryAddCondition)</param>
        /// <param name="value">The value</param>
        public void AddCondition(string clause, object value)
        {
            Add((clause, value));
        }
        
        /// <summary>
        /// Adds a condition, throwing an exception if it is invalid.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="clause">The where-clause segment</param>
        /// <param name="value">The where-clause value</param>
        /// <param name="argname">The argument's name in the error message</param>
        /// <param name="illegalValue">The illegal value</param>
        /// <exception cref="ArgumentException">If the value equals the illegalValue</exception>
        public void AddManditoryCondition<T>(string clause, T value, string argname, T illegalValue = default(T))
        {
            if (!EqualityComparer<T>.Default.Equals(value, illegalValue))
            {
                Add((clause, value));
            }
            else
            {
                throw new ArgumentException(string.Format("{0} is not valid (was '{1}')", argname, value));
            }
        }

        /// <summary>
        /// Converts this predicate to a sql-compatible string.
        /// </summary>
        /// <param name="numberingStart">The number to set the first parameter to.</param>
        /// <returns>The query</returns>
        public (string, object[]) Get(int numberingStart = 0)
        {
            StringBuilder str = new StringBuilder();

            object[] objs = new object[this.Count];

            for(int i = 0; i < this.Count; i++)
            {
                (string, object) clause = this[i];

                str.Append("(").Append(string.Format(clause.Item1, "@" + (numberingStart + i))).Append(")");
                objs[i] = clause.Item2;

                if(i < this.Count - 1)
                {
                    str.Append(ClauseSeparator);
                }
            }

            return (str.ToString(), objs);
        }
        
    }

    /// <summary>
    /// Builds an sql insert query for a single row
    /// </summary>
    class SqlInsertBuilder : List<(string, object)>
    {
        /// <summary>
        /// The table to insert into.
        /// </summary>
        public string Table { get; set; }
        
        /// <summary>
        /// Tries to add a column, doing nothing if the parameter is invalid
        /// </summary>
        /// <param name="colName">The column's name</param>
        /// <param name="colValue">The column's value</param>
        /// <param name="illegalValue">The illegal value (null for objects)</param>
        public void TryAddColumn<T>(string colName, T colValue, T illegalValue = default(T))
        {
            if (!EqualityComparer<T>.Default.Equals(colValue, illegalValue))
            {
                Add((colName, colValue));
            }
        }

        /// <summary>
        /// Tries to add a column, failing if the parameter is invalid
        /// </summary>
        /// <param name="colName">The column's name</param>
        /// <param name="colValue">The column's value</param>
        /// <param name="illegalValue">The illegal value (null for objects)</param>
        /// <exception cref="ArgumentException">If the column value is invalid</exception>
        public void AddManditoryColumn<T>(string colName, T colValue, T illegalValue = default(T))
        {
            if (!EqualityComparer<T>.Default.Equals(colValue, illegalValue))
            {
                Add((colName, colValue));
            }
            else
            {
                throw new ArgumentException(string.Format("Column {0} is an invalid value", colName));
            }
        }

        /// <summary>
        /// Builds the query and returns it.
        /// </summary>
        public string GetQuery()
        {
            string columns = "", values = "";
            
            for(int i = 0; i < Count; i++)
            {
                columns += (i > 0 ? ", " : "") + "[" + this[i].Item1 + "]";
                values += (i > 0 ? ", " : "") + "@" + i;
            }

            return string.Format("INSERT INTO [{0}] ({1}) VALUES ({2})", Table, columns, values);
        }

        /// <summary>
        /// Gathers the column's values and returns them in an array.
        /// </summary>
        public object GetValues()
        {
            object[] objs = new object[this.Count];

            for(int i = 0; i < Count; i++)
            {
                objs[i] = this[i].Item2;
            }

            return objs;
        }

    }
}
