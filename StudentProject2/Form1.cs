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
        public enum arrowKey { Left = 37, Up, Right, Down };
        public arrowKey inKey;
        public Form1()
        {
            InitializeComponent();
            KeyPreview = true;  // this allows the form to capture keystrokes
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
            //string[] sRecords = ProcessChart.GetStockData(txbSymbol.Text);
            ProcessChart.sRecords = ProcessChart.GetStockData(txbSymbol.Text);

            // load into the list box
            lbData.Items.Clear();
            int nCnt = 0;
            foreach (var line in ProcessChart.sRecords)
                lbData.Items.Add($"{nCnt++}. {line}");

            // load into the chart
            ProcessChart.InitChart(dateStart.Text, dateEnd.Text, chart1);
            ProcessChart.PopulateChart();
        }

        public static class ProcessChart
        {
            public static DateTime dtStart { get; set; }
            public static DateTime dtEnd { get; set; }
            public static string[] sRecords { get; set; }
            public static Chart chtStockChart { get; set; }

            public static int scrollValue = 0;      // increase this value to scroll right, decrease to scroll left
                                                    // the range is +/- (1/SCROLLFACT)
            public const double SCROLLFACT = 0.05;  // factor (% of bars in display) used for scrolling increment
            public static int zoomValue = 0;        // increase this value to zoom out, decrease to zoom in
            static int scrollValPrev;               // fallback value for scroll setting
            static int zoomValPrev;                 // fallback value for zoom setting
            static int barsPrev;                    // fallback value for bars setting
            static DateTime dt0;                    // start date for the chart after scroll & zoom
            static DateTime dt1;                    // end date for the chart after scroll & zoom
            static int bars;                        // days between dt0 and dt1
            
            public static void InitChart(string startDate, string endDate, Chart chartRef)
            {
                dtStart = DateTime.Parse(startDate);
                dtEnd = DateTime.Parse(endDate);
                chtStockChart = chartRef;
                scrollValue = 0;
                zoomValue = 0;
            }
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
            public static void PopulateChart()
            {
                // setup variables for getting the lowest low and highest high; use to scale the vertical axis
                decimal hiMax = 0m;
                decimal lowMin = 0m;
                decimal low;
                decimal high;
                // setup our daily record, it is ordered: Date, Open, High, Low, Close, Vol
                var sEODdata = new string[6];

                GetChartDates();
                chtStockChart.Series["s1"].Points.Clear();

                // go through each record to plot it
                for (int i = sRecords.Length - 1; i > 0; i--)
                {
                    // split into fields
                    sEODdata = sRecords[i].Split(',');

                    // use only the data requested
                    if (DateTime.Parse(sEODdata[0]) < dt0)
                        continue;
                    if (DateTime.Parse(sEODdata[0]) > dt1)
                        break;

                    // process low and high
                    if (lowMin == 0)
                        lowMin = decimal.Parse(sEODdata[3]);    // initialize low
                    low = decimal.Parse(sEODdata[3]);
                    lowMin = low < lowMin ? low : lowMin;
                    high = decimal.Parse(sEODdata[2]);
                    hiMax = high > hiMax ? high : hiMax;

                    // plot the candle, ordered: Date, Low, High, Open, Close
                    chtStockChart.Series["s1"].Points.AddXY(sEODdata[0],
                        low, high, decimal.Parse(sEODdata[1]),
                        decimal.Parse(sEODdata[4]));
                }
                // use the max and min values from above for Y-axis scaling
                chtStockChart.ChartAreas[0].AxisY.Minimum = Math.Ceiling((double)lowMin) - 1;
                chtStockChart.ChartAreas[0].AxisY.Maximum = Math.Ceiling((double)hiMax);
            }

            public static void SetPanAndZoom(arrowKey inArrow)
            {
                // This function is called whenever one of the arrow keys is pressed.
                switch (inArrow)
                {
                    case arrowKey.Left: --scrollValue ; break;
                    case arrowKey.Up:   --zoomValue; break;
                    case arrowKey.Right: ++scrollValue; break;
                    case arrowKey.Down:  ++zoomValue; break;
                }
                PopulateChart();
            }
            static void CalculateDates()
            {
                bars = (dt1 - dt0).Days;
                try  // sometimes a zoom-out will cause an illegal date
                {
                    dt0 = dtStart.AddDays(Math.Ceiling(bars * (scrollValue * SCROLLFACT - zoomValue * SCROLLFACT * 0.5)));
                    dt1 = dtEnd.AddDays(Math.Floor(bars * (scrollValue * SCROLLFACT + zoomValue * SCROLLFACT * 0.5)));
                }
                catch { }  // just ignore the calculation and wait for different input
            }
            static void GetChartDates()
            {
                // adjust chart based on user input

                // get the earliest date
                var sEarliest = sRecords[sRecords.Length - 1].Split(',');
                DateTime earliestDate = DateTime.Parse(sEarliest[0]);

                if (dt0 == DateTime.Parse("1/1/001"))   // dt0, dt1 not initialized yet
                {
                    dt0 = dtStart;
                    dt1 = dtEnd;
                }
                if ((dt1 - dt0).Days != 0)      // quick error-checking
                    bars = (dt1 - dt0).Days;
                CalculateDates();

                // As user input changes the zoom and scroll, especially at wide zoom settings, the bounding dates
                // of the chart can quickly exceed the available data.  This loop brings the settings back into range.
                // An iterative approach is sometimes needed since increment amounts are based on the number of bars
                // in the chart, which changes with zoom
                int tryCnt = 5;
                while (--tryCnt >= 0 && (dt1 > DateTime.Now.Date || dt0 < earliestDate || dt0 >= dt1))
                {
                    // if there's too much zoom (less than 3 bars showing), zoom out
                    if (bars <= 3)
                    {
                        --zoomValue;
                        CalculateDates();
                    }
                    // in case the end date goes past today, move the chart to the left and recalculate
                    if (dt1 > DateTime.Now.Date)
                    {
                        --scrollValue;
                        CalculateDates();
                        if (dt0 < earliestDate)
                        {
                            --zoomValue;
                            CalculateDates();
                        }
                    }
                    // in case the start date goes before the earliest, move the chart to the right and recalculate
                    if (dt0 < earliestDate)
                    {
                        ++scrollValue;
                        CalculateDates();
                        if (dt1 > DateTime.Now.Date)
                        {
                            --zoomValue;
                            CalculateDates();
                        }
                    }
                }
                // if the attempted fixes didn't work, reset everything
                if (dt1 > DateTime.Now.Date || dt0 < earliestDate || dt0 >= dt1)
                {
                    dt0 = dtStart;
                    dt1 = dtEnd;
                    bars = (dt1 - dt0).Days;
                    scrollValPrev = scrollValue = 0;
                    zoomValPrev = zoomValue = 0;
                }
                // save the results
                scrollValPrev = scrollValue;
                zoomValPrev = zoomValue;
                barsPrev = bars;
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

        private void OnPreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            e.IsInputKey = true;    // needed to detect the arrow keys
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyValue >= 37 && e.KeyValue <= 40)
            {
                inKey = (arrowKey)e.KeyValue;
                e.Handled = true;
                ProcessChart.SetPanAndZoom(inKey);
            }
        }
    }

}
