namespace EMS2
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.updateButton = new System.Windows.Forms.Button();
            this.addPatientButton = new System.Windows.Forms.Button();
            this.provinceComboBox = new System.Windows.Forms.ComboBox();
            this.sexComboBox = new System.Windows.Forms.ComboBox();
            this.dateOfBirthDTP = new System.Windows.Forms.DateTimePicker();
            this.phoneLabel = new System.Windows.Forms.Label();
            this.provinceLabel = new System.Windows.Forms.Label();
            this.cityLabel = new System.Windows.Forms.Label();
            this.add2Label = new System.Windows.Forms.Label();
            this.add1Label = new System.Windows.Forms.Label();
            this.phoneNumberTextBox = new System.Windows.Forms.TextBox();
            this.cityTextBox = new System.Windows.Forms.TextBox();
            this.address2TextBox = new System.Windows.Forms.TextBox();
            this.address1TextBox = new System.Windows.Forms.TextBox();
            this.headOfHouseTextBox = new System.Windows.Forms.TextBox();
            this.HOHLabel = new System.Windows.Forms.Label();
            this.sexLabel = new System.Windows.Forms.Label();
            this.DOBLabel = new System.Windows.Forms.Label();
            this.mInitialLabel = new System.Windows.Forms.Label();
            this.fNameLabel = new System.Windows.Forms.Label();
            this.lNameLabel = new System.Windows.Forms.Label();
            this.mInitialTextBox = new System.Windows.Forms.TextBox();
            this.fNameTextBox = new System.Windows.Forms.TextBox();
            this.lNameTextBox = new System.Windows.Forms.TextBox();
            this.generateFileButton = new System.Windows.Forms.Button();
            this.generateSummaryButton = new System.Windows.Forms.Button();
            this.billingDate = new System.Windows.Forms.DateTimePicker();
            this.tabs = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.calendarOutput = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.appSlots = new System.Windows.Forms.ListBox();
            this.calendar = new System.Windows.Forms.MonthCalendar();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.HCNLabel = new System.Windows.Forms.Label();
            this.healthCardTextBox = new System.Windows.Forms.TextBox();
            this.patientMessage = new System.Windows.Forms.Label();
            this.label = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.billingOutput = new System.Windows.Forms.RichTextBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.tabs.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // updateButton
            // 
            this.updateButton.Location = new System.Drawing.Point(170, 216);
            this.updateButton.Name = "updateButton";
            this.updateButton.Size = new System.Drawing.Size(100, 23);
            this.updateButton.TabIndex = 13;
            this.updateButton.Text = "Update";
            this.updateButton.UseVisualStyleBackColor = true;
            this.updateButton.Visible = false;
            this.updateButton.Click += new System.EventHandler(this.UpdateButton_Click);
            // 
            // addPatientButton
            // 
            this.addPatientButton.Location = new System.Drawing.Point(170, 216);
            this.addPatientButton.Name = "addPatientButton";
            this.addPatientButton.Size = new System.Drawing.Size(100, 23);
            this.addPatientButton.TabIndex = 13;
            this.addPatientButton.Text = "Add";
            this.addPatientButton.UseVisualStyleBackColor = true;
            this.addPatientButton.Click += new System.EventHandler(this.AddPatientButton_Click);
            // 
            // provinceComboBox
            // 
            this.provinceComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.provinceComboBox.FormattingEnabled = true;
            this.provinceComboBox.Items.AddRange(new object[] {
            "AB",
            "BC",
            "MB",
            "NB",
            "NL",
            "NT",
            "NS",
            "NU",
            "ON",
            "PE",
            "QC",
            "SK",
            "YT"});
            this.provinceComboBox.Location = new System.Drawing.Point(313, 156);
            this.provinceComboBox.Name = "provinceComboBox";
            this.provinceComboBox.Size = new System.Drawing.Size(119, 21);
            this.provinceComboBox.TabIndex = 11;
            // 
            // sexComboBox
            // 
            this.sexComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sexComboBox.FormattingEnabled = true;
            this.sexComboBox.ItemHeight = 13;
            this.sexComboBox.Items.AddRange(new object[] {
            "M",
            "F",
            "I",
            "H"});
            this.sexComboBox.Location = new System.Drawing.Point(83, 182);
            this.sexComboBox.Name = "sexComboBox";
            this.sexComboBox.Size = new System.Drawing.Size(55, 21);
            this.sexComboBox.TabIndex = 6;
            this.sexComboBox.SelectedIndexChanged += new System.EventHandler(this.SexCBIndexChanged);
            // 
            // dateOfBirthDTP
            // 
            this.dateOfBirthDTP.Location = new System.Drawing.Point(83, 157);
            this.dateOfBirthDTP.Name = "dateOfBirthDTP";
            this.dateOfBirthDTP.Size = new System.Drawing.Size(119, 20);
            this.dateOfBirthDTP.TabIndex = 5;
            // 
            // phoneLabel
            // 
            this.phoneLabel.AutoSize = true;
            this.phoneLabel.Location = new System.Drawing.Point(208, 186);
            this.phoneLabel.Name = "phoneLabel";
            this.phoneLabel.Size = new System.Drawing.Size(78, 13);
            this.phoneLabel.TabIndex = 33;
            this.phoneLabel.Text = "Phone Number";
            // 
            // provinceLabel
            // 
            this.provinceLabel.AutoSize = true;
            this.provinceLabel.Location = new System.Drawing.Point(208, 160);
            this.provinceLabel.Name = "provinceLabel";
            this.provinceLabel.Size = new System.Drawing.Size(49, 13);
            this.provinceLabel.TabIndex = 32;
            this.provinceLabel.Text = "Province";
            // 
            // cityLabel
            // 
            this.cityLabel.AutoSize = true;
            this.cityLabel.Location = new System.Drawing.Point(208, 134);
            this.cityLabel.Name = "cityLabel";
            this.cityLabel.Size = new System.Drawing.Size(24, 13);
            this.cityLabel.TabIndex = 31;
            this.cityLabel.Text = "City";
            // 
            // add2Label
            // 
            this.add2Label.AutoSize = true;
            this.add2Label.Location = new System.Drawing.Point(207, 108);
            this.add2Label.Name = "add2Label";
            this.add2Label.Size = new System.Drawing.Size(77, 13);
            this.add2Label.TabIndex = 30;
            this.add2Label.Text = "Address Line 2";
            // 
            // add1Label
            // 
            this.add1Label.AutoSize = true;
            this.add1Label.Location = new System.Drawing.Point(208, 82);
            this.add1Label.Name = "add1Label";
            this.add1Label.Size = new System.Drawing.Size(77, 13);
            this.add1Label.TabIndex = 29;
            this.add1Label.Text = "Address Line 1";
            // 
            // phoneNumberTextBox
            // 
            this.phoneNumberTextBox.Location = new System.Drawing.Point(313, 183);
            this.phoneNumberTextBox.Name = "phoneNumberTextBox";
            this.phoneNumberTextBox.Size = new System.Drawing.Size(119, 20);
            this.phoneNumberTextBox.TabIndex = 12;
            this.phoneNumberTextBox.TextChanged += new System.EventHandler(this.TextBoxTextChanged);
            // 
            // cityTextBox
            // 
            this.cityTextBox.Location = new System.Drawing.Point(313, 131);
            this.cityTextBox.Name = "cityTextBox";
            this.cityTextBox.Size = new System.Drawing.Size(119, 20);
            this.cityTextBox.TabIndex = 10;
            this.cityTextBox.TextChanged += new System.EventHandler(this.TextBoxTextChanged);
            // 
            // address2TextBox
            // 
            this.address2TextBox.Location = new System.Drawing.Point(313, 105);
            this.address2TextBox.Name = "address2TextBox";
            this.address2TextBox.Size = new System.Drawing.Size(119, 20);
            this.address2TextBox.TabIndex = 9;
            this.address2TextBox.TextChanged += new System.EventHandler(this.TextBoxTextChanged);
            // 
            // address1TextBox
            // 
            this.address1TextBox.Location = new System.Drawing.Point(313, 79);
            this.address1TextBox.Name = "address1TextBox";
            this.address1TextBox.Size = new System.Drawing.Size(119, 20);
            this.address1TextBox.TabIndex = 8;
            this.address1TextBox.TextChanged += new System.EventHandler(this.TextBoxTextChanged);
            // 
            // headOfHouseTextBox
            // 
            this.headOfHouseTextBox.Location = new System.Drawing.Point(313, 53);
            this.headOfHouseTextBox.Name = "headOfHouseTextBox";
            this.headOfHouseTextBox.Size = new System.Drawing.Size(119, 20);
            this.headOfHouseTextBox.TabIndex = 7;
            this.headOfHouseTextBox.TextChanged += new System.EventHandler(this.HouseHeadText_Changed);
            // 
            // HOHLabel
            // 
            this.HOHLabel.AutoSize = true;
            this.HOHLabel.Location = new System.Drawing.Point(208, 56);
            this.HOHLabel.Name = "HOHLabel";
            this.HOHLabel.Size = new System.Drawing.Size(101, 13);
            this.HOHLabel.TabIndex = 22;
            this.HOHLabel.Text = "Head Of Household";
            // 
            // sexLabel
            // 
            this.sexLabel.AutoSize = true;
            this.sexLabel.Location = new System.Drawing.Point(9, 185);
            this.sexLabel.Name = "sexLabel";
            this.sexLabel.Size = new System.Drawing.Size(25, 13);
            this.sexLabel.TabIndex = 21;
            this.sexLabel.Text = "Sex";
            // 
            // DOBLabel
            // 
            this.DOBLabel.AutoSize = true;
            this.DOBLabel.Location = new System.Drawing.Point(9, 159);
            this.DOBLabel.Name = "DOBLabel";
            this.DOBLabel.Size = new System.Drawing.Size(68, 13);
            this.DOBLabel.TabIndex = 20;
            this.DOBLabel.Text = "Date Of Birth";
            // 
            // mInitialLabel
            // 
            this.mInitialLabel.AutoSize = true;
            this.mInitialLabel.Location = new System.Drawing.Point(9, 134);
            this.mInitialLabel.Name = "mInitialLabel";
            this.mInitialLabel.Size = new System.Drawing.Size(65, 13);
            this.mInitialLabel.TabIndex = 19;
            this.mInitialLabel.Text = "Middle Initial";
            // 
            // fNameLabel
            // 
            this.fNameLabel.AutoSize = true;
            this.fNameLabel.Location = new System.Drawing.Point(9, 108);
            this.fNameLabel.Name = "fNameLabel";
            this.fNameLabel.Size = new System.Drawing.Size(57, 13);
            this.fNameLabel.TabIndex = 18;
            this.fNameLabel.Text = "First Name";
            // 
            // lNameLabel
            // 
            this.lNameLabel.AutoSize = true;
            this.lNameLabel.Location = new System.Drawing.Point(9, 82);
            this.lNameLabel.Name = "lNameLabel";
            this.lNameLabel.Size = new System.Drawing.Size(58, 13);
            this.lNameLabel.TabIndex = 17;
            this.lNameLabel.Text = "Last Name";
            // 
            // mInitialTextBox
            // 
            this.mInitialTextBox.Location = new System.Drawing.Point(83, 131);
            this.mInitialTextBox.Name = "mInitialTextBox";
            this.mInitialTextBox.Size = new System.Drawing.Size(119, 20);
            this.mInitialTextBox.TabIndex = 4;
            this.mInitialTextBox.TextChanged += new System.EventHandler(this.TextBoxTextChanged);
            // 
            // fNameTextBox
            // 
            this.fNameTextBox.Location = new System.Drawing.Point(83, 105);
            this.fNameTextBox.Name = "fNameTextBox";
            this.fNameTextBox.Size = new System.Drawing.Size(119, 20);
            this.fNameTextBox.TabIndex = 3;
            this.fNameTextBox.TextChanged += new System.EventHandler(this.TextBoxTextChanged);
            // 
            // lNameTextBox
            // 
            this.lNameTextBox.Location = new System.Drawing.Point(83, 79);
            this.lNameTextBox.Name = "lNameTextBox";
            this.lNameTextBox.Size = new System.Drawing.Size(119, 20);
            this.lNameTextBox.TabIndex = 2;
            this.lNameTextBox.TextChanged += new System.EventHandler(this.TextBoxTextChanged);
            // 
            // generateFileButton
            // 
            this.generateFileButton.Location = new System.Drawing.Point(3, 65);
            this.generateFileButton.Name = "generateFileButton";
            this.generateFileButton.Size = new System.Drawing.Size(107, 23);
            this.generateFileButton.TabIndex = 2;
            this.generateFileButton.Text = "Generate File";
            this.generateFileButton.UseVisualStyleBackColor = true;
            this.generateFileButton.Click += new System.EventHandler(this.GenerateFileButton_Click);
            // 
            // generateSummaryButton
            // 
            this.generateSummaryButton.Location = new System.Drawing.Point(3, 36);
            this.generateSummaryButton.Name = "generateSummaryButton";
            this.generateSummaryButton.Size = new System.Drawing.Size(107, 23);
            this.generateSummaryButton.TabIndex = 1;
            this.generateSummaryButton.Text = "Generate Summary";
            this.generateSummaryButton.UseVisualStyleBackColor = true;
            // 
            // billingDate
            // 
            this.billingDate.CustomFormat = "MMM yyyy";
            this.billingDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.billingDate.Location = new System.Drawing.Point(3, 10);
            this.billingDate.Name = "billingDate";
            this.billingDate.ShowUpDown = true;
            this.billingDate.Size = new System.Drawing.Size(107, 20);
            this.billingDate.TabIndex = 0;
            // 
            // tabs
            // 
            this.tabs.Controls.Add(this.tabPage1);
            this.tabs.Controls.Add(this.tabPage2);
            this.tabs.Controls.Add(this.tabPage3);
            this.tabs.Controls.Add(this.tabPage4);
            this.tabs.Location = new System.Drawing.Point(12, 12);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(449, 271);
            this.tabs.TabIndex = 0;
            this.tabs.TabStop = false;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.calendarOutput);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.appSlots);
            this.tabPage1.Controls.Add(this.calendar);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(441, 245);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Calendar";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // calendarOutput
            // 
            this.calendarOutput.AutoSize = true;
            this.calendarOutput.Location = new System.Drawing.Point(15, 16);
            this.calendarOutput.Name = "calendarOutput";
            this.calendarOutput.Size = new System.Drawing.Size(0, 13);
            this.calendarOutput.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 209);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(333, 39);
            this.label1.TabIndex = 2;
            this.label1.Text = "Arrow key to select date, W and S to select app slot. \r\nHold Ctrl and use arrow k" +
    "eys for qick navigation of months and years.\r\n\r\n";
            // 
            // appSlots
            // 
            this.appSlots.FormattingEnabled = true;
            this.appSlots.Location = new System.Drawing.Point(257, 40);
            this.appSlots.Name = "appSlots";
            this.appSlots.Size = new System.Drawing.Size(157, 160);
            this.appSlots.TabIndex = 1;
            this.appSlots.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Control_KeyDown);
            this.appSlots.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.AppSlots_MouseDoubleClick);
            // 
            // calendar
            // 
            this.calendar.Location = new System.Drawing.Point(18, 38);
            this.calendar.MaxSelectionCount = 1;
            this.calendar.Name = "calendar";
            this.calendar.TabIndex = 0;
            this.calendar.DateChanged += new System.Windows.Forms.DateRangeEventHandler(this.DateSelected);
            this.calendar.DateSelected += new System.Windows.Forms.DateRangeEventHandler(this.DateSelected);
            this.calendar.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Control_KeyDown);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.HCNLabel);
            this.tabPage2.Controls.Add(this.healthCardTextBox);
            this.tabPage2.Controls.Add(this.patientMessage);
            this.tabPage2.Controls.Add(this.label);
            this.tabPage2.Controls.Add(this.updateButton);
            this.tabPage2.Controls.Add(this.addPatientButton);
            this.tabPage2.Controls.Add(this.provinceComboBox);
            this.tabPage2.Controls.Add(this.lNameTextBox);
            this.tabPage2.Controls.Add(this.sexComboBox);
            this.tabPage2.Controls.Add(this.fNameTextBox);
            this.tabPage2.Controls.Add(this.dateOfBirthDTP);
            this.tabPage2.Controls.Add(this.mInitialTextBox);
            this.tabPage2.Controls.Add(this.phoneLabel);
            this.tabPage2.Controls.Add(this.lNameLabel);
            this.tabPage2.Controls.Add(this.provinceLabel);
            this.tabPage2.Controls.Add(this.fNameLabel);
            this.tabPage2.Controls.Add(this.cityLabel);
            this.tabPage2.Controls.Add(this.mInitialLabel);
            this.tabPage2.Controls.Add(this.add2Label);
            this.tabPage2.Controls.Add(this.DOBLabel);
            this.tabPage2.Controls.Add(this.add1Label);
            this.tabPage2.Controls.Add(this.sexLabel);
            this.tabPage2.Controls.Add(this.phoneNumberTextBox);
            this.tabPage2.Controls.Add(this.HOHLabel);
            this.tabPage2.Controls.Add(this.cityTextBox);
            this.tabPage2.Controls.Add(this.headOfHouseTextBox);
            this.tabPage2.Controls.Add(this.address2TextBox);
            this.tabPage2.Controls.Add(this.address1TextBox);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(441, 245);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Patient";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // HCNLabel
            // 
            this.HCNLabel.AutoSize = true;
            this.HCNLabel.Location = new System.Drawing.Point(9, 56);
            this.HCNLabel.Name = "HCNLabel";
            this.HCNLabel.Size = new System.Drawing.Size(73, 13);
            this.HCNLabel.TabIndex = 38;
            this.HCNLabel.Text = "Health Card #";
            // 
            // healthCardTextBox
            // 
            this.healthCardTextBox.Location = new System.Drawing.Point(83, 53);
            this.healthCardTextBox.Name = "healthCardTextBox";
            this.healthCardTextBox.Size = new System.Drawing.Size(119, 20);
            this.healthCardTextBox.TabIndex = 1;
            this.healthCardTextBox.TextChanged += new System.EventHandler(this.HCN_TextChanged);
            // 
            // patientMessage
            // 
            this.patientMessage.AutoSize = true;
            this.patientMessage.ForeColor = System.Drawing.Color.Red;
            this.patientMessage.Location = new System.Drawing.Point(7, 33);
            this.patientMessage.Name = "patientMessage";
            this.patientMessage.Size = new System.Drawing.Size(0, 13);
            this.patientMessage.TabIndex = 35;
            // 
            // label
            // 
            this.label.AutoSize = true;
            this.label.Location = new System.Drawing.Point(7, 10);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(137, 13);
            this.label.TabIndex = 34;
            this.label.Text = "Enter a health card Number";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.billingOutput);
            this.tabPage3.Controls.Add(this.generateFileButton);
            this.tabPage3.Controls.Add(this.billingDate);
            this.tabPage3.Controls.Add(this.generateSummaryButton);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(441, 245);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Billing";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // billingOutput
            // 
            this.billingOutput.Location = new System.Drawing.Point(116, 10);
            this.billingOutput.Name = "billingOutput";
            this.billingOutput.ReadOnly = true;
            this.billingOutput.Size = new System.Drawing.Size(312, 223);
            this.billingOutput.TabIndex = 3;
            this.billingOutput.TabStop = false;
            this.billingOutput.Text = "";
            // 
            // tabPage4
            // 
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(441, 245);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "tabPage4";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(538, 331);
            this.Controls.Add(this.tabs);
            this.Name = "MainForm";
            this.Text = "EMS2";
            this.tabs.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ComboBox provinceComboBox;
        private System.Windows.Forms.ComboBox sexComboBox;
        private System.Windows.Forms.DateTimePicker dateOfBirthDTP;
        private System.Windows.Forms.Label phoneLabel;
        private System.Windows.Forms.Label provinceLabel;
        private System.Windows.Forms.Label cityLabel;
        private System.Windows.Forms.Label add2Label;
        private System.Windows.Forms.Label add1Label;
        private System.Windows.Forms.TextBox phoneNumberTextBox;
        private System.Windows.Forms.TextBox cityTextBox;
        private System.Windows.Forms.TextBox address2TextBox;
        private System.Windows.Forms.TextBox address1TextBox;
        private System.Windows.Forms.TextBox headOfHouseTextBox;
        private System.Windows.Forms.Label HOHLabel;
        private System.Windows.Forms.Label sexLabel;
        private System.Windows.Forms.Label DOBLabel;
        private System.Windows.Forms.Label mInitialLabel;
        private System.Windows.Forms.Label fNameLabel;
        private System.Windows.Forms.Label lNameLabel;
        private System.Windows.Forms.TextBox mInitialTextBox;
        private System.Windows.Forms.TextBox fNameTextBox;
        private System.Windows.Forms.TextBox lNameTextBox;
        private System.Windows.Forms.Button addPatientButton;
        private System.Windows.Forms.Button updateButton;
        private System.Windows.Forms.Button generateSummaryButton;
        private System.Windows.Forms.DateTimePicker billingDate;
        private System.Windows.Forms.Button generateFileButton;
        private System.Windows.Forms.TabControl tabs;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.MonthCalendar calendar;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.ListBox appSlots;
        private System.Windows.Forms.Label label;
        private System.Windows.Forms.Label patientMessage;
        private System.Windows.Forms.TextBox healthCardTextBox;
        private System.Windows.Forms.Label HCNLabel;
        private System.Windows.Forms.Label calendarOutput;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox billingOutput;
    }
}

