﻿/// PROJECT: EMS2
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
    public partial class appointmentBilling : Form
    {
        // Factories being used in the UI
        private static QueryFactory queryFactory = new QueryFactory();
        private BillableProcedureFactory billableProcedureFactory = new BillableProcedureFactory(queryFactory);
        private BillingCodeFactory billingCodeFactory = new BillingCodeFactory(queryFactory);
        private static PeopleFactory peopleFactory = new PeopleFactory(queryFactory);
        private static AppointmentFactory appointmentFactory = new AppointmentFactory(queryFactory, peopleFactory);
        private AppointmentPatientFactory appointmentPatientFactory = new AppointmentPatientFactory(queryFactory, appointmentFactory, peopleFactory);

        // Appointment selected from the main form
        private Appointment appointment = Form1.SelectedAppointment;
        
       
        private AppointmentPatient appointmentPatient = new AppointmentPatient(appointmentFactory, peopleFactory);
        private List<BillableProcedure> billableProcedures = new List<BillableProcedure>();
        private List<BillingCode> billingCodes = new List<BillingCode>();





        /// <summary>
        /// Constructor for form
        /// </summary>
        public appointmentBilling()
        {
            InitializeComponent();

            appointmentInfo.Text = appointment.Display();

            appointmentPatient = appointmentPatientFactory.Find(appointment.ID);

            billableProcedures = billableProcedureFactory.GetBillableProcedures(appointmentPatient);

            // Converts the found list of billable procedures to a list of billing codes 
            procedureToCode();

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
            billableProcedureFactory.SetBillableProcedures(appointmentPatient, billingCodes);

            this.Close();
        }





        /// <summary>
        /// When the add button is clicked add the given code if it is found
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Add_Click(object sender, EventArgs e)
        {
            BillingCode code = billingCodeFactory.Find(billingCodeTB.Text);
            if (code != null)
            {
                billingCodes.Add(code);
            }


        }





        /// <summary>
        /// When the remove button is clicked remove the selected code
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Remove_Click(object sender, EventArgs e)
        {
            if (billingCodeBox.SelectedIndex >= 0)
            {
                billingCodes.RemoveAt(billingCodeBox.SelectedIndex);

                UpdateList();
            }
        }





        /// <summary>
        /// add thebilling codes of the found billableProcedures to the listBox
        /// </summary>
        private void procedureToCode()
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
    }
}
