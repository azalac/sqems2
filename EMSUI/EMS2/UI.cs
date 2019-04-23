using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Text.RegularExpressions;

namespace EMS2
{
    class UI
    {

        private Color red = Color.Red;
        private Color black = Color.Black;

        private bool clearing;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="update"></param>
        public void ClearPatientInfo(Dictionary<TextBox, Label> textLabels, Dictionary<ComboBox, Label> comboBoxLabels, bool clearHCN = true)
        {
            clearing = true;

            int i = 0;

            foreach (KeyValuePair<TextBox, Label> textLabel in textLabels)
            {
                if (i != 0 || clearHCN == true)
                {
                    textLabel.Key.Text = "";
                    textLabel.Value.ForeColor = black;
                }
                i = 1;
            }

            foreach (KeyValuePair<ComboBox, Label> comboBoxLabel in comboBoxLabels)
            {
                comboBoxLabel.Value.ForeColor = black;
            }

            clearing = false;
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="tb"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public int CheckNull(TextBox tb, Label label)
        {
            int ret = 0;

            if (clearing == false)
            {
                if (tb.Text == "")
                {
                    label.ForeColor = red;
                    ret = 1;
                }
                else
                {
                    label.ForeColor = black;
                    ret = 0;
                }
            }

            return ret;
        }


        public int Validate(TextBox tb, Label label)
        {
            int ret = 0;
            Regex regex = null;

            if (!tb.Text.Equals(""))
            {
                switch (label.Text)
                {
                    case "Health Card #":
                    case "Head Of Household":
                        tb.Text = tb.Text.ToUpper();
                        regex = new Regex(@"^[0-9]{10,10}[A-Z]{2,2}$");
                        break;
                    case "Last Name":
                    case "First Name":
                        tb.Text = (tb.Text.Length > 40) ? tb.Text.Substring(0, 40) : tb.Text;
                        regex = new Regex(@"^[-'a-zA-Z]+$");
                        break;
                    case "Middle Initial":
                        tb.Text = tb.Text.Substring(0, 1).ToUpper();
                        regex = new Regex(@"^[a-zA-Z]$");
                        break;
                    case "Address Line 1":
                        tb.Text = (tb.Text.Length > 40) ? tb.Text.Substring(0, 40) : tb.Text;
                        regex = new Regex("^[0-9]{1,}[-]?[0-9]* [a-zA-Z'-]{1,}" +
                                          " [a-zA-Z.]{1,}[ ]?[eEwWnNsS.]{0,2}$");
                        break;
                    case "Address Line 2":
                        tb.Text = (tb.Text.Length > 40) ? tb.Text.Substring(0, 40) : tb.Text;
                        regex = new Regex(@"^[.a-zA-Z0-9\s]+$");
                        break;
                    case "City":
                        tb.Text = (tb.Text.Length > 40) ? tb.Text.Substring(0, 40) : tb.Text;
                        regex = new Regex(@"^[.\'a-zA-Z\s-]+$");
                        break;
                    case "Phone Number":
                        if (tb.Text.Length == 10)
                        {
                            tb.Text = tb.Text.Substring(0, 3) + "-" + tb.Text.Substring(3, 3) + "-" + tb.Text.Substring(6);
                        }
                        tb.Text.Replace(" ", "-");
                        regex = new Regex("[0-9]{3,3}-[0-9]{3,3}-[0-9]{4,4}");
                        break;
                    default:
                        regex = null;
                        break;
                }
            }

            if (regex != null)
            {
                if (!regex.Match(tb.Text).Success)
                {
                    label.ForeColor = red;
                    ret = 1;
                }
                else
                {
                    label.ForeColor = black;
                    ret = 0;
                }
            }

            return ret;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="update"></param>
        /// <returns></returns>
        public int CheckNullInfo(Dictionary<TextBox, Label> personTextLabels, Dictionary<TextBox, Label> headTextLabels, bool update = false)
        {
            int error = 0;

            int i = 0;

            foreach (KeyValuePair<TextBox, Label> textLabel in personTextLabels)
            {
                error += CheckNull(textLabel.Key, textLabel.Value);
            }


            if (headTextLabels.ElementAt(0).Key.Text == "" || update == true)
            {
                foreach (KeyValuePair<TextBox, Label> textLabel in headTextLabels)
                {
                    if (i != 0)
                    {
                        error += CheckNull(textLabel.Key, textLabel.Value);
                    }
                    i = 1;
                }
            }

            return error;
        }

        public int ValidateInfo(Dictionary<TextBox, Label> personTextLabels, Dictionary<TextBox, Label> headTextLabels, bool update = false)
        {
            int error = 0;

            int i = 0;

            foreach (KeyValuePair<TextBox, Label> textLabel in personTextLabels)
            {
                error += Validate(textLabel.Key, textLabel.Value);
            }


            if (headTextLabels.ElementAt(0).Key.Text == "" || update == true)
            {
                foreach (KeyValuePair<TextBox, Label> textLabel in headTextLabels)
                {
                    if (i != 0)
                    {
                        error += Validate(textLabel.Key, textLabel.Value);
                    }
                    i = 1;
                }
            }

            return error;
        }
    

        public bool HouseHeadText_Changed(TextBox headOfHouseTextBox, Dictionary<TextBox, Label> headTextLabels)
        {
            bool ret = false;
            int i = 0;

            if (headOfHouseTextBox.Text != "")
            {

                foreach (KeyValuePair<TextBox, Label> textLabel in headTextLabels)
                {
                    if (i != 0)
                    {
                        textLabel.Key.Enabled = false;
                        textLabel.Value.ForeColor = black;
                    }
                    i = 1;
                }
            }
            else
            {
                foreach (KeyValuePair<TextBox, Label> textLabel in headTextLabels)
                {
                    
                    if (i != 0)
                    {
                        textLabel.Key.Enabled = true;
                        CheckNull(textLabel.Key, textLabel.Value);
                    }
                    else
                    {
                        textLabel.Value.ForeColor = black;
                        i = 1;
                    }
                }

                ret = true;
            }

            return ret;
        }
    }
}
