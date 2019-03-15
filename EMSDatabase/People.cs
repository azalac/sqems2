using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace EMSDatabase
{
    public class PeopleFactory : DatabaseRowFactoryBase<Person, int>
    {
        public PeopleFactory(QueryFactory queryFactory) : base(queryFactory)
        {
        }

        public Person Find(string firstName, string lastName = null)
        {
            if (lastName != null)
            {
                return Find("SELECT * FROM People WHERE firstName = @0 AND lastName = @1", firstName, lastName);
            }
            else
            {
                return Find("SELECT * FROM People WHERE firstName = @0", firstName);
            }
        }

        protected override Person CreateObject(MySqlDataReader reader)
        {
            return DatabaseRowObjectAdapter.Fill(new Person(), reader);
        }
    }

    public class Person
    {
        [SQLColumnBinding("PersonID")]
        public int ID;

        public string HCN, lastName, firstName, mInitial, sex;

        [SQLColumnBinding("dateBirth")]
        public DateTime dateOfBirth;

        [SQLColumnBinding("HouseID")]
        public int HouseholdID;
    }
}
