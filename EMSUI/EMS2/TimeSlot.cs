/// PROJECT: EMS2
/// FILE: TimeSlot.cs
/// AUTHOR: Billy Parmenter
/// DATE: April 19 - 2019

using EMSDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS2
{

    public class TimeSlotFactory
    {
        public List<TimeSlot> slots = new List<TimeSlot>();
        public TimeSlot selectedSlot = new TimeSlot();

        private QueryFactory queryFactory;
        private PeopleFactory peopleFactory;
        private AppointmentFactory appointmentFactory;
        private HouseholdFactory householdFactory;
        private AppointmentPatientFactory appointmentPatientFactory;
        private GenderFactory genderFactory;
        private BillableProcedureFactory billableProcedureFactory;


        public TimeSlotFactory(string connStr)
        {
            queryFactory = new QueryFactory(connStr);
            peopleFactory = new PeopleFactory(queryFactory);
            appointmentFactory = new AppointmentFactory(queryFactory, peopleFactory);
            householdFactory = new HouseholdFactory(queryFactory);
            appointmentPatientFactory = new AppointmentPatientFactory(queryFactory, appointmentFactory, peopleFactory);
            genderFactory = new GenderFactory(queryFactory);
            billableProcedureFactory = new BillableProcedureFactory(queryFactory);
        }

      

        /// <summary>
        /// Switches a list of appointments to a list of time slots for a given date
        /// </summary>
        /// <param name="appointments"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public List<TimeSlot> GetAppSlots(List<Appointment> appointments, DateTime date)
        {
            slots = new List<TimeSlot>();


            int numberOfSlots = 6;

            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
            {
                numberOfSlots = 2;
            }



            for (int i = 1; i <= numberOfSlots; i++)
            {
                TimeSlot slot = new TimeSlot
                {
                    slotID = i,
                    date = date,
                    available = true
                };

                foreach (Appointment app in appointments)
                {
                    if (app.Timeslot == i)
                    {
                        slot.available = false;
                    }
                }

                slots.Add(slot);
            }

            return slots;
        }


    }

    /// <summary>
    /// This class represents a timeslot
    /// </summary>
    public class TimeSlot
    {
        public int slotID;
        public DateTime date;
        public bool available;





        /// <summary>
        /// Over ridden to string for display
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string availableString = "Yes";

            if (available == false)
            {
                availableString = "No";
            }

            return String.Format("Slot: {0} Available {1}", slotID, availableString);
        }
    }
}
