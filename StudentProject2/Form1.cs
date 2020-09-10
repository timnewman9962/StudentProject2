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
using System.Windows.Forms.DataVisualization.Charting;

namespace StudentProject2
{
    public partial class Form1 : Form
    {
        public static string stockDataFolder = ".\\StockData\\";    // This is folder where our saved stock data will go, in same folder as .exe
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

            // get the stock data as an array of strings
            if(txbSymbol.Text == "STOCK / INDEX")
            {
                MessageBox.Show("Please enter a Stock or Index symbol","Missing Input");
                txbSymbol.Focus();
                return;
            }
            if(dateStart.Text == dateEnd.Text)
            {
                MessageBox.Show("Start and End dates must be different","Missing Input");
                dateStart.Focus();
                return;
            }
            string[] sRecords = ProcessChart.GetStockData(txbSymbol.Text);

            // load into the list box
            lbData.Items.Clear();
            int nCnt = 0;
            foreach (var line in sRecords)
                lbData.Items.Add($"{nCnt++}. {line}");

            // load into the chart
            ProcessChart.PopulateChart(sRecords, dateStart.Text, dateEnd.Text, chart1);
        }

        public static class ProcessChart
        {
            public static int panValue = 0;    // increase this value to scroll right, decrease to scroll left
            public static int zoomValue = 0;   // increase this value to zoom out, decrease to zoom in
            
            public static string[] GetStockData(string symbol)
            {
                // Determine the base filename, which consists of the symbol and todays date
                string fileBase = $"{symbol}_" + DateTime.Now.ToString("d").Replace('/', '-');

                string sData = "empty";  // this will hold our stock data, whether from a file or AlphaVantage
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
                    sData = conn.SaveCSVfromURL(symbol);

                    // delete older versions
                    if (File.Exists(stockDataFolder + symbol + "_*.csv"))
                    {
                        string[] old = Directory.GetFiles(stockDataFolder, symbol + "_*.csv");
                        foreach (var item in old)
                            if (item != null)
                                File.Delete(item);
                    }
                    // save a copy
                    File.WriteAllText(stockDataFolder + fileBase + ".csv", sData);
                }

                // split the data into records of daily data
                string[] sep = { "\r\n" };
                return sData.Split(sep, System.StringSplitOptions.RemoveEmptyEntries);
            }

            public static void PopulateChart(string[] records, string begin, string end, Chart chart1)
            {
                // convert our 'string' dates into 'DateTime' so they can be compared
                var dtStart = DateTime.Parse(begin);
                var dtEnd = DateTime.Parse(end);
                // setup variables for getting the lowest low and highest high; use to scale the vertical axis
                decimal hiMax = 0m;
                decimal lowMin = 0m;
                decimal low;
                decimal high;
                // setup our daily record, it is ordered: Date, Open, High, Low, Close, Vol
                var sEODdata = new string[6];

                // adjust chart based on user input
                dtStart.AddDays(panValue - zoomValue);
                dtEnd.AddDays(panValue + zoomValue);

                chart1.Series["s1"].Points.Clear();

                // go through each record to plot it
                for (int i = records.Length - 1; i > 0; i--)
                {
                    // split into fields
                    sEODdata = records[i].Split(',');

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
                // use the max and min values from above for Y-axis scaling
                chart1.ChartAreas[0].AxisY.Minimum = Math.Ceiling((double)lowMin) - 1;
                chart1.ChartAreas[0].AxisY.Maximum = Math.Ceiling((double)hiMax);
            }

            public static void SetPanAndZoom()
            {
                // This function needs to be called whenever one of the arrow keys is pressed.
                // It is anticipated that the pan or scroll function will consider the number
                // of bars on the chart, so that one key press will yield a visually incremental
                // movement of the plot (possibly 5% or so).
                // Likewise, a zoom increment should also consider the number of bars currently displayed.
                // This function must also comprehend the date limits, so that an offset past the end
                // of the chart is not possible.
            }

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
