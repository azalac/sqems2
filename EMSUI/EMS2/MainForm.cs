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
    public partial class MainForm : Form
    {
        private QueryFactory queryFactory = new QueryFactory();
        private PeopleFactory peopleFactory;
        private AppointmentFactory appointmentFactory;
        private HouseholdFactory householdFactory;


        private Dictionary<TextBox, Label> textLabels;
        private Dictionary<TextBox, Label> headTextLabels;
        private Dictionary<TextBox, Label> personTextLabels;


        private List<Appointment> appointments;
        private List<TimeSlot> slots;
        private List<Person> found = new List<Person>();


        private TimeSlot selectedSlot;
        private Appointment selectedAppointment;
        private Person foundPerson;

        Color red = Color.Red;
        Color black = Color.Black;



        /// <summary>
        /// 
        /// </summary>
        public MainForm()
        {
            peopleFactory = new PeopleFactory(queryFactory);
            appointmentFactory = new AppointmentFactory(queryFactory, peopleFactory);
            householdFactory = new HouseholdFactory(queryFactory);
            InitializeComponent();

            SetDictionaries();

            provinceComboBox.SelectedItem = "ON";

            GetSlots(monthCalendar1.SelectionStart);

            monthCalendar1.Focus();
        }





        /// <summary>
        /// 
        /// </summary>
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





        /// <summary>
        /// 
        /// </summary>
        /// <param name="update"></param>
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





        /// <summary>
        /// 
        /// </summary>
        /// <param name="update"></param>
        /// <returns></returns>
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





        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

       



        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxTextChanged(object sender, EventArgs e)
        {
            TextBox senderObj = sender as TextBox;

            CheckNull(senderObj, textLabels[senderObj]);
        }





        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SexCBIndexChanged(object sender, EventArgs e)
        {
            sexLabel.ForeColor = black;
        }





        /// <summary>
        /// 
        /// </summary>
        /// <param name="tb"></param>
        /// <param name="label"></param>
        /// <returns></returns>
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






        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GenerateFileButton_Click(object sender, EventArgs e)
        {
            
        }





        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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





        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddPatientButton_Click(object sender, EventArgs e)
        {
            if (CheckNullPatientInfo() == false)
            {
                int? houseID = null;
                if(headOfHouseTextBox.Text != "")
                {
                    found = peopleFactory.Find(null, null, null, headOfHouseTextBox.Text);
                    houseID = found[0].GetHousehold();
                }
                Person createdPerson = peopleFactory.Create(fNameTextBox.Text, mInitialTextBox.Text[0], lNameTextBox.Text, healthCardTextBox.Text, dateOfBirthDTP.Value, sexComboBox.SelectedIndex + 1, houseID);

                Household household = new Household(queryFactory);

                if (headOfHouseTextBox.Text == "")
                {
                    household = householdFactory.FindOrCreate(address1TextBox.Text, address2TextBox.Text, cityTextBox.Text, (string)provinceComboBox.SelectedItem, phoneNumberTextBox.Text);

                    household.HeadOfHouse = createdPerson.ID;
                }
                else
                {
                    Person hoh = peopleFactory.Find(null, null, null, headOfHouseTextBox.Text)[0];

                    household = householdFactory.Find((int)hoh.HouseholdID);
                }

                createdPerson.HouseholdID = household.ID;
            }
            else
            {
                patientMessage.Text = "Fix the errors below";
            }
        }





        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DateSelected(object sender, DateRangeEventArgs e)
        {
            GetSlots(monthCalendar1.SelectionStart);
        }





        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppSlots_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            SlotSelected(slots[appSlots.SelectedIndex]);
        }





        /// <summary>
        /// 
        /// </summary>
        /// <param name="date"></param>
        private void GetSlots(DateTime date)
        {
            appointments = appointmentFactory.FindWithTimes(date.Year, date.Month, date.Day);

            slots = TimeSlot.ToList(appointments, date);

            appSlots.DataSource = slots;

        }

       



        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Control_KeyDown(object sender, KeyEventArgs e)
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

            if (e.KeyCode == Keys.Enter)
            {
                SlotSelected(slots[appSlots.SelectedIndex]);
            }
        }






        private void SlotSelected(TimeSlot timeSlot)
        {
            if (timeSlot.available == true)
            {
                selectedSlot = timeSlot;

                FindPatientWindow fpw = new FindPatientWindow(selectedSlot);
                fpw.ShowDialog();
            }
            if (timeSlot.available == false)
            {
                selectedSlot = timeSlot;

                foreach (Appointment app in appointments)
                {
                    if (selectedSlot.slotID == app.Timeslot)
                    {
                        selectedAppointment = app;
                    }
                }

                appointmentBilling abw = new appointmentBilling(selectedAppointment);
                abw.ShowDialog();
            }
        }





        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Search_TextChanged(object sender, EventArgs e)
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
                    Household household = householdFactory.Find((int)houseID);
                    if (household.HeadOfHouse != null)
                    {
                         Person houseHead = peopleFactory.Find((int)household.HeadOfHouse);


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
