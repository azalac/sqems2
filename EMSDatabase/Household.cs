using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace EMSDatabase
{
    /// <summary>
    /// Finds and creates households
    /// </summary>
    public class HouseholdFactory : DatabaseRowFactoryBase<Household>
    {
        public HouseholdFactory(QueryFactory queryFactory) : base(queryFactory)
        {
            ignoreExtras = false;
        }

        /// <summary>
        /// Finds a house by its id
        /// </summary>
        public Household Find(int ID)
        {
            return Find("SELECT * FROM Household WHERE ID = @0", ID);
        }

        /// <summary>
        /// Finds or creates a house with the given information
        /// </summary>
        /// <returns></returns>
        public Household FindOrCreate(string Address1 = null, string Address2 = null, string City = null,
            string Province = null, string PhoneNumber = null)
        {
            Household household = Find(Address1, Address2, City, Province, PhoneNumber);

            if(household == null)
            {
                Create(Address1, Address2, City, Province, PhoneNumber);
                household = Find(Address1, Address2, City, Province, PhoneNumber);
            }

            return household;
        }
        
        /// <summary>
        /// Finds the first household with the given information
        /// </summary>
        public Household Find(string Address1 = null, string Address2 = null, string City = null,
            string Province = null, string PhoneNumber = null)
        {

            SqlWhereClauseBuilder predicate = new SqlWhereClauseBuilder();

            predicate.TryAddCondition("AddressLine1 = {0}", Address1);
            predicate.TryAddCondition("AddressLine2 = {0}", Address2);
            predicate.TryAddCondition("City = {0}", City);
            predicate.TryAddCondition("Province = {0}", Province);
            predicate.TryAddCondition("numPhone = {0}", PhoneNumber);

            var (predicate_str, predicate_obj) = predicate.Get();
            
            return Find("SELECT * FROM Household WHERE " + predicate_str, predicate_obj);
        }

        /// <summary>
        /// Creates a household with the given information, does not return it.
        /// </summary>
        private void Create(string Address1 = null, string Address2 = null, string City = null,
            string Province = null, string PhoneNumber = null)
        {
            SqlInsertBuilder insertBuilder = new SqlInsertBuilder()
            {
                Table = "Household"
            };

            insertBuilder.TryAddColumn("addressLine1", Address1);
            insertBuilder.TryAddColumn("addressLine2", Address2);
            insertBuilder.TryAddColumn("city", City);
            insertBuilder.TryAddColumn("province", Province);
            insertBuilder.TryAddColumn("numPhone", PhoneNumber);

            using (SqlCommand cmd = queryFactory.CreateQuery(insertBuilder.GetQuery(), insertBuilder.GetValues()))
            {
                cmd.ExecuteNonQuery();
            }
        }

        protected override Household CreateObject(SqlDataReader reader)
        {
            return DatabaseRowObjectAdapter.Fill(new Household(queryFactory), reader);
        }
    }

    /// <summary>
    /// Represents a household
    /// </summary>
    public class Household
    {
        /// <summary>
        /// Gets or sets the head of house (Person ID)
        /// </summary>
        public int? HeadOfHouse { get => GetHeadOfHouse(); set => SetHeadOfHouse(value); }

        public readonly int ID;

        public readonly string AddressLine1, AddressLine2, City, Province;

        [SQLColumnBinding("numPhone")]
        public readonly string PhoneNumber;
        
        private QueryFactory queryFactory;

        public Household(QueryFactory queryFactory)
        {
            this.queryFactory = queryFactory;
        }

        /// <summary>
        /// Sets the head of house.
        /// </summary>
        /// <param name="hohId">Sets the head of house, with null being none</param>
        /// <exception cref="ArgumentException">If the given person id is not found</exception>
        public void SetHeadOfHouse(int? hohId)
        {
            // Delete the previous house head (if there was one)
            using (SqlCommand deleter = queryFactory.CreateQuery("DELETE FROM HouseHead WHERE HouseID = @0", ID))
            {
                deleter.ExecuteNonQuery();
            }

            if (hohId != null)
            {
                // Inserts a new house head if it's not being removed
                using (SqlCommand cmd = queryFactory.CreateQuery("INSERT INTO HouseHead (HouseID, PersonID) VALUES (@0, @1)", ID, hohId))
                {
                    if (cmd.ExecuteNonQuery() == 0)
                    {
                        throw new ArgumentException("Invalid head of house id " + hohId);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the head of house
        /// </summary>
        public int GetHeadOfHouse()
        {
            using (SqlCommand cmd = queryFactory.CreateQuery("SELECT PersonID FROM HouseHead WHERE HouseID = @0", ID))
            {
                return (int)cmd.ExecuteScalar();
            }
        }
    }
}
