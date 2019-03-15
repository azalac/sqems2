using EMSDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing
{
    class Program
    {
        static void Main(string[] args)
        {
            QueryFactory query = new QueryFactory();

            BillingCodeFactory factory = new BillingCodeFactory(query);

            List<BillingCode> codes = factory.Similar("Z");
            
            foreach(var a in codes)
            {
                Console.WriteLine(a);
            }

        }
    }
}
