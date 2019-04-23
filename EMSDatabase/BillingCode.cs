using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace EMSDatabase
{
    /// <summary>
    /// Finds billing codes.
    /// </summary>
    public class BillingCodeFactory : DatabaseRowFactoryBase<BillingCode>
    {
        public BillingCodeFactory(QueryFactory queryFactory) : base(queryFactory)
        {
        }

        public BillingCode Find(string code)
        {
            return Find("SELECT * FROM MasterBillingCode WHERE Code = @0", code);
        }

        public List<BillingCode> Similar(string code, int max = 10)
        {
            return FindMany("SELECT TOP(@0) * FROM MasterBillingCode WHERE Code LIKE @1", max, string.Format("%{0}%", code));
        }

        protected override BillingCode CreateObject(SqlDataReader reader)
        {
            return DatabaseRowObjectAdapter.Fill(new BillingCode(), reader);
        }
    }

    /// <summary>
    /// Gets and sets billable procedures for a given appointment patient
    /// </summary>
    public class BillableProcedureFactory : DatabaseRowFactoryBase<BillableProcedure>
    {
        /// <summary>
        /// The internal status factory.
        /// </summary>
        public ProcedureStatusFactory StatusFactory { get; }

        public BillableProcedureFactory(QueryFactory queryFactory) : base(queryFactory)
        {
            StatusFactory = new ProcedureStatusFactory(queryFactory);
        }
        
        /// <summary>
        /// Gets all billable procedures for an appointment patient.
        /// </summary>
        /// <param name="person">The appointment patient</param>
        /// <returns>The list of billableprocedures</returns>
        public List<BillableProcedure> GetBillableProcedures(AppointmentPatient person)
        {
            return FindMany("SELECT * FROM BillingInfo WHERE AppointmentPatientID = @0", person.ID);
        }


        /// <summary>
        /// Gets all billable procedures for an appointment patient.
        /// </summary>
        /// <param name="person">The appointment patient</param>
        /// <returns>The list of billableprocedures</returns>
        public List<BillableProcedure> GetBillableProcedures(Appointment appointment)
        {
            return FindMany("SELECT * FROM BillingInfo WHERE AppointmentPatientID = (select ID from AppointmentPatient where appointmentID = @0 and iscaregiver = 0)", appointment.ID);
        }


        /// <summary>
        /// Sets the billable procedures for an appointment patient
        /// </summary>
        /// <param name="person">The appointment patient</param>
        /// <param name="codes">The list of billableprocedures</param>
        public void SetBillableProcedures(AppointmentPatient person, List<BillingCode> codes)
        {
            DataTable codeTable = new DataTable();

            codeTable.Columns.Add("BillingCodeID");

            foreach(var code in codes)
            {
                codeTable.Rows.Add(code.ID);
            }

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                ["AppointmentPatientID"] = person.ID,
                ["BillingCodeList"] = codeTable
            };

            using (SqlCommand cmd = queryFactory.CreateProcedureCall("SetBillableProcedures", parameters))
            {
                cmd.ExecuteNonQuery();
            }
        }

        protected override BillableProcedure CreateObject(SqlDataReader reader)
        {
            return DatabaseRowObjectAdapter.Fill(new BillableProcedure(queryFactory, StatusFactory), reader);
        }
    }

    /// <summary>
    /// Finds procedure statuses, with the defaults as property getters
    /// </summary>
    public class ProcedureStatusFactory : DatabaseRowFactoryBase<BillableProcedureStatus>
    {
        public BillableProcedureStatus None { get => Find("NONE"); }
        public BillableProcedureStatus Paid { get => Find("PAID"); }
        public BillableProcedureStatus Declined { get => Find("DECL"); }
        public BillableProcedureStatus InvalidHCN { get => Find("FHCV"); }
        public BillableProcedureStatus ContactMoH { get => Find("CMOH"); }

        public ProcedureStatusFactory(QueryFactory queryFactory) : base(queryFactory)
        {
        }

        public BillableProcedureStatus Find(int id)
        {
            return Find("SELECT * FROM [ProcedureState] WHERE ID = @0", id);
        }

        public BillableProcedureStatus Find(string code)
        {
            return Find("SELECT * FROM [ProcedureState] WHERE [Code] = @0", code);
        }

        protected override BillableProcedureStatus CreateObject(SqlDataReader reader)
        {
            return DatabaseRowObjectAdapter.Fill(new BillableProcedureStatus(), reader);
        }
    }

    /// <summary>
    /// Represents a billing code.
    /// </summary>
    public class BillingCode
    {
        public readonly int ID;
        public readonly string Code;
        public readonly DateTime StartDate;
        public readonly double Price;

        public string display { get => Display(); }

        public override string ToString()
        {
            return string.Format("{0}[ID={1}, Start Date={2}, Price=${3}]", Code, ID, StartDate.ToString("d"), Price);
        }

        public override bool Equals(object obj)
        {
            if(obj is BillingCode code)
            {
                return code.ID == ID;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return ID;
        }

        public string Display()
        {
            return string.Format("Code: {0}, Price=${1}", Code, Price);
        }
    }
    
    /// <summary>
    /// Represents a billable procedure.
    /// Links an appointment patient and a billing code.
    /// </summary>
    public class BillableProcedure
    {
        public readonly int AppointmentPatientID;

        [SQLColumnBinding("CodeID")]
        public readonly int BillingCodeID;

        [SQLColumnBinding("Code")]
        public readonly string CodeName;

        public readonly double Price;

        [SQLColumnBinding("Status")]
        private int? _statusID;

        /// <summary>
        /// Gets and sets the current status
        /// </summary>
        public BillableProcedureStatus Status { get => GetStatus(); set => SetStatus(value); }

        private QueryFactory queryFactory;
        private ProcedureStatusFactory statusFactory;

        public BillableProcedure(QueryFactory queryFactory, ProcedureStatusFactory statusFactory)
        {
            this.queryFactory = queryFactory;
            this.statusFactory = statusFactory;
        }

        /// <summary>
        /// Gets the status
        /// </summary>
        public BillableProcedureStatus GetStatus()
        {
            using (SqlCommand cmd = queryFactory.CreateQuery("SELECT [Status] FROM [BillableProcedure] WHERE AppointmentPatientID = @0 AND CodeID = @1",
                AppointmentPatientID, BillingCodeID))
            {
                try
                {
                    _statusID = Convert.ToInt32(cmd.ExecuteScalar());
                    return statusFactory.Find(_statusID.Value) ?? statusFactory.None;

                }
                catch
                {
                    return statusFactory.None;

                }
            }
        }

        /// <summary>
        /// Sets the status
        /// </summary>
        public void SetStatus(BillableProcedureStatus status)
        {
            using (SqlCommand cmd = queryFactory.CreateQuery("UPDATE [BillableProcedure] SET Status = @2 WHERE AppointmentPatientID = @0 AND CodeID = @1",
                AppointmentPatientID, BillingCodeID, status?.ID))
            {
                cmd.ExecuteNonQuery();
            }
        }

        public int? GetStatusID(string statusName)
        {
            using (SqlCommand cmd = queryFactory.CreateQuery("SELECT [ID] FROM [ProcedureState] WHERE [Code] = @0",
                statusName))
            {
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public override string ToString()
        {
            return string.Format("BillableProcedure[For = {0}, Code = {1}/{2}, Price = {3}, Status = {4}/{5}]",
                AppointmentPatientID, CodeName, BillingCodeID, Price, Status?.Code, _statusID != null ? _statusID.ToString() : "null");
        }
    }

    /// <summary>
    /// The paid status of a billable procedure
    /// </summary>
    public class BillableProcedureStatus
    {
        public readonly int ID;
        public readonly string Code, FullName;

        /// <summary>
        /// true if this status is an error status.
        /// </summary>
        public readonly bool IsError;

        public override string ToString()
        {
            return FullName;
        }

        public override bool Equals(object obj)
        {
            if(obj is BillableProcedureStatus status)
            {
                return status.ID == ID;
            }
            else
            {
                return false;
            }
        }
        
        public override int GetHashCode()
        {
            return ID;
        }
    }
}
