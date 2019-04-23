using EMSDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS2
{
    class Demographics
    {
        private QueryFactory queryFactory;
        private PeopleFactory peopleFactory;
        private AppointmentFactory appointmentFactory;
        private HouseholdFactory householdFactory;
        private AppointmentPatientFactory appointmentPatientFactory;
        private GenderFactory genderFactory;
        private BillableProcedureFactory billableProcedureFactory;

        private List<Person> found = new List<Person>();
        private Person createdPerson;
        private Household household;
        private int? houseHoldID;


        public Demographics(string connStr)
        {
            queryFactory = new QueryFactory(connStr);
            peopleFactory = new PeopleFactory(queryFactory);
            appointmentFactory = new AppointmentFactory(queryFactory, peopleFactory);
            householdFactory = new HouseholdFactory(queryFactory);
            appointmentPatientFactory = new AppointmentPatientFactory(queryFactory, appointmentFactory, peopleFactory);
            genderFactory = new GenderFactory(queryFactory);
            billableProcedureFactory = new BillableProcedureFactory(queryFactory);
        }






        public int? AddPatient(string firstName, char middleInitial, string lastName, string HCN,
            DateTime DateOfBirth, int sex, string headOfHouse = null)
        {
            int? houseID = null;


            // If the patient is not the head of the house then find the houseID with the given HOH HCN
            if (headOfHouse != "")
            {
                found = peopleFactory.Find(null, null, null, headOfHouse);

                if (found.Count > 0)
                {
                    createdPerson = peopleFactory.Create(firstName, middleInitial, lastName, HCN, DateOfBirth, sex, houseID);

                    houseID = found[0].GetHousehold();
                }
            }
            else
            {
                createdPerson = peopleFactory.Create(firstName, middleInitial, lastName, HCN, DateOfBirth, sex, houseID);

                houseID = 0;
            }

            return houseID;
        }






        public void CreateHouseHold(int? houseID, string Address1 = null, string Address2 = null, string City = null,
            string Province = null, string PhoneNumber = null)
        {
            if (houseID == 0)
            {
                household = householdFactory.FindOrCreate(Address1, Address2, City, Province, PhoneNumber);

                household.HeadOfHouse = createdPerson.ID;

                houseID = household.ID;
            }

            createdPerson.HouseholdID = houseID;
        }
    }
}
