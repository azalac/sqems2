/// PROJECT: EMS2
/// FILE: FindPatientWindow.cs
/// AUTHOR: Billy Parmenter
/// DATE: April 19 - 2019
/// DESCRIPTION: This form will bisplay all patients with the same given last name
///                 once a name is selected the user can add a caregiver, it will then
///                 create an appointment with the selected date from the calendar on
///                 the main form with the given patient and caregiver
/// 

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using EMSDatabase;


namespace EMS2
{
    public partial class FindPatientWindow : Form
    {
        // The factories being useed
        private static QueryFactory query = new QueryFactory();
        private static PeopleFactory peopleFactory = new PeopleFactory(query);
        private AppointmentFactory appointmentFactory = new AppointmentFactory(query, peopleFactory);

        // The hint for the textBoxes
        private string tbHint = "Last Name";

        private bool initalizing = true;
        private bool checkingHint = false;
        private List<Person> people;

        // The timeSlot selected from the main form
        private TimeSlot timeSlot;

        // Holds a textBox and its corresponding listBox
        Dictionary<TextBox, ListBox> pairs = new Dictionary<TextBox, ListBox>();





        /// <summary>
        /// The form constructor
        /// </summary>
        public FindPatientWindow(TimeSlot slot)
        {
            timeSlot = slot;

            InitializeComponent();

            pairs.Add(patientSearchTB, foundPatients);
            pairs.Add(caregiverSearchTB, foundCaregivers);

            SetTextHint(patientSearchTB, tbHint);
            SetTextHint(caregiverSearchTB, tbHint);

            initalizing = false;
        }





        /// <summary>
        /// When the text in a textBox is changed, the hint must be updated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            if (initalizing == false)
            { 
                TextBox textBox = sender as TextBox;

                UpdateTextHint(textBox, tbHint);

                Find(textBox);

                if (foundPatients.SelectedValue as Person != null)
                {
                    findOutput.Text = "";
                }
            }
        }





        /// <summary>
        /// Finds a list of people based on the users input from a given textBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FindPerson(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;

            Find(textBox);
        }





        /// <summary>
        /// Creates an appointment with the date and timeSlot selected from the main form
        /// and the selected patient and caregiver if selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Select(object sender, EventArgs e)
        {
            Submit();
        }





        /// <summary>
        /// CLoses the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close_Click(object sender, EventArgs e)
        {
            query.Close();

            this.Close();
        }







        /// <summary>
        /// Sets a given textBox with a given hint
        /// </summary>
        /// <param name="textBox"></param>
        /// <param name="hint"></param>
        private void SetTextHint(TextBox textBox, string hint)
        {
            textBox.ForeColor = Color.Gray;
            textBox.Text = hint;
            textBox.Select(textBox.TextLength, 0);
        }





        /// <summary>
        /// Updates the hint of a textBox
        /// </summary>
        /// <param name="textBox"></param>
        /// <param name="hint"></param>
        private void UpdateTextHint(TextBox textBox, string hint)
        {
            if (checkingHint == false)
            {
                checkingHint = true;

                int hintLen = hint.Length;
                int textLen = textBox.Text.Length;
                string text = textBox.Text;
                bool removed = false;

                if (removed == true && text != "")
                {
                    removed = false;
                }

                // If the string starts with the hint and is longet than the hint, 
                // remove the hint leaveing what was entered
                if (text.StartsWith(hint.Substring(0, hintLen)) && textLen > hintLen)
                {
                    textBox.ForeColor = Color.Black;
                    textBox.Text = text.Remove(0, hintLen);
                    textBox.SelectionStart = textBox.Text.Length;
                    textBox.SelectionLength = 0;
                }

                // If the string is the start of the hint but is shorter than the hint, 
                // then the user hit back space
                else if (text.StartsWith(hint.Substring(0, textLen)) && textLen > 0)
                {
                    textBox.ForeColor = Color.Black;
                    textBox.Text = "";
                    removed = true;
                }

                else if (removed == false && text == "")
                {
                    SetTextHint(textBox, hint);
                }

                checkingHint = false;
            }
        }

        private void Control_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (foundPatients.SelectedValue as Person == null)
                {
                    findOutput.Text = "No patient is selected";
                }
                else
                {
                    Submit();
                }
            }
            else if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void Find(TextBox textBox)
        {
            people = peopleFactory.Find(null, textBox.Text);

            pairs[textBox].DataSource = people;
        }



        private void Submit()
        {
            DateTime date = timeSlot.date;

            appointmentFactory.Create(date.Year, date.Month, date.Day, timeSlot.slotID, foundPatients.SelectedValue as Person, foundCaregivers.SelectedValue as Person);

            this.Close();
        }
    }
}
