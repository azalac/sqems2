namespace EMS2
{
    partial class AppointmentBilling
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
            this.appointmentInfo = new System.Windows.Forms.Label();
            this.billingCodeBox = new System.Windows.Forms.ListBox();
            this.addBillingCode = new System.Windows.Forms.Button();
            this.removeBillingCode = new System.Windows.Forms.Button();
            this.billingFinish = new System.Windows.Forms.Button();
            this.billingCodeTB = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.recallCB = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Appointment Info:";
            // 
            // appointmentInfo
            // 
            this.appointmentInfo.AutoSize = true;
            this.appointmentInfo.Location = new System.Drawing.Point(12, 37);
            this.appointmentInfo.Name = "appointmentInfo";
            this.appointmentInfo.Size = new System.Drawing.Size(0, 13);
            this.appointmentInfo.TabIndex = 1;
            // 
            // billingCodeBox
            // 
            this.billingCodeBox.FormattingEnabled = true;
            this.billingCodeBox.Location = new System.Drawing.Point(15, 180);
            this.billingCodeBox.Name = "billingCodeBox";
            this.billingCodeBox.Size = new System.Drawing.Size(188, 121);
            this.billingCodeBox.TabIndex = 2;
            // 
            // addBillingCode
            // 
            this.addBillingCode.Location = new System.Drawing.Point(128, 151);
            this.addBillingCode.Name = "addBillingCode";
            this.addBillingCode.Size = new System.Drawing.Size(75, 23);
            this.addBillingCode.TabIndex = 3;
            this.addBillingCode.Text = "Add";
            this.addBillingCode.UseVisualStyleBackColor = true;
            this.addBillingCode.Click += new System.EventHandler(this.Add_Click);
            // 
            // removeBillingCode
            // 
            this.removeBillingCode.Location = new System.Drawing.Point(15, 307);
            this.removeBillingCode.Name = "removeBillingCode";
            this.removeBillingCode.Size = new System.Drawing.Size(75, 23);
            this.removeBillingCode.TabIndex = 4;
            this.removeBillingCode.Text = "Remove";
            this.removeBillingCode.UseVisualStyleBackColor = true;
            this.removeBillingCode.Click += new System.EventHandler(this.Remove_Click);
            // 
            // billingFinish
            // 
            this.billingFinish.Location = new System.Drawing.Point(132, 349);
            this.billingFinish.Name = "billingFinish";
            this.billingFinish.Size = new System.Drawing.Size(75, 23);
            this.billingFinish.TabIndex = 5;
            this.billingFinish.Text = "Finish";
            this.billingFinish.UseVisualStyleBackColor = true;
            this.billingFinish.Click += new System.EventHandler(this.Finish_Click);
            // 
            // billingCodeTB
            // 
            this.billingCodeTB.Location = new System.Drawing.Point(15, 153);
            this.billingCodeTB.Name = "billingCodeTB";
            this.billingCodeTB.Size = new System.Drawing.Size(100, 20);
            this.billingCodeTB.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 137);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Billing Code";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 354);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Recall";
            // 
            // recallCB
            // 
            this.recallCB.FormattingEnabled = true;
            this.recallCB.Items.AddRange(new object[] {
            "None",
            "1 Week",
            "2 Weeks",
            "3 Weeks"});
            this.recallCB.Location = new System.Drawing.Point(57, 351);
            this.recallCB.Name = "recallCB";
            this.recallCB.Size = new System.Drawing.Size(69, 21);
            this.recallCB.TabIndex = 9;
            // 
            // appointmentBilling
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(219, 379);
            this.Controls.Add(this.recallCB);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.billingCodeTB);
            this.Controls.Add(this.billingFinish);
            this.Controls.Add(this.removeBillingCode);
            this.Controls.Add(this.addBillingCode);
            this.Controls.Add(this.billingCodeBox);
            this.Controls.Add(this.appointmentInfo);
            this.Controls.Add(this.label1);
            this.Name = "appointmentBilling";
            this.Text = "appointmentBilling";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label appointmentInfo;
        private System.Windows.Forms.ListBox billingCodeBox;
        private System.Windows.Forms.Button addBillingCode;
        private System.Windows.Forms.Button removeBillingCode;
        private System.Windows.Forms.Button billingFinish;
        private System.Windows.Forms.TextBox billingCodeTB;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox recallCB;
    }
}