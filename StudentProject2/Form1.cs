using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Security.Cryptography;

namespace StudentProject2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            txbDebug.Text = "MainWindow loaded";
            dateEnd.Text = DateTime.Now.ToString("d");
            txbSymbol.Focus();
            txbDebug.Text = "Enter a stock symbol and start date, click \"Load\"";
        }

        private void txbSymbol_Leave(object sender, EventArgs e)
        {
            txbSymbol.Text = txbSymbol.Text.ToUpper();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            txbDebug.Text = $"Symbol: {txbSymbol.Text}; Start Date: {dateStart.Text}; " +
                $"End date: {dateEnd.Text}";

            var conn = new AVConnection("AYZBS9JXW6CTSR3P");  // get your own key at (https://www.alphavantage.co/support/#api-key)
            string sData = conn.SaveCSVfromURL(txbSymbol.Text);
            string[] sep = { "\r\n" };
            string[] sRecords = sData.Split(sep, System.StringSplitOptions.RemoveEmptyEntries);
            lbData.Items.Clear();
            int nCnt = 0;
            foreach (var line in sRecords)
                lbData.Items.Add($"{nCnt++}. {line}");

            var sEODdata = new string[6];
            decimal hiMax = 0m;
            decimal lowMin = 0m;
            decimal low;
            decimal high;
            for(int i = sRecords.Length-1; i > 0; i--)
            {
                sEODdata = sRecords[i].Split(',');                              // sRecord: Date, Open, High, Low, Close, Vol
                    
                if (i == sRecords.Length - 1)
                    lowMin = decimal.Parse(sEODdata[3]);                        // initialize low
                low = decimal.Parse(sEODdata[3]);
                lowMin = low < lowMin ? low : lowMin;
                high = decimal.Parse(sEODdata[2]);
                hiMax = high > hiMax? high : hiMax;

                chart1.Series["s1"].Points.AddXY(sEODdata[0],                   // AddXY: Date, Low, High, Open, Close
                    low, high, decimal.Parse(sEODdata[1]), 
                    decimal.Parse(sEODdata[4]));  
            }
            chart1.ChartAreas[0].AxisY.Minimum = Math.Ceiling((double)lowMin) - 1;
            chart1.ChartAreas[0].AxisY.Maximum = Math.Ceiling((double)hiMax);
            
        }

        public class AVConnection
        {
            private readonly string _apiKey;
            public AVConnection(string apiKey)
            {
                _apiKey = apiKey;
            }
            public string SaveCSVfromURL(string symbol)
            {
                var req = (HttpWebRequest)WebRequest.Create("https://" + $@"www.alphavantage.co/query?function=TIME_SERIES_DAILY&symbol={symbol}&apikey={_apiKey}&datatype=csv");
                var resp = (HttpWebResponse)req.GetResponse();

                var sr = new StreamReader(resp.GetResponseStream());
                string results = sr.ReadToEnd();
                sr.Close();
                return results;
            }
        }
    }

}
