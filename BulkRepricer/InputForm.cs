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
            string sku;
            double adjusted_dn;
            double shipping = 0; //Need to set for 0 for shipping logic
            string template = "NULL"; //Need some kind of value for the string split
            double commRate = .15;
            double price2 = 0;
            double profitMargin;
            double PM_Lower_Range;
            double PM_Upper_Range;
            string[] workingArray = new string[5];
            string date = DateTime.Today.ToString();
            string mydocpath = System.IO.Path.Combine(@"C:\Users\ashleyd\Documents\","Bulkrepricer_Test_.txt");
            System.IO.StreamReader file = new System.IO.StreamReader(fileName);
            string headers = "sku\tprice\tshipping";
            string output;
            using (StreamWriter outputFile = new System.IO.StreamWriter(mydocpath))
            {
                outputFile.WriteLine(headers);
            }

            while ((line = file.ReadLine()) != null)
            {
                //Parses line and stores all values from one line in to variables for calculation
                workingArray = line.Split('\t'); //should work on tab delimited files
                sku = workingArray[0];
                adjusted_dn = Double.Parse(workingArray[1]);
                if (!string.IsNullOrEmpty(workingArray[2])) { shipping = Double.Parse(workingArray[2]); }
                if (!string.IsNullOrEmpty(workingArray[3])) { template = workingArray[3]; }
                profitMargin = Double.Parse(workingArray[4]);
                PM_Lower_Range = profitMargin;
                PM_Upper_Range = profitMargin + .0014;
                label3.Text = "The price in the file is total price including shipping.";
                

                if (!string.IsNullOrEmpty(workingArray[2]))
                {
                    double tempTotal = adjusted_dn + shipping;
                    double commission = tempTotal * commRate;
                    double total = (tempTotal + commission);
                    price2 = total + (total * .25);
                    double PM = (price2 - total) / price2;
                    
                    while ((PM < PM_Lower_Range) || (PM > PM_Upper_Range))
                    {
                        if (PM > PM_Upper_Range)
                        {
                            price2 = price2 - .01;
                            commission = Math.Round((price2 * commRate), 2);
                            total = tempTotal + commission;
                            PM = (price2 - total) / price2;
                        }
                        else if (PM < PM_Lower_Range)
                        {
                            price2 = price2 + .01;
                            commission = Math.Round((price2 * commRate), 2);
                            total = tempTotal + commission;
                            PM = (price2 - total) / price2;
                        }
                        price2 = Math.Round(price2, 2);
                    }
                    price2 = Math.Round(price2, 2);
                    label2.Text = "Shipping is included in price, so subtraction not needed for floor in local.";
                }
                else if (!string.IsNullOrEmpty(workingArray[3]))
                {
                    double commission2 = adjusted_dn * commRate;
                    double total2 = (adjusted_dn + commission2);
                    string templateType = template;
                    double currShipping = template_values(total2, templateType);
                    total2 = total2 + currShipping;
                    price2 = total2 + (total2 * .25);
                    double PM = (price2 - total2) / price2;

                    while ((PM < PM_Lower_Range) || (PM > PM_Upper_Range))
                    {
                        if (PM >= PM_Upper_Range)
                        {
                            price2 = price2 - shipping;
                            price2 = price2 - .01;
                            shipping = template_values(price2, templateType);
                            price2 = price2 + shipping;
                            commission2 = Math.Round((price2 * commRate), 2);
                            total2 = commission2 + adjusted_dn + shipping;
                            PM = (price2 - total2) / price2;
                        }
                        else if (PM <= PM_Lower_Range)
                        {
                            price2 = price2 - shipping;
                            price2 = price2 + .01;
                            shipping = template_values(price2, templateType);
                            price2 = price2 + shipping;
                            commission2 = Math.Round((price2 * commRate), 2);
                            total2 = commission2 + adjusted_dn + shipping;
                            PM = (price2 - total2) / price2;
                        }
                    }

                    price2 = Math.Round(price2, 2);
                    double finalPrice = price2 - shipping;
                    label2.Text = "Make sure you subtract shipping template costs from price in file";
                }
                else
                {
                    label1.Text = "Please make sure all values that are needed exist";
                }

                using (System.IO.StreamWriter outputfile =
                        new System.IO.StreamWriter(mydocpath, true))
                {
                    output = String.Format("{0}\t{1}\t{2}", sku, price2.ToString(), shipping.ToString());
                    outputfile.WriteLine(output);
                }
            }
            file.Close();
            label1.Text = "The program has finished running";
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

        private double template_values(double price, string template)
        {
            if (template == "Main-Products-ShippingAddedByTemplate")
            {
                if (price <= 10) { return 8.99; }
                else if (price > 10 && price <= 20) { return 8.99; }
                else if (price > 20 && price <= 30) { return 10.99; }
                else if (price > 30 && price <= 50) { return 12.99; }
                else if (price > 50 && price <= 75) { return 17.99; }
                else { return 20.99; }
            }
            else
            {
                if (price <= 10) { return 7.99; }
                else { return 8.99; }
            }
            
        }

    }
}
