using EMSDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS2
{
    class Billing
    {
        private QueryFactory queryFactory = new QueryFactory();
        private PeopleFactory peopleFactory;
        private AppointmentFactory appointmentFactory;
        private HouseholdFactory householdFactory;
        private AppointmentPatientFactory appointmentPatientFactory;
        private GenderFactory genderFactory;
        private BillableProcedureFactory billableProcedureFactory;

        public Billing()
        {
            peopleFactory = new PeopleFactory(queryFactory);
            appointmentFactory = new AppointmentFactory(queryFactory, peopleFactory);
            householdFactory = new HouseholdFactory(queryFactory);
            appointmentPatientFactory = new AppointmentPatientFactory(queryFactory, appointmentFactory, peopleFactory);
            genderFactory = new GenderFactory(queryFactory);
            billableProcedureFactory = new BillableProcedureFactory(queryFactory);
        }

        public string GenerateSummary(DateTime date)
        {
            Dictionary<BillableProcedure, Appointment> bCodes = new Dictionary<BillableProcedure, Appointment>();
            bCodes = getBCodes(date.Year, date.Month);

            string billingOutput = "";

            foreach (KeyValuePair<BillableProcedure, Appointment> pair in bCodes)
            {
                string priceConverted = pair.Key.Price.ToString();//"189.45";
                string replacedPrice = priceConverted.Replace(".", "");
                int numZeros = (9 - replacedPrice.Length);
                string zeroString = string.Empty;

                for (int i = 0; i < numZeros; i++)
                {
                    zeroString += "0";
                }

                billingOutput += string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}\n",
                                                            pair.Value.Year,
                                                            pair.Value.Month.ToString().PadLeft(2, '0'),
                                                                        pair.Value.Day.ToString().PadLeft(2, '0'),
                                                                        pair.Value.PatientHCN,
                                                                        genderFactory.Find(pair.Value.PatientID),
                                                                        pair.Key.CodeName,
                                                                        zeroString,
                                                                        replacedPrice,
                                                                        "00");
            }

            return billingOutput;
        }



        private Dictionary<BillableProcedure, Appointment> getBCodes(int year, int month)
        {
            Dictionary<BillableProcedure, Appointment> bCodes = new Dictionary<BillableProcedure, Appointment>();

            List<Appointment> appointments = appointmentFactory.FindWithTimes(year, month);

            foreach (Appointment app in appointments)
            {
                List<BillableProcedure> b = billableProcedureFactory.GetBillableProcedures(app);
                foreach (BillableProcedure billableProcedure in b)
                {
                    bCodes[billableProcedure] = app;
                }
            }
            return bCodes;
        }
    }
}
