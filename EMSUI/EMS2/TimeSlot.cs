using EMSDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS2
{
    class TimeSlot
    {
        public int slotID;
        public DateTime date;
        public bool available;

        public static List<TimeSlot> ToList (List<Appointment> appointments, DateTime date)
        {
            int numberOfSlots = 6;

            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
            {
                numberOfSlots = 2;
            }

            List<TimeSlot> slots = new List<TimeSlot>();

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

    //public TimeSlots(List<Appointment> appointments, DayOfWeek day)
    //{
    //    int numberOfSlots = 6;

    //    if (day == DayOfWeek.Saturday || day == DayOfWeek.Sunday)
    //    {
    //        numberOfSlots = 2;
    //    }

    //    for (int i = 1; i <= numberOfSlots; i++)
    //    {
    //        slots.Add(new Tuple<int, bool, string>;
    //    }

    //    for (int i = 0; i < appointments.Count; i++)
    //    {
    //        slotAvailability[String.Format("Slot: {0} Available: ", appointments[i].Timeslot)] = false;
    //    }


    //}
}
