﻿/// PROJECT: EMS2
/// FILE: Billing.cs
/// AUTHOR: Billy Parmenter, Mike Ramoutsakis, Austin Zalac, Mike Hilts
/// DATE: April 19 - 2019
/// DESCRIPTION: This form will handle all of the billing functionality in 
///              the UI.


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
        private BillingCodeFactory billingCodeFactory;
        private List<BillableProcedureStatus> bps = new List<BillableProcedureStatus>() { };

        /// <summary>
        /// This is the default billing constructor
        /// </summary>
        public Billing()
        {
            peopleFactory = new PeopleFactory(queryFactory);
            appointmentFactory = new AppointmentFactory(queryFactory, peopleFactory);
            householdFactory = new HouseholdFactory(queryFactory);
            appointmentPatientFactory = new AppointmentPatientFactory(queryFactory, appointmentFactory, peopleFactory);
            genderFactory = new GenderFactory(queryFactory);
            billingCodeFactory = new BillingCodeFactory(queryFactory);
            billableProcedureFactory = new BillableProcedureFactory(queryFactory);
        }
        /// <summary>
        /// This function will generate the billing file to be displayed 
        /// to the user.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public string GenerateSummary(DateTime date)
        {
            Dictionary<BillableProcedure, Appointment> bCodes = new Dictionary<BillableProcedure, Appointment>();
            bCodes = GetBillableProcedures(date.Year, date.Month);

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
        /// <summary>
        /// This function will handle a response file from the MOH
        /// </summary>
        /// <param name="readFile"></param>
        public void checkTransDB(List<string> readFile)
        {
            Dictionary<BillableProcedure, Appointment> bCodes = new Dictionary<BillableProcedure, Appointment>();
            ProcedureStatusFactory procedureStatusFactory = new ProcedureStatusFactory(queryFactory);

            int year = 0;
            int month = 0;
            string bcode = string.Empty;
            string hcn = string.Empty;
            string statusName = string.Empty;

            foreach (string s in readFile)
            {
                year = Int32.Parse(s.Substring(0, 4));
                month = Int32.Parse(s.Substring(4, 2));
                hcn = s.Substring(8, 12);
                bcode = s.Substring(21, 4);
                statusName = s.Substring(36, 4);


                bCodes = GetBillableProcedures(year, month);

                foreach (KeyValuePair<BillableProcedure, Appointment> pair in bCodes)
                {
                    if (year == pair.Value.Year && month == pair.Value.Month && hcn == pair.Value.PatientHCN && bcode == pair.Key.CodeName)
                    {
                        BillableProcedureStatus billableProcedureStatus = pair.Key.GetStatus();
                        //if (billableProcedureStatus.ID == procedureStatusFactory.None.ID)
                        //{
                            pair.Key.SetStatus(procedureStatusFactory.Find(statusName));
                        //}
                    }
                }
            }
        }


        /// <summary>
        /// Loads the valid billing codes from a file and puts them in the database
        /// </summary>
        /// <param name="path">Path to the file</param>
        public void ReloadMasterBillingCodes(string path)
        {
            billingCodeFactory.SetMasterCodes(BillingCodeFactory.ParseCodesFromFile(path));
        }


        /// <summary>
        /// This function will generate the billing summary to be displayed to the user
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public string GenerateSummary(int year, int month)
        {
            Dictionary<BillableProcedure, Appointment> billableProcedures = GetBillableProcedures(year, month);

            ProcedureStatusFactory procedureStatusFactory = new ProcedureStatusFactory(queryFactory);

            int totalEncountersBilled = billableProcedures.Count();
            double totalBilled = 0;
            double receivedTotal = 0;
            int numberOfFollowUps = 0;

            foreach(KeyValuePair<BillableProcedure, Appointment> pair in billableProcedures)
            {
                BillableProcedure procedure = pair.Key;

                totalBilled += procedure.Price;

                if(procedure.Status.ID == procedureStatusFactory.Paid.ID)
                {
                    receivedTotal += procedure.Price;
                }
                else if (procedure.Status.ID == procedureStatusFactory.ContactMoH.ID || procedure.Status.ID == procedureStatusFactory.InvalidHCN.ID)
                {
                    numberOfFollowUps++;
                }
            }

            double receivedPercent = (receivedTotal / totalBilled) * 100;
            double averageBilling = (receivedTotal / totalEncountersBilled);

            return string.Format("Total Enc. Billed = {0}\nTotal Billed = ${1:#.00}\nRec'd Total = ${2:#.00}\nRec'd Percrnt = {3:#.00}%\nAvg Billing = ${4:#.00}\nNo. Followup = {5}",
                totalEncountersBilled, totalBilled, receivedTotal, receivedPercent, averageBilling, numberOfFollowUps);
        }



        /// <summary>
        /// This funtion will get a list of billable procedures and appointments
        /// based on the year and the month selected.
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        private Dictionary<BillableProcedure, Appointment> GetBillableProcedures(int year, int month)
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
