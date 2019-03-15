using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace EMSDatabase
{
    public class HouseholdFactory : DatabaseRowFactoryBase<Household, int>
    {
        public HouseholdFactory(QueryFactory queryFactory) : base(queryFactory)
        {
            ignoreExtras = false;
        }

        public Household Find(int ID)
        {
            return Find("SELECT * FROM Household WHERE ID = @0", ID);
        }

        public Household FindOrCreate(string Address1 = null, string Address2 = null, string City = null,
            string Province = null, string PhoneNumber = null)
        {
            DynamicSQLQuery predicate = new DynamicSQLQuery();

            predicate.TryAddCondition("AddressLine1 = {0}", Address1);
            predicate.TryAddCondition("AddressLine2 = {0}", Address2);
            predicate.TryAddCondition("City = {0}", City);
            predicate.TryAddCondition("Province = {0}", Province);
            predicate.TryAddCondition("PhoneNumber = {0}", PhoneNumber);

            var (predicate_str, predicate_obj) = predicate.Get();

            Household household = Find(predicate_str, predicate_obj);

            if(household == null)
            {
                Create(Address1, Address2, City, Province, PhoneNumber);
                household = FindImpl(Address1, Address2, City, Province, PhoneNumber);
            }

            return household;
        }
        
        private void Create(string Address1 = null, string Address2 = null, string City = null,
            string Province = null, string PhoneNumber = null)
        {
            using(MySqlCommand cmd = queryFactory.CreateQuery("INSERT INTO Household (addressLine1, addressLine2, city, province, numPhone) VALUES (@0, @1, @2, @3, @4)",
                Address1, Address2, City, Province, PhoneNumber))
            {
                int affected = cmd.ExecuteNonQuery();
            }
        }

        private Household FindImpl(string Address1 = null, string Address2 = null, string City = null,
            string Province = null, string PhoneNumber = null)
        {

            DynamicSQLQuery predicate = new DynamicSQLQuery();

            predicate.TryAddCondition("AddressLine1 = {0}", Address1);
            predicate.TryAddCondition("AddressLine2 = {0}", Address2);
            predicate.TryAddCondition("City = {0}", City);
            predicate.TryAddCondition("Province = {0}", Province);
            predicate.TryAddCondition("PhoneNumber = {0}", PhoneNumber);

            var (predicate_str, predicate_obj) = predicate.Get();
            
            return Find("SELECT addressLine1, addressLine2, city, province, numPhone, headOfHouse FROM Household WHERE " + predicate_str, predicate_obj);
        }

        protected override Household CreateObject(MySqlDataReader reader)
        {
            return DatabaseRowObjectAdapter.Fill(new Household(queryFactory), reader);
        }
    }

    public class Household
    {
        public int ID, HeadOfHouse;
        public string AddressLine1, AddressLine2, City, Province, PhoneNumber;

        private QueryFactory queryFactory;

        public Household(QueryFactory queryFactory)
        {
            this.queryFactory = queryFactory;
        }

        public void SetHeadOfHouse(int hohId)
        {
            using (MySqlCommand cmd = queryFactory.CreateQuery("CALL SetHouseholdHead(@0, @1)", ID, hohId))
            {
                int status = (int)cmd.ExecuteScalar();

                if(status == 0)
                {
                    throw new ArgumentException("Person with ID " + hohId + " does not exist");
                }
            }
        }
    }
}
