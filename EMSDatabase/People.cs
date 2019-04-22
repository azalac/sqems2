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
        public HCVStatusFactory StatusFactory {get;}

        public PeopleFactory(QueryFactory queryFactory) : base(queryFactory)
        {
            StatusFactory = new HCVStatusFactory(queryFactory);
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
            return DatabaseRowObjectAdapter.Fill(new Person(queryFactory, StatusFactory), reader);
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
    
    public class HCVStatusFactory : DatabaseRowFactoryBase<HCVStatus>
    {
        public HCVStatus NotValidated { get => FindByCode("NOHCV"); }
        public HCVStatus Valid { get => FindByCode("VALID"); }
        public HCVStatus VersionCode { get => FindByCode("VCODE"); }
        public HCVStatus Punko { get => FindByCode("PUNKO"); }

        public HCVStatusFactory(QueryFactory queryFactory) : base(queryFactory)
        {
        }

        public HCVStatus FindById(int ID)
        {
            return Find("SELECT * FROM HCVStatus WHERE ID = @0", ID);
        }

        public HCVStatus FindByCode(string CodeName)
        {
            return Find("SELECT * FROM HCVStatus WHERE CodeName LIKE @0", CodeName);
        }
        
        protected override HCVStatus CreateObject(SqlDataReader reader)
        {
            return DatabaseRowObjectAdapter.Fill(new HCVStatus(), reader);
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

        public string HCN => _HCN;
        public string lastName => _lastName;
        public string firstName => _firstName;
        public string mInitial => _mInitial;

        public int sex => _sex;

        public DateTime dateOfBirth => _dateOfBirth;

        public HCVStatus HCVStatus { get => GetHCVStatus(); set => SetHCVStatus(value); }

        [SQLColumnBinding("PersonID")]
        public readonly int ID;

        [SQLColumnBinding("HCN")]
        private string _HCN;

        [SQLColumnBinding("lastName")]
        private string _lastName;

        [SQLColumnBinding("firstName")]
        private string _firstName;

        [SQLColumnBinding("mInitial")]
        private string _mInitial;

        [SQLColumnBinding("sex")]
        private int _sex;

        [SQLColumnBinding("dateBirth")]
        private DateTime _dateOfBirth;
        
        private QueryFactory queryFactory;
        private HCVStatusFactory statusFactory;

        public Person(QueryFactory query, HCVStatusFactory statusFactory)
        {
            queryFactory = query;
            this.statusFactory = statusFactory;
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

        /// <summary>
        /// Gets the current HCV status, or 'NotValidated' if none set
        /// </summary>
        public HCVStatus GetHCVStatus()
        {
            using (SqlCommand cmd = queryFactory.CreateQuery("SELECT HCVStatusID FROM People WHERE PersonID = @0", ID))
            {
                int? statusid = cmd.ExecuteScalar() as int?;

                if (statusid == null)
                {
                    return statusFactory.NotValidated;
                }
                else
                {
                    return statusFactory.FindById(statusid.Value);
                }
            }
        }

        /// <summary>
        /// Sets the hcv's last validation status
        /// </summary>
        /// <param name="status">The last validation status</param>
        /// <exception cref="ArgumentException">If the HCVStatus' ID is invalid (should never happen unless invalid usage)</exception>
        public void SetHCVStatus(HCVStatus status)
        {
            using (SqlCommand cmd = queryFactory.CreateQuery("UPDATE People SET HCVStatusID = @0 WHERE PersonID = @1", status.ID, ID))
            {
                if (cmd.ExecuteNonQuery() != 1)
                {
                    throw new ArgumentException("Invalid status id " + status.ID);
                }
            }
        }

        /// <summary>
        /// Updates this person's values. Set a parameter to null to ignore it
        /// </summary>
        /// <returns>true if the person was successfully updated, false otherwise</returns>
        public bool UpdateValues(string HCN, string lastName, string firstName, string mInitial, int sex, DateTime dateOfBirth)
        {
            SqlWhereClauseBuilder selector = new SqlWhereClauseBuilder();

            selector.AddManditoryCondition("PersonID = {0}", ID, "PersonID");

            SqlUpdateBuilder updater = new SqlUpdateBuilder()
            {
                Table = "People",
                RowSelector = selector
            };

            if (updater.TryAddColumn("HCN", HCN))
            {
                _HCN = HCN;
            }

            if(updater.TryAddColumn("lastName", lastName))
            {
                _lastName = lastName;
            }
            
            if(updater.TryAddColumn("firstName", firstName))
            {
                _firstName = firstName;
            }

            if (updater.TryAddColumn("mInitial", mInitial))
            {
                _mInitial = mInitial;
            }

            if(updater.TryAddColumn("sex", sex))
            {
                _sex = sex;
            }

            if(updater.TryAddColumn("dateOfBirth", dateOfBirth))
            {
                _dateOfBirth = dateOfBirth;
            }

            using (SqlCommand cmd = queryFactory.CreateQuery(updater.GetQuery(), updater.GetValues()))
            {
                return cmd.ExecuteNonQuery() == 1;
            }

        }

        /// <summary>
        /// Returns an editable version of this person
        /// </summary>
        public EditablePerson StartEditing()
        {
            return new EditablePerson(this);
        }
    }

    /// <summary>
    /// An editable version of a person. Changes must be commited after fields
    /// are updated (should be after all operations). Null fields are not updated.
    /// DO NOT USE FOR DATA STORAGE. PROPERTIES ARE WRITE ONLY
    /// </summary>
    public class EditablePerson
    {

        public readonly int ID;

        public string HCN { private get; set; }
        public string lastName { private get; set; }
        public string firstName { private get; set; }
        public string mInitial { private get; set; }
        public int sex { private get; set; }
        public DateTime DateOfBirth { private get; set; }
        
        private Person owner;

        public EditablePerson(Person owner)
        {
            ID = owner.ID;

            this.owner = owner;
        }

        public void CommitChanges()
        {
            owner.UpdateValues(HCN, lastName, firstName, mInitial, sex, DateOfBirth);
        }

    }

    /// <summary>
    /// Represents a health card validation status
    /// </summary>
    public class HCVStatus
    {
        public readonly int ID;

        public readonly string CodeName, FullName;

        /// <summary>
        /// True if this code is an error code (ie HCN is invalid)
        /// </summary>
        public bool IsError;
    }
}
