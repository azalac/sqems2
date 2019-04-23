﻿using System;
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
                int? id = Create(Address1, Address2, City, Province, PhoneNumber);

                // if the create was successful, get the new household
                if (id != null)
                {
                    household = Find(id.Value);
                }
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
            
            return Find("SELECT * FROM Household WHERE " + predicate.GetQuery(), predicate.GetValues());
        }

        /// <summary>
        /// Creates a household with the given information, does not return it.
        /// </summary>
        /// <returns>The ID of the created household</returns>
        private int? Create(string Address1 = null, string Address2 = null, string City = null,
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

            using (SqlCommand cmd = queryFactory.CreateQuery(insertBuilder.GetQuery() + ";SELECT SCOPE_IDENTITY()", insertBuilder.GetValues()))
            {
                return Convert.ToInt32(cmd.ExecuteScalar());
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

        public int ID => _ID;

        public string AddressLine1 { get => _AddressLine1; }
        public string AddressLine2 { get => _AddressLine2; }
        public string City { get => _City; }
        public string Province { get => _Province; }
        public string PhoneNumber { get => _PhoneNumber; }

        [SQLColumnBinding("ID")]
        private readonly int _ID;

        [SQLColumnBinding("AddressLine1")]
        private string _AddressLine1;
        [SQLColumnBinding("AddressLine2")]
        private string _AddressLine2;
        [SQLColumnBinding("City")]
        private string _City;
        [SQLColumnBinding("Province")]
        private string _Province;

        [SQLColumnBinding("numPhone")]
        private string _PhoneNumber;
        
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
            using (SqlCommand deleter = queryFactory.CreateQuery("DELETE FROM HouseHead WHERE HouseID = @0", _ID))
            {
                deleter.ExecuteNonQuery();
            }

            if (hohId != null)
            {
                // Inserts a new house head if it's not being removed
                using (SqlCommand cmd = queryFactory.CreateQuery("INSERT INTO HouseHead (HouseID, PersonID) VALUES (@0, @1)", _ID, hohId))
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
            using (SqlCommand cmd = queryFactory.CreateQuery("SELECT PersonID FROM HouseHead WHERE HouseID = @0", _ID))
            {
                return (int)cmd.ExecuteScalar();
            }
        }
        
        /// <summary>
        /// Updates this household's values. Set a parameter to null to ignore it
        /// </summary>
        /// <returns>true if the household was successfully updated, false otherwise</returns>
        public bool UpdateValues(string AddressLine1, string AddressLine2, string City, string Province, string PhoneNumber)
        {
            SqlWhereClauseBuilder selector = new SqlWhereClauseBuilder();

            selector.AddManditoryCondition("ID = {0}", _ID, "ID");

            SqlUpdateBuilder updater = new SqlUpdateBuilder()
            {
                Table = "Household",
                RowSelector = selector
            };

            if(updater.TryAddColumn("AddressLine1", AddressLine1))
            {
                this._AddressLine1 = AddressLine1;
            }

            if(updater.TryAddColumn("AddressLine2", AddressLine2))
            {
                this._AddressLine2 = AddressLine2;
            }

            if(updater.TryAddColumn("City", City))
            {
                this._City = City;
            }

            if(updater.TryAddColumn("Province", Province))
            {
                this._Province = Province;
            }

            if(updater.TryAddColumn("numPhone", PhoneNumber))
            {
                this._PhoneNumber = PhoneNumber;
            }

            using (SqlCommand cmd = queryFactory.CreateQuery(updater.GetQuery(), updater.GetValues()))
            {
                return cmd.ExecuteNonQuery() == 1;
            }

        }

        /// <summary>
        /// Returns an editable version of this household
        /// </summary>
        public EditableHousehold StartEditing()
        {
            return new EditableHousehold(this);
        }

    }

    /// <summary>
    /// An editable version of a household. Changes must be commited after fields
    /// are updated (should be after all operations). Null fields are not updated.
    /// DO NOT USE FOR DATA STORAGE. PROPERTIES ARE WRITE ONLY
    /// </summary>
    public class EditableHousehold
    {
        public readonly int ID;

        public string AddressLine1 { private get; set; }
        public string AddressLine2 { private get; set; }
        public string City { private get; set; }
        public string Province { private get; set; }
        public string PhoneNumber { private get; set; }

        private readonly Household household;

        public EditableHousehold(Household household)
        {
            ID = household.ID;

            this.household = household;
        }
        
        /// <summary>
        /// Updates this household's values
        /// </summary>
        public void CommitChanges()
        {
            household.UpdateValues(AddressLine1, AddressLine2, City, Province, PhoneNumber);
        }

    }

}
