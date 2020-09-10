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
        public string stockDataFolder = ".\\StockData\\";
        public Form1()
        {
            InitializeComponent();
            txbDebug.Text = "MainWindow loaded";
            dateEnd.Text = DateTime.Now.ToString("d");
            txbSymbol.Focus();
            txbDebug.Text = "Enter a stock symbol and start date, click \"Load\"";
            if (!Directory.Exists(stockDataFolder))
                Directory.CreateDirectory(stockDataFolder);
        }

        private void txbSymbol_Leave(object sender, EventArgs e)
        {
            txbSymbol.Text = txbSymbol.Text.ToUpper();
        }

        
        private void btnLoad_Click(object sender, EventArgs e)
        {
            txbDebug.Text = $"Symbol: {txbSymbol.Text}; Start Date: {dateStart.Text}; " +
                $"End date: {dateEnd.Text}";

            // Determine the base filename, which consists of the symbol and todays date
            string fileBase = $"{txbSymbol.Text}_" + DateTime.Now.ToString("d").Replace('/', '-');

            string sData ="empty";  // this will hold our stock data, whether from a file or AlphaVantage
            // If the file exists, read the data from it; otherwise get the data online
            if (File.Exists(stockDataFolder + fileBase + ".csv"))
            {
                var sr = new StreamReader(stockDataFolder + fileBase + ".csv");
                sData = sr.ReadToEnd();
                sr.Close();
            }
            else
            {
                var conn = new AVConnection("AYZBS9JXW6CTSR3P");  // get your own key at (https://www.alphavantage.co/support/#api-key)
                sData = conn.SaveCSVfromURL(txbSymbol.Text);

                // delete older versions
                if (File.Exists(stockDataFolder + txbSymbol.Text + "_*.csv"))
                {
                    string[] old = Directory.GetFiles(stockDataFolder, txbSymbol.Text + "_*.csv");
                    foreach (var item in old)
                        if (item != null)
                            File.Delete(item);
                }
                
                // save a copy
                File.WriteAllText(stockDataFolder + fileBase + ".csv", sData);
            }
            
            // split the data into records of daily data
            string[] sep = { "\r\n" };
            string[] sRecords = sData.Split(sep, System.StringSplitOptions.RemoveEmptyEntries);
            // load into the list box
            lbData.Items.Clear();
            int nCnt = 0;
            foreach (var line in sRecords)
                lbData.Items.Add($"{nCnt++}. {line}");

            // setup variables for getting the lowest low and highest high; use to scale the vertical axis
            var sEODdata = new string[6];
            decimal hiMax = 0m;
            decimal lowMin = 0m;
            decimal low;
            decimal high;
            chart1.Series["s1"].Points.Clear();

            // go through each record to plot it
            var dtStart = DateTime.Parse(dateStart.Text);
            var dtEnd = DateTime.Parse(dateEnd.Text);
            for (int i = sRecords.Length - 1; i > 0; i--)
            {
                // split into fields, ordered: Date, Open, High, Low, Close, Vol
                sEODdata = sRecords[i].Split(',');

                // use only the data requested
                if (DateTime.Parse(sEODdata[0]) < dtStart)
                    continue;
                if (DateTime.Parse(sEODdata[0]) > dtEnd)
                    break;

                // process low and high
                if (lowMin == 0)
                    lowMin = decimal.Parse(sEODdata[3]);    // initialize low
                low = decimal.Parse(sEODdata[3]);
                lowMin = low < lowMin ? low : lowMin;
                high = decimal.Parse(sEODdata[2]);
                hiMax = high > hiMax ? high : hiMax;

                // plot the candle, ordered: Date, Low, High, Open, Close
                chart1.Series["s1"].Points.AddXY(sEODdata[0],
                    low, high, decimal.Parse(sEODdata[1]),
                    decimal.Parse(sEODdata[4]));
            }
            // use the max and min values for Y-axis scaling
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
                //var req = (HttpWebRequest)WebRequest.Create("https://" + $@"www.alphavantage.co/query?function=TIME_SERIES_DAILY&symbol={symbol}&apikey={_apiKey}&datatype=csv");
                var req = (HttpWebRequest)WebRequest.Create("https://" + $@"www.alphavantage.co/query?function=TIME_SERIES_DAILY&symbol={symbol}&outputsize=full&apikey={_apiKey}&datatype=csv");
                var resp = (HttpWebResponse)req.GetResponse();

                var sr = new StreamReader(resp.GetResponseStream());
                string results = sr.ReadToEnd();
                sr.Close();
                return results;
            }
        }

    }

}
