using EMSDatabase;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EMS2
{
    public partial class appointmentBilling : Form
    {
        private static QueryFactory queryFactory = new QueryFactory();
        private BillableProcedureFactory billableProcedureFactory = new BillableProcedureFactory(queryFactory);
        private BillingCodeFactory billingCodeFactory = new BillingCodeFactory(queryFactory);
        private static PeopleFactory peopleFactory = new PeopleFactory(queryFactory);
        private static AppointmentFactory appointmentFactory = new AppointmentFactory(queryFactory, peopleFactory);
        private AppointmentPatientFactory appointmentPatientFactory = new AppointmentPatientFactory(queryFactory, appointmentFactory, peopleFactory);
        private AppointmentPatient appointmentPatient = new AppointmentPatient(appointmentFactory, peopleFactory);


        private Appointment appointment = Form1.SelectedAppointment;
        private List<BillableProcedure> billableProcedures = new List<BillableProcedure>();
        private List<BillingCode> billingCodes = new List<BillingCode>();

        public appointmentBilling()
        {
            InitializeComponent();

            appointmentInfo.Text = appointment.Display();

            appointmentPatient = appointmentPatientFactory.Find(appointment.ID);

            billableProcedures = billableProcedureFactory.GetBillableProcedures(appointmentPatient);

            procedureToCode();

            billingCodeBox.DataSource = billingCodes;
            billingCodeBox.DisplayMember = "Display";

        }

        private void procedureToCode()
        {
            foreach (BillableProcedure procedure in billableProcedures)
            {
                billingCodes.Add(billingCodeFactory.Find(procedure.CodeName));
            }
        }

        private void addBillingCode_Click(object sender, EventArgs e)
        {
            BillingCode code = billingCodeFactory.Find(billingCodeTB.Text);
            if (code != null)
            {
                billingCodes.Add(code);
            }

            billingCodeBox.DataSource = null;
            billingCodeBox.DataSource = billingCodes;
            billingCodeBox.DisplayMember = "Display";
        }

        private void removeBillingCode_Click(object sender, EventArgs e)
        {
            if (billingCodeBox.SelectedIndex >= 0)
            {
                billingCodes.RemoveAt(billingCodeBox.SelectedIndex);
                billingCodeBox.DataSource = null;
                billingCodeBox.DataSource = billingCodes;
                billingCodeBox.DisplayMember = "Display";
            }
        }

        private void billingFinish_Click(object sender, EventArgs e)
        {
            AppointmentPatient appointmentPatient = appointmentPatientFactory.Find(appointment.ID);
            billableProcedureFactory.SetBillableProcedures(appointmentPatient, billingCodes);

            this.Close();
        }
    }
}
