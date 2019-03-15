using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EMSDatabase
{
    public class QueryFactory
    {
        private string _connectionStr;

        private MySqlConnection connection;

        public string ConnectionString
        {
            get => _connectionStr;

            set
            {
                _connectionStr = value;
                connection?.Close();

                if (_connectionStr != null)
                {
                    connection = new MySqlConnection(_connectionStr);
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
        public MySqlCommand CreateQuery(string query, params object[] args)
        {
            MySqlCommand cmd = new MySqlCommand(query, connection);

            for (int i = 0; i < args.Length; i++)
            {
                cmd.Parameters.Add(new MySqlParameter(i.ToString(), args[i]));
            }

            cmd.Prepare();
            return cmd;
        }
        
        public void Close()
        {
            connection?.Close();
        }
    }

    public abstract class DatabaseRowFactoryBase<T, Q>
    {

        protected readonly QueryFactory queryFactory;
        
        protected bool ignoreExtras = true;
        
        public DatabaseRowFactoryBase(QueryFactory queryFactory)
        {
            this.queryFactory = queryFactory;
        }

        protected T Find(string query, params object[] args)
        {
            using (MySqlCommand cmd = queryFactory.CreateQuery(query, args))
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (ignoreExtras && reader.RecordsAffected > 1)
                    {
                        throw new ArgumentException("Too many rows returned, and ignoreExtras == true");
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
            using (MySqlCommand cmd = queryFactory.CreateQuery(query, args))
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
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
        
        protected abstract T CreateObject(MySqlDataReader reader);
    }
    
    public class DatabaseRowObjectAdapter
    {
        /// <summary>
        /// Fills an object with data from an SQL row.
        /// </summary>
        /// <typeparam name="T">The type of the object to fill</typeparam>
        /// <param name="obj">The object to fill</param>
        /// <param name="reader">The SQL reader to get data from</param>
        /// <param name="objFieldPrefix">The prefix to add to the front of object field names</param>
        /// <param name="colFieldPrefix">The prefix to add to the front of SQL column names</param>
        /// <param name="ignoreCase">Ignore case when checking column and field names</param>
        /// <param name="errorOnInvalidFields">Throw an exception when a field with no matching column is found</param>
        /// <exception cref="InvalidOperationException">When the object has a field with no matching column,
        /// and <code>errorOnInvalidFields</code> is true</exception>
        /// <returns></returns>
        public static T Fill<T>(T obj, MySqlDataReader reader, bool ignoreCase = true, bool errorOnInvalidFields = false)
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
                    obj = SetField(field, obj, reader, column);
                }
            }

            return obj;
        }

        private static int FindColumn(string name, MySqlDataReader reader, bool ignoreCase)
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

        private static T SetField<T>(FieldInfo field, T obj, MySqlDataReader reader, int column)
        {
            if(reader.IsDBNull(column))
            {
                field.SetValue(obj, null);
                return obj;
            }

            if(field.GetCustomAttribute<EnumSQLColumn>() != null)
            {
                return SetPEnumField(field, obj, reader.GetString(column));
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
            else
            {
                throw new InvalidOperationException("Unknown data type " + colType);
            }

            return obj;
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
    /// When used on a field, allows <see cref="DatabaseRowObjectAdapter.Fill{T}(T, MySqlDataReader, string, string, bool, bool)"/>
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

    public class DynamicSQLQuery: List<(string, object)>
    {
        public string ClauseSeparator = " AND ";

        /// <summary>
        /// Tries to add a condition if the given value doesn't equal the illegalValue.
        /// The variable (@n) in the clause must be replaced with '{0}'
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
            if(!value.Equals(illegalValue))
            {
                Add((clause, value));
            }
        }
        
        public void AddCondition(string clause, object value)
        {
            Add((clause, value));
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
}
