using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMSDatabase
{
    public class AppointmentFactory : DatabaseRowFactoryBase<Appointment>
    {
        public AppointmentPatientFactory APFactory { get; }

        public AppointmentFactory(QueryFactory queryFactory, PeopleFactory peopleFactory) : base(queryFactory)
        {
            APFactory = new AppointmentPatientFactory(queryFactory, this, peopleFactory);
        }

        /// <summary>
        /// Finds an appointment by its id
        /// </summary>
        public Appointment Find(int ID)
        {
            return Find("SELECT * FROM AppointmentInfo WHERE ID = @0", ID);
        }

        /// <summary>
        /// Finds an appointment on the given time
        /// </summary>
        public Appointment Find(int Year, int Month, int Day, int Timeslot)
        {
            return Find("SELECT * FROM AppointmentInfo WHERE Year = @0 AND Month = @1 AND Day = @2 AND Timeslot = @3",
                Year, Month, Day, Timeslot);
        }

        /// <summary>
        /// Finds all appointments with the matching times.
        /// Set a parameter to null to ignore it.
        /// </summary>
        /// <returns>The list of appointments</returns>
        public List<Appointment> FindWithTimes(int? Year = null, int? Month = null, int? Day = null, int? Timeslot = null)
        {
            SqlWhereClauseBuilder dsq = new SqlWhereClauseBuilder();

            dsq.TryAddCondition("Year = {0}", Year);
            dsq.TryAddCondition("Month = {0}", Month);
            dsq.TryAddCondition("Day = {0}", Day);
            dsq.TryAddCondition("TimeSlot = {0}", Timeslot);

            var (where_str, where_objs) = dsq.Get();

            return FindMany("SELECT * FROM AppointmentInfo " + (dsq.Count > 0 ? "WHERE " : "") + where_str, where_objs);
        }

        /// <summary>
        /// Creates an appointment on the given time with a patient and optional caregiver.
        /// </summary>
        public Appointment Create(int year, int month, int day, int timeslot, Person patient, Person caregiver = null)
        {
            return Create(year, month, day, timeslot, patient.ID, caregiver?.ID);
        }

        /// <summary>
        /// Creates an appointment on the given time with a patient and optional caregiver.
        /// </summary>
        public Appointment Create(int year, int month, int day, int timeslot, int patient, int? caregiver = null)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                ["Year"] = year,
                ["Month"] = month,
                ["Day"] = day,
                ["Timeslot"] = timeslot,
                ["PatientID"] = patient,
                ["CaregiverID"] = caregiver
            };

            using(SqlCommand cmd = queryFactory.CreateProcedureCall("CreateAppointment", parameters))
            {
                object ret = cmd.ExecuteScalar();

                return Find((int)ret);
            }
        }

        protected override Appointment CreateObject(SqlDataReader reader)
        {
            return DatabaseRowObjectAdapter.Fill(new Appointment(APFactory), reader);
        }
        
    }

    /// <summary>
    /// Finds appointment patients and caregivers.
    /// </summary>
    public class AppointmentPatientFactory : DatabaseRowFactoryBase<AppointmentPatient>
    {
        private readonly AppointmentFactory appointmentFactory;
        private readonly PeopleFactory peopleFactory;

        public AppointmentPatientFactory(QueryFactory query, AppointmentFactory appointmentFactory, PeopleFactory peopleFactory)
            : base(query)
        {
            this.appointmentFactory = appointmentFactory;
            this.peopleFactory = peopleFactory;
        }

        /// <summary>
        /// Finds a patient or caregiver
        /// </summary>
        /// <param name="AppointmentID">The appointment to check</param>
        /// <param name="IsCaregiver">true to get the caregiver</param>
        /// <returns>The patient, caregiver, or null if none found</returns>
        public AppointmentPatient Find(int AppointmentID, bool IsCaregiver = false)
        {
            return Find("SELECT * FROM AppointmentPatient WHERE AppointmentID = @0 AND IsCaregiver = @1", AppointmentID, IsCaregiver ? "1" : "0");
        }

        protected override AppointmentPatient CreateObject(SqlDataReader reader)
        {
            return DatabaseRowObjectAdapter.Fill(new AppointmentPatient(appointmentFactory, peopleFactory), reader);
        }
    }

    /// <summary>
    /// Represents an appointment.
    /// </summary>
    public class Appointment
    {
        public readonly int ID, Year, Month, Day, Timeslot, PatientID, CaregiverID;
        public readonly string PatientHCN, CaregiverHCN, PatientName, CaregiverName;
        
        /// <summary>
        /// Calls GetPatient
        /// </summary>
        public AppointmentPatient Patient { get => GetPatient(); }

        /// <summary>
        /// Calls GetCaregiver
        /// </summary>
        public AppointmentPatient Caregiver { get => GetCaregiver(); }

        private readonly AppointmentPatientFactory apFactory;

        public Appointment(AppointmentPatientFactory apFactory)
        {
            this.apFactory = apFactory;
        }
        
        /// <summary>
        /// Gets the patient for this appointment, or null if none set (should never happen).
        /// </summary>
        /// <returns></returns>
        public AppointmentPatient GetPatient()
        {
            return apFactory.Find(ID, IsCaregiver: false);
        }

        /// <summary>
        /// Gets the caregiver for this appointment, or null if none set.
        /// </summary>
        public AppointmentPatient GetCaregiver()
        {
            return apFactory.Find(ID, IsCaregiver: true);
        }

        public override string ToString()
        {
            return string.Format("Appointment[ID={0}, {1}/{2}/{3} Timeslot {4}, Patient='{5}', Caregiver='{6}']",
                ID, Day, Month, Year, Timeslot, PatientName, CaregiverName);
        }

    }

    /// <summary>
    /// Represents a person going to an appointment.
    /// Can be a patient or a caregiver.
    /// </summary>
    public class AppointmentPatient
    {
        public readonly bool IsCaregiver;

        public readonly int ID, AppointmentID, PersonID;
        
        /// <summary>
        /// Gets the owning Appointment.
        /// </summary>
        public Appointment Appointment { get => aptFactory.Find(AppointmentID); }

        /// <summary>
        /// Gets the owning Person.
        /// </summary>
        public Person Person { get => peopleFactory.Find(PersonID); }
        
        private AppointmentFactory aptFactory;
        private PeopleFactory peopleFactory;

        public AppointmentPatient(AppointmentFactory aptFactory, PeopleFactory peopleFactory)
        {
            this.aptFactory = aptFactory;
            this.peopleFactory = peopleFactory;
        }

        public override string ToString()
        {
            return string.Format("AppointmentPatient[Appointment={0}, Person={1}, Type={2}]", AppointmentID, PersonID, IsCaregiver ? "Caregiver" : "Patient");
        }

    }
}
