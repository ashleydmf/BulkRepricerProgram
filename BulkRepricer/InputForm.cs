using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace BulkRepricer
{
    public partial class InputForm : Form
    {
        public InputForm()
        {
            InitializeComponent();
        }

        public void ChooseFolder()
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
            }
        }

        public void ParseText(string fileName)
        {
            string line;
            string[] workingArray = new string[4];
            System.IO.StreamReader file = new System.IO.StreamReader(fileName);
            //System.IO.StreamWriter outputFile = new System.IO.StreamWriter("Bulkrepricer_" + DateTime.Today + "_.txt");

            while ((line = file.ReadLine()) != null)
            {
                workingArray = line.Split(' ');

            }
            
            file.Close();
        }

        private static String WildCardToRegular(String value)
        {
            return "^" + Regex.Escape(value).Replace("\\*", ".*") + "$";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ChooseFolder();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string filePath = textBox1.Text;
            Boolean isText = Regex.IsMatch(filePath, WildCardToRegular("*txt"));

            if (!isText)
            {
                label1.Text = "Please select a .txt file";
            }
            else
            {
                ParseText(filePath);
            }
        }
    }
}
