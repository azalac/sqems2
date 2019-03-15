using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMSDatabase
{
    public class BillingCodeFactory : DatabaseRowFactoryBase<BillingCode, string>
    {
        public BillingCodeFactory(QueryFactory queryFactory) : base(queryFactory)
        {
        }

        public BillingCode Find(string code)
        {
            return Find(code, "SELECT * FROM MasterBillingCode WHERE Code = @0", code);
        }

        public List<BillingCode> Similar(string code, int max = 10)
        {
            return FindMany("SELECT * FROM MasterBillingCode WHERE Code LIKE CONCAT('%',@1,'%') LIMIT 0, @0", max, code);
        }

        protected override BillingCode CreateObject(MySqlDataReader reader)
        {
            return DatabaseRowObjectAdapter.Fill(new BillingCode(), reader);
        }
    }

    public class BillingCode
    {
        public readonly int ID;
        public readonly string Code;
        public readonly DateTime StartDate;
        public readonly double Price;

        public override string ToString()
        {
            return string.Format("{0}[{1}, {2}, ${3}]", Code, ID, StartDate.ToString("d"), Price);
        }
    }
}
