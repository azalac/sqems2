using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EMSDatabase;


/*
 * This form does nothing right now but should bisplay all patients with
 *      the same last name, once a name is double clicked on in the list box
 *      it should pass the patients info back to the main form to fill in
 *      the patients info
 */

namespace EMS2
{
    public partial class FindPatientWindow : Form
    {
        private string tbHint = "Last Name";

        private bool initalizing = true;

        private TimeSlot timeSlot = Form1.SelectedSlot;

        Dictionary<TextBox, ListBox> pairs = new Dictionary<TextBox, ListBox>();

        public FindPatientWindow()
        {
            InitializeComponent();

            pairs.Add(patientSearchTB, foundPatients);
            pairs.Add(caregiverSearchTB, foundCaregivers);

            patientSearchTB.ForeColor = Color.Gray;
            patientSearchTB.Text = tbHint;
            patientSearchTB.Select(patientSearchTB.TextLength, 0);

            caregiverSearchTB.ForeColor = Color.Gray;
            caregiverSearchTB.Text = tbHint;
            caregiverSearchTB.Select(caregiverSearchTB.TextLength, 0);

            initalizing = false; 
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            if (initalizing == false)
            {
                TextBox senderObj = sender as TextBox;

                int removeLength = 0;

                bool remove = false;

                if (senderObj.Text.StartsWith(tbHint.Substring(0, tbHint.Length)) && senderObj.Text.Length > tbHint.Length)
                {
                    removeLength = tbHint.Length;
                    remove = true;
                }
                else if (senderObj.Text.StartsWith(tbHint.Substring(0, senderObj.Text.Length)))
                {
                    removeLength = senderObj.Text.Length;
                    remove = true;
                }

                if (remove == true)
                {
                    senderObj.ForeColor = Color.Black;
                    senderObj.Text = senderObj.Text.Remove(0, removeLength);
                    senderObj.SelectionStart = senderObj.Text.Length;
                    senderObj.SelectionLength = 0;
                    senderObj.SelectionLength = 0;
                }
            }
        }

        private void FindPerson(object sender, EventArgs e)
        {
            Find(sender);
        }

        private void Select(object sender, EventArgs e)
        {
            QueryFactory query = new QueryFactory();
            PeopleFactory peopleFactory = new PeopleFactory(query);
            AppointmentFactory appointmentFactory = new AppointmentFactory(query, peopleFactory);

            DateTime date = timeSlot.date;

 
            appointmentFactory.Create(date.Year, date.Month, date.Day, timeSlot.slotID, foundPatients.SelectedValue as Person, foundCaregivers.SelectedValue as Person);

            this.Close();
            

        }

        private void Close(object sender, EventArgs e)
        {
            this.Close();
        }

        private void patientSearchTB_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Find(sender);
            }
        }

        private void Find(object sender)
        {
            TextBox textBox = sender as TextBox;

            QueryFactory query = new QueryFactory();

            PeopleFactory peopleFactory = new PeopleFactory(query);

            List<Person> people = peopleFactory.Find(null, textBox.Text);

            pairs[textBox].DataSource = people;

            query.Close();
        }
    }
}
