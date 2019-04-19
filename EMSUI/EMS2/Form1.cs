using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EMSDatabase;


namespace EMS2
{
    public partial class Form1 : Form
    {
        private static QueryFactory queryFactory = new QueryFactory();
        private static PeopleFactory peopleFactory = new PeopleFactory(queryFactory);
        private AppointmentFactory appointmentFactory = new AppointmentFactory(queryFactory, peopleFactory);
        private HouseholdFactory householdFactory = new HouseholdFactory(queryFactory);


        private Dictionary<TextBox, Label> textLabels;
        private Dictionary<TextBox, Label> headTextLabels;
        private Dictionary<TextBox, Label> personTextLabels;


        private List<Appointment> appointments;
        private List<TimeSlot> slots;
        private List<Person> found = new List<Person>();


        private static TimeSlot selectedSlot;
        private static Appointment selectedAppointment;
        private Person foundPerson;
        private Household household;
        private Person houseHead;


        Color red = System.Drawing.Color.Red;
        Color black = System.Drawing.Color.Black;


        internal static TimeSlot SelectedSlot { get => selectedSlot; set => selectedSlot = value; }
        internal static Appointment SelectedAppointment { get => selectedAppointment; set => selectedAppointment = value; }

        //billing responcee



        public Form1()
        {
            InitializeComponent();

            SetDictionaries();

            provinceComboBox.SelectedItem = "ON";

            GetSlots(monthCalendar1.SelectionStart);

            monthCalendar1.Focus();
        }




        private void ClearPatientInfo(bool update)
        {
            foreach (KeyValuePair<TextBox, Label> textLabel in textLabels)
            {
                if (update == true && textLabel.Key != healthCardTextBox)
                {
                    textLabel.Key.Text = "";
                    textLabel.Value.ForeColor = black;
                }
            }
            sexLabel.ForeColor = black;

            provinceComboBox.SelectedIndex = 8;
            sexComboBox.SelectedIndex = -1;

            dateOfBirthDTP.Value = DateTime.Today;
        }

        private bool CheckNullPatientInfo(bool update = false)
        {
            int error = 0;
            bool ret = false;

            foreach (KeyValuePair<TextBox, Label> textLabel in personTextLabels)
            {
                error += CheckNull(textLabel.Key, textLabel.Value);
            }

            if (sexComboBox.SelectedIndex == -1)
            {
                sexLabel.ForeColor = red;
                error++;
            }
            else
            {
                sexLabel.ForeColor = black;
            }


            if (headOfHouseTextBox.Text == "" || update == true)
            {
                foreach (KeyValuePair<TextBox, Label> textLabel in headTextLabels)
                {
                    error += CheckNull(textLabel.Key, textLabel.Value);
                }
            }

            if(error != 0)
            {
                ret= true;
            }

            return ret;
        }

        private void HeadOfHouseTextBox_TextChanged(object sender, EventArgs e)
       {
            if (headOfHouseTextBox.Text != "")
            {
                foreach(KeyValuePair<TextBox, Label> textLabel in headTextLabels)
                {
                    textLabel.Key.Enabled = false;
                    textLabel.Value.ForeColor = black;
                }

                provinceComboBox.Enabled = false;
            }
            else
            {
                foreach (KeyValuePair<TextBox, Label> textLabel in headTextLabels)
                {
                    textLabel.Key.Enabled = true;
                    CheckNull(textLabel.Key, textLabel.Value);
                }

                provinceComboBox.Enabled = true;
            }
        }

       

        private void TextBoxTextChanged(object sender, EventArgs e)
        {
            TextBox senderObj = sender as TextBox;

            CheckNull(senderObj, textLabels[senderObj]);
        }

        private void SexCBIndexChanged(object sender, EventArgs e)
        {
            sexLabel.ForeColor = black;
        }

        private int CheckNull(TextBox tb, Label label)
        {
            if (tb.Text == "")
            {
                label.ForeColor = red;
                return 1;
            }
            else
            {
                label.ForeColor = black;
                return 0;
            }
        }

        private void SetDictionaries()
        {
            textLabels = new Dictionary<TextBox, Label>()
            {
                {healthCardTextBox, HCNLabel },
                {lNameTextBox, lNameLabel },
                {fNameTextBox, fNameLabel },
                {mInitialTextBox, mInitialLabel },
                {address1TextBox, add1Label },
                {address2TextBox, add2Label },
                {cityTextBox, cityLabel },
                {phoneNumberTextBox, phoneLabel },
                {headOfHouseTextBox, HOHLabel }
            };

            headTextLabels = new Dictionary<TextBox, Label>()
            {
                {address1TextBox, add1Label },
                {cityTextBox, cityLabel },
                {phoneNumberTextBox, phoneLabel }
            };

            personTextLabels = new Dictionary<TextBox, Label>()
            {
                {healthCardTextBox, HCNLabel },
                {lNameTextBox, lNameLabel },
                {fNameTextBox, fNameLabel },
                {mInitialTextBox, mInitialLabel },
            };
        }

        private void generateFileButton_Click(object sender, EventArgs e)
        {
            
        }


        private void UpdateButton_Click(object sender, EventArgs e)
        {
            if (CheckNullPatientInfo(true) == false)
            {
                patientMessage.Text = "Updated";
            }
            else
            {
                patientMessage.Text = "Fix the errors below";
            }
        }

