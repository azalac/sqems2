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
        private string connStr;

        private QueryFactory queryFactory;
        private AppointmentFactory appointmentFactory;
        private PeopleFactory peopleFactory;


        private Billing billing;
        private UI ui = new UI();
        private Demographics demographics;
        private TimeSlotFactory timeSlotFactory;
        private FileIO fileIO = new FileIO(); 


        private Dictionary<TextBox, Label> textLabels;
        private Dictionary<ComboBox, Label> comboBoxLabels;
        private Dictionary<TextBox, Label> headTextLabels;
        private Dictionary<TextBox, Label> personTextLabels;


        private List<Appointment> appointments;
       

        private Appointment recallAppointment;

        private Color red = Color.Red;
        private Color black = Color.Black;



        /// <summary>
        /// 
        /// </summary>
        public MainForm(string _connStr)
        {
            connStr = _connStr;
            queryFactory = new QueryFactory(connStr);
            appointmentFactory = new AppointmentFactory(queryFactory, peopleFactory);
            peopleFactory = new PeopleFactory(queryFactory);
            billing = new Billing(connStr);
            demographics = new Demographics(connStr);
            timeSlotFactory = new TimeSlotFactory(connStr);



            InitializeComponent();

            SetDictionaries();

            provinceComboBox.SelectedItem = "ON";

            GetSlots(calendar.SelectionStart);

            calendar.Focus();
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

            comboBoxLabels = new Dictionary<ComboBox, Label>()
            {
                {sexComboBox, sexLabel },
                {provinceComboBox, provinceLabel }
            };

            headTextLabels = new Dictionary<TextBox, Label>()
            {
                {headOfHouseTextBox, HOHLabel },
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
        private void ClearPatientInfo(bool clearHCN = true)
        {
            ui.ClearPatientInfo(textLabels, comboBoxLabels, clearHCN);

            provinceComboBox.SelectedIndex = 8;
            sexComboBox.SelectedIndex = -1;

            dateOfBirthDTP.Value = DateTime.Today;
        }





        /// <summary>
        /// 
        /// </summary>
        /// <param name="update"></param>
        /// <returns></returns>
        private int CheckNullPatientInfo(bool update = false)
        {
            int error = 0;

            error = ui.CheckNullInfo(personTextLabels, headTextLabels, update);

            sexLabel.ForeColor = black;

            if (sexComboBox.SelectedIndex == -1)
            {
                sexLabel.ForeColor = red;
                error++;
            }

            return error;
        }

        private int ValidatePatientInfo(bool update = false)
        {
            int error = 0;

            error = ui.ValidateInfo(personTextLabels, headTextLabels, update);

            return error;
        }



        private void HouseHeadText_Changed(object sender, EventArgs e)
        {
            bool enable = ui.HouseHeadText_Changed(headOfHouseTextBox, headTextLabels);

            provinceComboBox.Enabled = enable;
            address2TextBox.Enabled = enable;
        }






        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxTextChanged(object sender, EventArgs e)
        {
            TextBox senderObj = sender as TextBox;
            
            ui.CheckNull(senderObj, textLabels[senderObj]);
            
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












        private void GenerateFileButton_Click(object sender, EventArgs e)
        {
            string output = billing.GenerateSummary(billingDate.Value);
            billingOutput.Text = output;
            string message = "File Created";
            string title = "Created";

            if (output != "")
            {
                fileIO.SaveToFile(output, billingDate.Value.ToString("MMMM-yyyy"));


                MessageBox.Show(message, title);
            }
            else
            {
                message = "There were no billable procedures found for the given date";
                title = "No Billable Procedures";

                MessageBox.Show(message, title);
            }
        }






        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateButton_Click(object sender, EventArgs e)
        {
            if (CheckNullPatientInfo(true) == 0)
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
            // If validated
            if (CheckNullPatientInfo() == 0 && ValidatePatientInfo() == 0)
            {
                int? result = demographics.AddPatient(fNameTextBox.Text,
                                                   mInitialTextBox.Text[0],
                                                   lNameTextBox.Text,
                                                   healthCardTextBox.Text,
                                                   dateOfBirthDTP.Value,
                                                   sexComboBox.SelectedIndex + 1,
                                                   headOfHouseTextBox.Text);
                if (result != null)
                {
                    demographics.CreateHouseHold(result, 
                                               address1TextBox.Text, 
                                               address2TextBox.Text, 
                                               cityTextBox.Text, 
                                               (string)provinceComboBox.SelectedItem, 
                                               phoneNumberTextBox.Text);

                    string message = "Patient Created";
                    string title = "Created";
                    MessageBox.Show(message, title);

                    ClearPatientInfo();
                }
                else
                {
                    patientMessage.Text = "Head of house not found";
                    HOHLabel.ForeColor = red;
                }
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
            GetSlots(calendar.SelectionStart);
        }





        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppSlots_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (calendar.BoldedDates.Count() == 0)
            {
                SlotSelected(timeSlotFactory.slots[appSlots.SelectedIndex]);
            }
            else
            {
                ScheduleRecall(timeSlotFactory.slots[appSlots.SelectedIndex]);
            }

            GetSlots(calendar.SelectionStart);
        }





        /// <summary>
        /// 
        /// </summary>
        /// <param name="date"></param>
        private void GetSlots(DateTime date)
        {
            appointments = appointmentFactory.FindWithTimes(date.Year, date.Month, date.Day);

            timeSlotFactory.slots = timeSlotFactory.GetAppSlots(appointments, date);

            UpdateList();
        }

       



        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Control_KeyDown(object sender, KeyEventArgs e)
        {
            if (tabs.TabIndex == 0 && sender.GetType().Name == "TabControl")
            {
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
                    if (appSlots.SelectedIndex == -1)
                    {
                        appSlots.SelectedIndex = appSlots.Items.Count - 1;

                    }
                }

                if (e.KeyCode == Keys.Enter)
                {
                    if (calendar.BoldedDates.Count() == 0)
                    {
                        SlotSelected(timeSlotFactory.slots[appSlots.SelectedIndex]);
                    }
                    else
                    {
                        ScheduleRecall(timeSlotFactory.slots[appSlots.SelectedIndex]);
                    }

                    GetSlots(calendar.SelectionStart);
                }
            }
        }





        private void ScheduleRecall(TimeSlot timeSlot)
        {
            
            if (timeSlot.available == true)
            {
                DateTime date = timeSlot.date;

                if (calendar.BoldedDates.Contains(date))
                {
                    calendarOutput.Text = "";

                    string message = string.Format("Do you want to reschedule the appointment for {0}?", date.ToShortDateString());
                    string title = "Close Window";
                    MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                    DialogResult result = MessageBox.Show(message, title, buttons);
                    if (result == DialogResult.Yes)
                    {
                        appointmentFactory.Create(date.Year, date.Month, date.Day, timeSlot.slotID, recallAppointment.PatientID, recallAppointment.CaregiverID);

                        calendar.RemoveAllBoldedDates();
                        calendar.UpdateBoldedDates();

                        GetSlots(calendar.SelectionStart);
                    }
                }
                else
                {
                    calendarOutput.Text = "Please select a bolded date to schedule this appointment for recall";
                }
            }
        }






        private void SlotSelected(TimeSlot timeSlot)
        {
            
            AppointmentPatientFactory appointmentPatientFactory = new AppointmentPatientFactory(queryFactory, appointmentFactory, peopleFactory);

            Appointment selectedAppointment = new Appointment(appointmentPatientFactory);

            if (timeSlot.available == true)
            {

                FindPatientWindow findPatientWindow = new FindPatientWindow(timeSlot, connStr);
                findPatientWindow.ShowDialog();
            }
            if (timeSlot.available == false)
            {

                foreach (Appointment app in appointments)
                {
                    if (timeSlot.slotID == app.Timeslot)
                    {
                        selectedAppointment = app;
                    }
                }

                AppointmentBilling appointmentBilling = new AppointmentBilling(selectedAppointment, connStr);

                appointmentBilling.ShowDialog();

                recallAppointment = selectedAppointment;

                HighLightRecall(appointmentBilling.recallWeeks, appointmentBilling.appointmentDate);
            }
        }





        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HCN_TextChanged(object sender, EventArgs e)
        {
            HouseholdFactory householdFactory = new HouseholdFactory(queryFactory);

            List<Person> found = peopleFactory.Find(null, null, null, healthCardTextBox.Text);
            if (found.Count == 1)
            {
                patientMessage.Text = "Patient Found. Change the fields below to update their information.";
                Person foundPerson = found[0];

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

                ClearPatientInfo(false);

                updateButton.Visible = false;
                addPatientButton.Visible = true;
            }
        }






        private void HighLightRecall(int recallWeeks, DateTime startDate)
        {
            if (recallWeeks > 0)
            {
                int days = 7;
                DateTime[] dates = new DateTime[7];

                startDate = startDate.AddDays(7 * recallWeeks);

                for (int i = 0; i < days; i++)
                {
                    dates[i] = startDate.AddDays(i);
                }

                calendar.BoldedDates = dates;
            }
        }

        private void UpdateList()
        {
            appSlots.DataSource = null;
            appSlots.DataSource = timeSlotFactory.slots;
        }


        private void tabs_Selected(object sender, TabControlEventArgs e)
        {
            ui.ClearPatientInfo(textLabels, comboBoxLabels);
        }
    }
}
