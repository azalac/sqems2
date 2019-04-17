using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace EMSDatabase
{
    /// <summary>
    /// Gets and creates people in the database
    /// </summary>
    public class PeopleFactory : DatabaseRowFactoryBase<Person>
    {
        public PeopleFactory(QueryFactory queryFactory) : base(queryFactory)
        {
        }

        /// <summary>
        /// Creates a person. All parameters are manditory except HouseID.
        /// </summary>
        /// <returns>The newly created person, or null if one could not be created</returns>
        public Person Create(string firstName, char middleInitial, string lastName, string HCN,
            DateTime DateOfBirth, int sex, int? HouseID = null)
        {
            SqlInsertBuilder insertBuilder = new SqlInsertBuilder()
            {
                Table = "People"
            };

            insertBuilder.AddManditoryColumn("firstName", firstName);
            insertBuilder.AddManditoryColumn("lastName", lastName);
            insertBuilder.AddManditoryColumn("mInitial", middleInitial);
            insertBuilder.AddManditoryColumn("HCN", HCN);
            insertBuilder.AddManditoryColumn("dateBirth", DateOfBirth);
            insertBuilder.AddManditoryColumn("sex", sex);
            insertBuilder.TryAddColumn("HouseID", HouseID);
            
            using (SqlCommand cmd = queryFactory.CreateQuery(insertBuilder.GetQuery(), insertBuilder.GetValues()))
            {
                if(cmd.ExecuteNonQuery() != 1)
                {
                    return null;
                }

                return Find(firstName, lastName, middleInitial, HCN).FirstOrDefault();
            }
        }

        /// <summary>
        /// Finds a person by their id
        /// </summary>
        public Person Find(int ID)
        {
            return Find("SELECT * FROM People WHERE PersonID = @0", ID);
        }

        /// <summary>
        /// Finds a list of people by their name or HCN 
        /// </summary>
        public List<Person> Find(string firstName = null, string lastName = null, char? mInitial = null, string HCN = null)
        {
            SqlWhereClauseBuilder where = new SqlWhereClauseBuilder();

            where.TryAddCondition("firstName = {0}", firstName);
            where.TryAddCondition("lastName = {0}", lastName);
            where.TryAddCondition("mInitial = {0}", mInitial);
            where.TryAddCondition("HCN = {0}", HCN);

            var (where_query, where_objs) = where.Get();

            return base.FindMany("SELECT * FROM People WHERE" + where_query, where_objs);
        }

        protected override Person CreateObject(SqlDataReader reader)
        {
            return DatabaseRowObjectAdapter.Fill(new Person(queryFactory), reader);
        }
    }

    /// <summary>
    /// Reads from and Writes to the 'Sexes' table.
    /// </summary>
    public class GenderFactory : DatabaseRowFactoryBase<char>
    {
        public GenderFactory(QueryFactory queryFactory) : base(queryFactory)
        {

        }

        /// <summary>
        /// Finds a sex's name by its id
        /// </summary>
        public char Find(int id)
        {
            return base.Find("SELECT Name FROM Sexes WHERE ID = @0", id);
        }

        /// <summary>
        /// Finds a sex's id by its name
        /// </summary>
        public int ForName(char name)
        {
            // copy and pasted from base.Find with slight modifications
            using (SqlCommand cmd = queryFactory.CreateQuery("SELECT ID FROM Sexes WHERE [Name] = @0", name.ToString()))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!ignoreExtras && reader.RecordsAffected > 1)
                    {
                        throw new ArgumentException("Too many rows returned, and ignoreExtras == false");
                    }

                    return reader.Read() ? reader.GetInt32(0) : 0;
                }
            }
        }

        protected override char CreateObject(SqlDataReader reader)
        {
            return reader.GetString(0)[0];
        }
    }

    /// <summary>
    /// Represents a person in the database
    /// </summary>
    public class Person
    {
        /// <summary>
        /// Gets or sets the household for this person.
        /// </summary>
        public int? HouseholdID { get => GetHousehold(); set => SetHousehold(value); }

        [SQLColumnBinding("PersonID")]
        public int ID;

        public string HCN, lastName, firstName, mInitial;

        public int sex;

        [SQLColumnBinding("dateBirth")]
        public DateTime dateOfBirth;
        
        private QueryFactory queryFactory;

        public Person(QueryFactory query)
        {
            queryFactory = query;
        }

        public override string ToString()
        {
            return string.Format("Person[{0}, {1} {2}, {3}, {4}]", ID, firstName, lastName, sex, dateOfBirth);
        }

        /// <summary>
        /// Gets this person's household
        /// </summary>
        public int? GetHousehold()
        {
            using (SqlCommand cmd = queryFactory.CreateQuery("SELECT HouseID FROM People WHERE PersonID = @0", ID))
            {
                return cmd.ExecuteScalar() as int?;
            }
        }

        /// <summary>
        /// Sets the household of this person.
        /// </summary>
        /// <param name="HouseholdID">The household id</param>
        /// <exception cref="ArgumentException">If the house id could not be set</exception>
        public void SetHousehold(int? HouseholdID)
        {
            using (SqlCommand cmd = queryFactory.CreateQuery("UPDATE People SET HouseID = @0 WHERE PersonID = @1", HouseholdID, ID))
            {
                if (cmd.ExecuteNonQuery() != 1)
                {
                    throw new ArgumentException("Invalid house id " + HouseholdID);
                }
            }
        }
    }
}
