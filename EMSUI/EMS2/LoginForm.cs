using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace EMS2
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            // needs the address of server if run on multiple comps
            string connStr = "Password=" + txtPassword.Text + ";Persist Security Info=True;User ID=" + txtUserName.Text + ";Initial Catalog=EMS2";

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    conn.Close();
                }
            }
            catch (SqlException exception)
            {
                MessageBox.Show(exception.Message, "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MainForm main = new MainForm(ConfigurationManager.ConnectionStrings["EMS_Database"]?.ConnectionString);
            main.Closed += (s, args) => this.Close();
            main.Show();
            main.Activate();          
            Hide();
        }
    }
}
