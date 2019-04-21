using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EMS2
{
    class FileIO
    {
        private readonly string saveLocation = "../../../../monthlySummaries";


        public void SaveToFile(string data, string file)
        {
            try
            {
                File.WriteAllText(string.Format("{0}/{1}.txt", saveLocation, file), data);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }



        public string ReadFromFile(string file)
        {
            string data = null;

            try
            {
                data = File.ReadAllText(string.Format("{0}/{1}.txt", saveLocation, file));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            return data;
        }
    }
}
