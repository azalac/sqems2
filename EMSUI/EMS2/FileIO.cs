using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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



        public string openFile()
        {
            OpenFileDialog openFile = new OpenFileDialog();
            string filePath = string.Empty;
            // = "Text File|*.txt|PNG File|*.png     ------------------ multiple file types?
            openFile.Filter = "Text File|*.txt";

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                filePath = openFile.FileName;
                //shows file name without path
                //MessageBox.Show(openFile.SafeFileName);
            }
            return filePath;
        }






        public List<string> ReadReconcile(string path)
        {
            Regex regex = new Regex("^[0-9]{18}[a-zA-Z]{4}[0-9]{14}[a-zA-Z]{4}$");
            List<string> temp = new List<string>();
            List<string> data = new List<string>();


            try
            {
                temp = File.ReadLines(path).ToList();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            foreach (string s in temp)
            {
                if (regex.Match(s).Success)
                {
                    data.Add(s);
                }
            }

            return data;
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
