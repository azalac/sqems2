namespace EMS2
{
    partial class FindPatientWindow
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
            this.label1 = new System.Windows.Forms.Label();
            this.patientSearchTB = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.foundPatients = new System.Windows.Forms.ListBox();
            this.findPatient = new System.Windows.Forms.Button();
            this.findCaregiver = new System.Windows.Forms.Button();
            this.foundCaregivers = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.caregiverSearchTB = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.select = new System.Windows.Forms.Button();
            this.cancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Search for patient";
            // 
            // patientSearchTB
            // 
            this.patientSearchTB.Location = new System.Drawing.Point(13, 30);
            this.patientSearchTB.Name = "patientSearchTB";
            this.patientSearchTB.Size = new System.Drawing.Size(142, 20);
            this.patientSearchTB.TabIndex = 1;
            this.patientSearchTB.TextChanged += new System.EventHandler(this.TextBox_TextChanged);
            this.patientSearchTB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SearchTextBox_KeyDown);
            this.patientSearchTB.Leave += new System.EventHandler(this.FindPerson);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Found patients";
            // 
            // foundPatients
            // 
            this.foundPatients.FormattingEnabled = true;
            this.foundPatients.Location = new System.Drawing.Point(13, 70);
            this.foundPatients.Name = "foundPatients";
            this.foundPatients.Size = new System.Drawing.Size(522, 95);
            this.foundPatients.TabIndex = 3;
            this.foundPatients.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Exit);
            // 
            // findPatient
            // 
            this.findPatient.Location = new System.Drawing.Point(161, 30);
            this.findPatient.Name = "findPatient";
            this.findPatient.Size = new System.Drawing.Size(75, 23);
            this.findPatient.TabIndex = 0;
            this.findPatient.TabStop = false;
            this.findPatient.Text = "Find";
            this.findPatient.UseVisualStyleBackColor = true;
            this.findPatient.Click += new System.EventHandler(this.FindPerson);
            this.findPatient.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Exit);
            // 
            // findCaregiver
            // 
            this.findCaregiver.Location = new System.Drawing.Point(161, 186);
            this.findCaregiver.Name = "findCaregiver";
            this.findCaregiver.Size = new System.Drawing.Size(75, 23);
            this.findCaregiver.TabIndex = 0;
            this.findCaregiver.TabStop = false;
            this.findCaregiver.Text = "Find";
            this.findCaregiver.UseVisualStyleBackColor = true;
            this.findCaregiver.Click += new System.EventHandler(this.FindPerson);
            this.findCaregiver.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Exit);
            // 
            // foundCaregivers
            // 
            this.foundCaregivers.FormattingEnabled = true;
            this.foundCaregivers.Location = new System.Drawing.Point(13, 226);
            this.foundCaregivers.Name = "foundCaregivers";
            this.foundCaregivers.Size = new System.Drawing.Size(522, 95);
            this.foundCaregivers.TabIndex = 6;
            this.foundCaregivers.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Exit);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 209);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Found caregivers";
            // 
            // caregiverSearchTB
            // 
            this.caregiverSearchTB.Location = new System.Drawing.Point(13, 186);
            this.caregiverSearchTB.Name = "caregiverSearchTB";
            this.caregiverSearchTB.Size = new System.Drawing.Size(142, 20);
            this.caregiverSearchTB.TabIndex = 4;
            this.caregiverSearchTB.TextChanged += new System.EventHandler(this.TextBox_TextChanged);
            this.caregiverSearchTB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SearchTextBox_KeyDown);
            this.caregiverSearchTB.Leave += new System.EventHandler(this.FindPerson);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 170);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(151, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Search for caregiver (Optional)";
            // 
            // select
            // 
            this.select.Location = new System.Drawing.Point(12, 327);
            this.select.Name = "select";
            this.select.Size = new System.Drawing.Size(75, 23);
            this.select.TabIndex = 10;
            this.select.Text = "Select";
            this.select.UseVisualStyleBackColor = true;
            this.select.Click += new System.EventHandler(this.Select);
            this.select.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Exit);
            // 
            // cancel
            // 
            this.cancel.Location = new System.Drawing.Point(460, 335);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(75, 23);
            this.cancel.TabIndex = 11;
            this.cancel.Text = "Cancel";
            this.cancel.UseVisualStyleBackColor = true;
            this.cancel.Click += new System.EventHandler(this.Close_Click);
            // 
            // FindPatientWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(547, 370);
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.select);
            this.Controls.Add(this.findCaregiver);
            this.Controls.Add(this.foundCaregivers);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.caregiverSearchTB);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.findPatient);
            this.Controls.Add(this.foundPatients);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.patientSearchTB);
            this.Controls.Add(this.label1);
            this.Name = "FindPatientWindow";
            this.Text = "Find Patient";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox patientSearchTB;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox foundPatients;
        private System.Windows.Forms.Button findPatient;
        private System.Windows.Forms.Button findCaregiver;
        private System.Windows.Forms.ListBox foundCaregivers;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox caregiverSearchTB;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button select;
        private System.Windows.Forms.Button cancel;
    }
}