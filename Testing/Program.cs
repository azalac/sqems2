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

            PeopleFactory peopleFactory = new PeopleFactory(query);
            GenderFactory genderFactory = new GenderFactory(query);
            AppointmentFactory appointmentFactory = new AppointmentFactory(query, peopleFactory);
            BillingCodeFactory billingCodeFactory = new BillingCodeFactory(query);
            BillableProcedureFactory bpFactory = new BillableProcedureFactory(query);
            HouseholdFactory householdFactory = new HouseholdFactory(query);

            Appointment appointment = appointmentFactory.Find(2);

            bpFactory.SetBillableProcedures(appointment.Patient, new List<BillingCode> { billingCodeFactory.Find("A184") });

            bpFactory.GetBillableProcedures(appointment.Patient).First().Status = bpFactory.StatusFactory.ContactMoH;

            List<Appointment> apts = appointmentFactory.FindWithTimes();

            foreach(Appointment apt in apts)
            {
                Console.WriteLine(apt);
                foreach(BillableProcedure bp in bpFactory.GetBillableProcedures(apt.Patient))
                {
                    Console.WriteLine("\t" + bp);
                }
            }

        }
    }
}
