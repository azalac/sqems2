/// PROJECT: EMS2
/// FILE: appointmentBilling.cs
/// AUTHOR: Billy Parmenter
/// DATE: April 19 - 2019
/// DESCRIPTION: This form will use the selected appointment from the
///                 calendar from the main form and allow the user to
///                 add and or remove billing codes from it

using EMSDatabase;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace EMS2
{
    public partial class AppointmentBilling : Form
    {
        // Factories being used in the UI
        private QueryFactory queryFactory;
        private BillableProcedureFactory billableProcedureFactory;
        private BillingCodeFactory billingCodeFactory;
        private PeopleFactory peopleFactory;
        private AppointmentFactory appointmentFactory;
        private AppointmentPatientFactory appointmentPatientFactory;

        // Appointment selected from the main form
        private Appointment appointment;


        private AppointmentPatient appointmentPatient;
        private List<BillableProcedure> billableProcedures = new List<BillableProcedure>();
        private List<BillingCode> billingCodes = new List<BillingCode>();

        public int recallWeeks;
        public DateTime appointmentDate;



        /// <summary>
        /// Constructor for form
        /// </summary>
        public AppointmentBilling(Appointment app, string connStr)
        {
            queryFactory = new QueryFactory(connStr);
            billableProcedureFactory = new BillableProcedureFactory(queryFactory);
            billingCodeFactory = new BillingCodeFactory(queryFactory);
            peopleFactory = new PeopleFactory(queryFactory);
            appointmentFactory = new AppointmentFactory(queryFactory, peopleFactory);
            appointmentPatientFactory = new AppointmentPatientFactory(queryFactory, appointmentFactory, peopleFactory);

            appointment = app;

            InitializeComponent();

            recallCB.SelectedIndex = 0;

            appointmentInfo.Text = appointment.Display();

            appointmentPatient = appointmentPatientFactory.Find(appointment.ID);

            billableProcedures = billableProcedureFactory.GetBillableProcedures(appointmentPatient);

            // Converts the found list of billable procedures to a list of billing codes 
            ProcedureToCode();

            billingCodeBox.DataSource = billingCodes;
            billingCodeBox.DisplayMember = "Display";

        }





        /// <summary>
        /// When the finish button is clicked update the  billableProcedures
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Finish_Click(object sender, EventArgs e)
        {
            Finish();
        }





        /// <summary>
        /// When the add button is clicked add the given code if it is found
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Add_Click(object sender, EventArgs e)
        {
            Add();
        }





        /// <summary>
        /// When the remove button is clicked remove the selected code
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Remove_Click(object sender, EventArgs e)
        {
            Remove();
        }





        /// <summary>
        /// add thebilling codes of the found billableProcedures to the listBox
        /// </summary>
        private void ProcedureToCode()
        {
            foreach (BillableProcedure procedure in billableProcedures)
            {
                billingCodes.Add(billingCodeFactory.Find(procedure.CodeName));

                UpdateList();
            }
        }





        /// <summary>
        /// Updates the datasource for the listBox
        /// </summary>
        private void UpdateList()
        {
            billingCodeBox.DataSource = null;
            billingCodeBox.DataSource = billingCodes;
            billingCodeBox.DisplayMember = "Display";
        }



        private void Add()
        {
            BillingCode code = billingCodeFactory.Find(billingCodeTB.Text);
            if (code != null && billingCodes.Contains(code) == false)
            {
                billingCodes.Add(code);

                UpdateList();
            }
        }



        private void Remove()
        {
            if (billingCodeBox.SelectedIndex >= 0)
            {
                billingCodes.RemoveAt(billingCodeBox.SelectedIndex);

                UpdateList();
            }
        }



        private void Finish()
        {
            billableProcedureFactory.SetBillableProcedures(appointmentPatient, billingCodes);

            this.recallWeeks = recallCB.SelectedIndex;

            this.appointmentDate = new DateTime(appointment.Year, appointment.Month, appointment.Day);

            this.Close();
        }




        private void Key_Pressed(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (sender is TextBox)
                {
                    Add();
                }
                else if (sender is ComboBox)
                {
                    Finish();
                }
            }
            else if (sender is ListBox && (e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete))
            {
                Remove();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void Exit(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }
    }
}