        private void AddPatientButton_Click(object sender, EventArgs e)
        {
            if (CheckNullPatientInfo() == false)
            {
                int? houseID = null;
                if(headOfHouseTextBox.Text != "")
                {
                    found = peopleFactory.Find(null, null, null, headOfHouseTextBox.Text);
                    houseHead = found[0];
                    houseID = houseHead.GetHousehold();
                }
                peopleFactory.Create("T", 'E', "S", "T", DateTime.Today, 1);
                peopleFactory.Create(fNameTextBox.Text, mInitialTextBox.Text[0], lNameTextBox.Text, healthCardTextBox.Text, dateOfBirthDTP.Value, sexComboBox.SelectedIndex, houseID);
               
            }
            else
            {
                patientMessage.Text = "Fix the errors below";
            }
        }

        private void DateSelected(object sender, DateRangeEventArgs e)
        {
            GetSlots(monthCalendar1.SelectionStart);
        }

        private void appSlots_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (slots[appSlots.SelectedIndex].available == true)
            {
                selectedSlot = slots[appSlots.SelectedIndex];

                FindPatientWindow fpw = new FindPatientWindow();
                fpw.ShowDialog();
            }
        }


        private void GetSlots(DateTime date)
        {
            appointments = appointmentFactory.FindWithTimes(date.Year, date.Month, date.Day);

            slots = TimeSlot.ToList(appointments, date);

            appSlots.DataSource = slots;

        }

        private void appSlots_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && slots[appSlots.SelectedIndex].available == true)
            {
                selectedSlot = slots[appSlots.SelectedIndex];

                FindPatientWindow fpw = new FindPatientWindow();
                fpw.ShowDialog();
            }
        }

       
        private void monthCalendar1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode >= Keys.D1 && e.KeyCode <= Keys.D4)
            {
                tabs.SelectedIndex = e.KeyCode - Keys.D1;
            }
            if (e.KeyCode == Keys.S)
            {
                try
                {
                    appSlots.SelectedIndex++;
                }
                catch
                {
                    appSlots.SelectedIndex = 0;
                }
            }

            if (e.KeyCode == Keys.W)
            {

                appSlots.SelectedIndex--;
                if( appSlots.SelectedIndex == -1)
                {
                    appSlots.SelectedIndex = appSlots.Items.Count - 1;

                }
            }

            if (e.KeyCode == Keys.Enter && slots[appSlots.SelectedIndex].available == true)
            {
                selectedSlot = slots[appSlots.SelectedIndex];

                FindPatientWindow fpw = new FindPatientWindow();
                fpw.ShowDialog();
            }
            if (e.KeyCode == Keys.Enter && slots[appSlots.SelectedIndex].available == false)
            {
                selectedSlot = slots[appSlots.SelectedIndex];

                foreach (Appointment app in appointments)
                {
                    if (selectedSlot.slotID == app.Timeslot)
                    {
                        selectedAppointment = app;
                    }
                }

                appointmentBilling abw = new appointmentBilling();
                abw.ShowDialog();
            }
        }

        private void lNameTextBox_Leave(object sender, EventArgs e)
        {
            fNameTextBox.Enabled = true;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            found = peopleFactory.Find(null, null, null, patientSearchTB.Text);
            if (found.Count == 1)
            {
                patientMessage.Text = "Patient Found. Change the feilds below to update their information.";
                foundPerson = found[0];

                healthCardTextBox.Text = foundPerson.HCN;
                lNameTextBox.Text = foundPerson.lastName;
                fNameTextBox.Text = foundPerson.firstName;
                mInitialTextBox.Text = foundPerson.mInitial;
                dateOfBirthDTP.Value = foundPerson.dateOfBirth;
                sexComboBox.SelectedIndex = foundPerson.sex - 1;

                int? houseID = foundPerson.HouseholdID;

                if (houseID != null)
                {
                    HouseholdFactory householdFactory = new HouseholdFactory(queryFactory);
                    Household household = householdFactory.Find((int)houseID);
                    if (household.HeadOfHouse != null)
                    {
                        houseHead = peopleFactory.Find((int)household.HeadOfHouse);


                        headOfHouseTextBox.Text = houseHead.HCN;
                        headOfHouseTextBox.Enabled = true;
                        address1TextBox.Text = household.AddressLine1;
                        address1TextBox.Enabled = true;
                        address2TextBox.Text = household.AddressLine2;
                        address2TextBox.Enabled = true;
                        cityTextBox.Text = household.City;
                        cityTextBox.Enabled = true;
                        provinceComboBox.SelectedItem = household.Province;
                        provinceComboBox.Enabled = true;
                        phoneNumberTextBox.Text = household.PhoneNumber;
                        phoneNumberTextBox.Enabled = true;
                    }
                }

                updateButton.Visible = true;
                addPatientButton.Visible = false;

            }
            else
            {
                patientMessage.Text = "No Patient Found. Please enter the required feilds below.";

                healthCardTextBox.Text = patientSearchTB.Text;

                ClearPatientInfo(true);

                updateButton.Visible = false;
                addPatientButton.Visible = true;
            }
        }
    }
}
