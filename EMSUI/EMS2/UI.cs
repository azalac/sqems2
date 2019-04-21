using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

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
        public void ClearPatientInfo(Dictionary<TextBox, Label> textLabels, Dictionary<ComboBox, Label> comboBoxLabels)
        {
            clearing = true;

            foreach (KeyValuePair<TextBox, Label> textLabel in textLabels)
            {
                textLabel.Key.Text = "";
                textLabel.Value.ForeColor = black;
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
