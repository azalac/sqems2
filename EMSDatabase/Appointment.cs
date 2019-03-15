using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace EMSDatabase
{
    public class AppointmentFactory : DatabaseRowFactoryBase<Appointment, (int, int, int, int)>
    {
        public AppointmentFactory(QueryFactory queryFactory) : base(queryFactory)
        {
        }
        
        public Appointment Find(int ID)
        {
            return Find("SELECT * FROM AppointmentInfo WHERE ID = @0", ID);
        }

        public Appointment Find(int Year, int Month, int Day, int Timeslot)
        {
            return Find("SELECT * FROM AppointmentInfo WHERE Year = @0 AND Month = @1 AND Day = @2 AND Timeslot = @3",
                Year, Month, Day, Timeslot);
        }
        
        protected override Appointment CreateObject(MySqlDataReader reader)
        {
            return DatabaseRowObjectAdapter.Fill(new Appointment(queryFactory), reader);
        }
    }
    
    public class Appointment
    {
        public readonly int ID, Year, Month, Day, Timeslot, PatientID, CaregiverID;
        public readonly string PatientHCN, CaregiverHCN;
        public readonly AppointmentPatient people;

        private readonly QueryFactory queryFactory;

        public Appointment(QueryFactory queryFactory)
        {
            this.queryFactory = queryFactory;
        }
        
    }

    public enum PatientType
    {
        Patient, Caregiver
    }

    public class AppointmentPatient
    {
        [EnumSQLColumn]
        public PatientType type;

        public int AppointmentID, PersonID;

        // these two fields are ignored
        public Appointment appointment;
        public Person person;

        public void Update(Appointment appointment, QueryFactory queryFactory)
        {
            this.appointment = appointment;

            if(appointment.ID != AppointmentID)
            {
                throw new Exception("Error: Appointment IDs don't match (" + appointment.ID + " vs " + AppointmentID + ")");
            }


        }
    }
}
