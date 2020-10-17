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
using System.Net.Mail;
using System.Net.Http;

namespace StudentProject2
{
    public partial class Form1 : Form
    {
        //public static string stockDataFolder = ".\\StockData\\";    // This is folder where our saved stock data will go, in same folder as .exe
        public static string stockDataFolder = @".\StockData\";    // This is folder where our saved stock data will go, in same folder as .exe
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
            // sort the dates to be in ascending order, then remove the csv header
            var lstTemp = new List<string>();
            lstTemp = ProcessChart.GetStockData(txbSymbol.Text).Reverse().ToList();
            lstTemp.Remove(lstTemp[lstTemp.Count() - 1]);
            ProcessChart.sRecords = lstTemp.ToArray();

            // load into the list box
            lbData.Items.Clear();
            int nCnt = 0;
            foreach (var line in ProcessChart.sRecords)
                lbData.Items.Add($"{nCnt++}. {line}");

            // load into the chart
            ProcessChart.InitChart(dateStart.Text, dateEnd.Text, chart1);
            ProcessChart.PopulateChart();

            //// send SMS
            //var client = new HttpClient();
            //var request = new HttpRequestMessage
            //{
            //    Method = HttpMethod.Post,
            //    RequestUri = new Uri("https://rapidapi.p.rapidapi.com/send"),
            //    Headers =
            //    {
            //        { "x-rapidapi-host", "quick-easy-sms.p.rapidapi.com" },
            //        { "x-rapidapi-key", "2bf61d5653mshc4f54f5afe6adf2p1452eejsn8f312618cab2" },
            //    },
            //    Content = new FormUrlEncodedContent(new Dictionary<string, string>
            //    {
            //        { "message", "message content from RapidAPI" },
            //        { "toNumber", "13173799962" }, //1xxxxxxxxxx
            //    }),
            //};
            //using (var response = await client.SendAsync(request))
            //{
            //    response.EnsureSuccessStatusCode();
            //    var body = await response.Content.ReadAsStringAsync();
            //    Console.WriteLine(body);
            //}
        }

        public static class ProcessChart
        {
            public static DateTime dtStart { get; set; }
            public static DateTime dtEnd { get; set; }
            public static string[] sRecords { get; set; }
            public static string[] sMovAvgRecords1 { get; set; }
            public static string[] sMovAvgRecords2 { get; set; }
            public static Chart chtStockChart { get; set; }

            public static string[] sSignals;        // subset of sRecords containing indicator-generated signals
            const double SCROLLFACT = 0.05;         // factor (% of bars in display) used for scrolling, zooming increment
            static int scrollValue = 0;             // increase this value to scroll right, decrease to scroll left
            static int zoomValue = 0;               // increase this value to zoom out, decrease to zoom in
            static int scrollValPrev;               // fallback value for scroll setting
            static int zoomValPrev;                 // fallback value for zoom setting
            static DateTime dt0;                    // start date for the chart after scroll & zoom
            static DateTime dt1;                    // end date for the chart after scroll & zoom
            static int bars;                        // days between dt0 and dt1
            static decimal hiMax = 0m;              // highest detected value on chart, used for vertical scaling
            static decimal lowMin = 0m;             // lowest detected value on chart, used for vertical scaling
            static string[] sEODdata;               // End-Of-Day stock data
            static int nStartIndex;                 // index of sRecords where chart starts
            public static int nMovAvgPer1 = -1;     // period of moving average 1
            public static int nMovAvgPer2 = -1;     // period of moving average 2

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
                    sData = conn.GetStockDataCSVfromURL(symbol);

                    // delete older versions
                    string[] old = Directory.GetFiles(stockDataFolder, symbol + "_*.csv");
                    if (old.Length > 0)
                    {
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
            public static string[] GetMovAvgData(string symbol, string MovAvgType, int period)
            {
                // Determine the base filename, which consists of the arguments and todays date
                string fileBase = $"{symbol}_{MovAvgType}_{period}_" + DateTime.Now.ToString("d").Replace('/', '-');

                string sData = "empty";  // this will hold our indicator data, whether from a file or AlphaVantage
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
                    sData = conn.GetMA_CSVfromURL(symbol, period, MovAvgType);

                    // delete older versions
                    string[] old = Directory.GetFiles(stockDataFolder, $"{symbol}_{MovAvgType}_{period}_*.csv");
                    if (old.Length > 0)
                    {
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
                decimal low;
                decimal high;
                // setup our daily record, it is ordered: Date, Open, High, Low, Close, Vol
                sEODdata = new string[6];

                GetChartDates();
                chtStockChart.Series["s1"].Points.Clear();
                nStartIndex = -1;

                hiMax = 0;
                lowMin = 0;
                // go through each record to plot it
                for (int i = 0; i < sRecords.Length; i++)
                {
                    // split into fields
                    sEODdata = sRecords[i].Split(',');

                    // use only the data requested
                    if (DateTime.Parse(sEODdata[0]) < dt0)
                        continue;
                    if (DateTime.Parse(sEODdata[0]) > dt1)
                        break;
                    if (nStartIndex == -1)
                        nStartIndex = i;

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

                if(ProcessChart.sMovAvgRecords1 != null)
                    ProcessChart.AddIndicatorToChart(1);
                if(ProcessChart.sMovAvgRecords2 != null)
                    ProcessChart.AddIndicatorToChart(2);
            }

            public static void AddIndicatorToChart(int id)
            {
                // select the chart and data records
                string serMovAvg;
                string[] sMovAvgRecords;
                switch (id)
                {
                    case 2: 
                        serMovAvg = "serMovAvg2";
                        sMovAvgRecords = ProcessChart.sMovAvgRecords2;
                        break;
                    case 1:
                    default:
                        serMovAvg = "serMovAvg1";
                        sMovAvgRecords = ProcessChart.sMovAvgRecords1;
                        break;
               }

                // setup variables for getting the lowest low and highest high; use to scale the vertical axis
                decimal low;
                decimal high;
                // setup our daily record, it is ordered: Date, MA value
                var sMAdata = new string[2];

                chtStockChart.Series[serMovAvg].Points.Clear();
                if (sMovAvgRecords == null)
                    return;

                // go through each record to plot it
                for (int i = 0; i < sMovAvgRecords.Length; i++)
                {
                    // split into fields
                    sMAdata = sMovAvgRecords[i].Split(',');

                    // use only the data requested
                    if (DateTime.Parse(sMAdata[0]) < dt0)
                        continue;
                    if (DateTime.Parse(sMAdata[0]) > dt1)
                        break;

                    // process low and high
                    low = decimal.Parse(sMAdata[1]);
                    lowMin = low < lowMin ? low : lowMin;
                    high = decimal.Parse(sMAdata[1]);
                    hiMax = high > hiMax ? high : hiMax;

                    // plot the line, ordered: Date, value
                    chtStockChart.Series[serMovAvg].Points.AddXY(sMAdata[0], decimal.Parse(sMAdata[1]));
                }
                
                chtStockChart.AlignDataPointsByAxisLabel($"s1, {serMovAvg}");
                if(id == 2 && ProcessChart.sMovAvgRecords1 != null)
                    chtStockChart.AlignDataPointsByAxisLabel("s1, serMovAvg1");

                // use the max and min values from above for Y-axis scaling
                chtStockChart.ChartAreas[0].AxisY.Minimum = Math.Ceiling((double)lowMin) - 1;
                chtStockChart.ChartAreas[0].AxisY.Maximum = Math.Ceiling((double)hiMax);

                // if this is the only indicator, find crossings between it and the stock close
                // otherwise find crossings between the two indicators
                string sSlow;
                string sFast;
                int nYindex;
                if(ProcessChart.sMovAvgRecords1 == null || ProcessChart.sMovAvgRecords2 == null)
                {
                    sFast = serMovAvg;
                    sSlow = "s1";
                    nYindex = 3; // if we're comparing to the stock value, the close is found in .YValues[3]
                }
                else if(ProcessChart.nMovAvgPer1 <= ProcessChart.nMovAvgPer2)
                {
                    sFast = "serMovAvg1";
                    sSlow = "serMovAvg2";
                    nYindex = 0; // if we're comparing to an indicator, the value is found in .YValues[0]
                }
                else
                {
                    sFast = "serMovAvg2";
                    sSlow = "serMovAvg1";
                    nYindex = 0; // if we're comparing to an indicator, the value is found in .YValues[0]
                }

                var lstSignals = new List<string>();
                bool bFastOverSlow = chtStockChart.Series[sFast].Points[0].YValues[0] >= chtStockChart.Series[sSlow].Points[0].YValues[nYindex];
                bool bTemp;
                int nMaxPoints = Math.Min(chtStockChart.Series[sFast].Points.Count(), chtStockChart.Series[sSlow].Points.Count());
                for (int i = 1; i < nMaxPoints; i++)
                {
                    bTemp = chtStockChart.Series[sFast].Points[i].YValues[0] >= chtStockChart.Series[sSlow].Points[i].YValues[nYindex];
                    if (bFastOverSlow != bTemp)
                    {
                        bFastOverSlow = bTemp;
                        string sDirection = bFastOverSlow ? " CrossOver" : " CrossUnder";
                        lstSignals.Add(sRecords[nStartIndex + i] + sDirection);
                    }
                }
                sSignals = lstSignals.ToArray();
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
                var sEarliest = sRecords[0].Split(',');
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
                // in the chart, which changes with zoom.  Keyboard repeat can be faster than chart re-draw.
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
            }
        }

        public class AVConnection
        {
            private readonly string _apiKey;
            public AVConnection(string apiKey)
            {
                _apiKey = apiKey;
            }
            public string GetStockDataCSVfromURL(string symbol)
            {
                var req = (HttpWebRequest)WebRequest.Create("https://" + $@"www.alphavantage.co/query?" +
                                                            $"function=TIME_SERIES_DAILY&" +
                                                            $"symbol={symbol}&" +
                                                            $"outputsize=full&" +
                                                            $"apikey={_apiKey}&" +
                                                            $"datatype=csv");

                var resp = (HttpWebResponse)req.GetResponse();

                var sr = new StreamReader(resp.GetResponseStream());
                string results = sr.ReadToEnd();
                sr.Close();
                return results;
            }
            public string GetMA_CSVfromURL(string symbol, int period, string movingAvgType = "SMA")
            {
                var req = (HttpWebRequest)WebRequest.Create("https://" + $"www.alphavantage.co/query?" +
                                                            $"function={movingAvgType}&" +
                                                            $"symbol={symbol}&"  +
                                                            $"interval=daily&"   +
                                                            $"time_period={period}&" +
                                                            $"series_type=close&"    +
                                                            $"apikey={_apiKey}&" +
                                                            $"datatype=csv");

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
                // refresh the crossover data in the listbox
                if (ProcessChart.sSignals != null)
                {
                    lbData.Items.Clear();
                    foreach (var line in ProcessChart.sSignals)
                        lbData.Items.Add(line);
                }
            }
        }

        private void OnBnMovAvg1(object sender, EventArgs e)
        {
            OnBnMovAvg(1, cmbMovAvg1, txbMovAvg1, ProcessChart.sMovAvgRecords1);
        }

        private void OnBnMovAvg2(object sender, EventArgs e)
        {

            OnBnMovAvg(2, cmbMovAvg2, txbMovAvg2, ProcessChart.sMovAvgRecords2);
        }

        private void OnBnMovAvg(int id, ComboBox cmbMovAvg, TextBox txbMovAvg, string[] sMovAvgRecords)
        {
            int period;
            // validate settings
            if(ProcessChart.sRecords == null)
            {
                txbSymbol.Focus();
                return;
            }

            if(cmbMovAvg.Text == "")
            {
                if (sMovAvgRecords != null)
                {
                    txbMovAvg.Text = "";
                    switch (id)
                    {
                        case 1: 
                            ProcessChart.sMovAvgRecords1 = null;
                            ProcessChart.nMovAvgPer1 = -1;
                            break;
                        case 2: 
                            ProcessChart.sMovAvgRecords2 = null; 
                            ProcessChart.nMovAvgPer2 = -1;
                            break;
                    }
                    ProcessChart.AddIndicatorToChart(id);
                    lbData.Items.Clear();
                }
                cmbMovAvg.Focus();
                return;
            }
            if (txbMovAvg.Text == "")
            {
                txbMovAvg.Focus();
                return;
            }
            if (Int32.TryParse(txbMovAvg.Text, out period) == false)
            {
                txbMovAvg.Text = "";
                txbMovAvg.Focus();
                return;
            }

            // get the data
            // sort the dates to be in ascending order, then remove the csv header
            var lstTemp = new List<string>();
            lstTemp = ProcessChart.GetMovAvgData(txbSymbol.Text, cmbMovAvg.Text, period).Reverse().ToList();
            lstTemp.Remove(lstTemp[lstTemp.Count() - 1]);
            switch (id)
            {
                case 1: 
                    ProcessChart.sMovAvgRecords1 = lstTemp.ToArray();
                    ProcessChart.nMovAvgPer1 = period;
                    break;
                case 2: 
                    ProcessChart.sMovAvgRecords2 = lstTemp.ToArray(); 
                    ProcessChart.nMovAvgPer2 = period;
                    break;
            }
                

            // load into the list box
            lbData.Items.Clear();
            int nCnt = 0;
            foreach (var line in ProcessChart.sRecords)
                lbData.Items.Add($"{nCnt++}. {line}");

            // load into the chart
            ProcessChart.AddIndicatorToChart(id);

            // load signals into the list box
            lbData.Items.Clear();
            foreach (var line in ProcessChart.sSignals)
                lbData.Items.Add(line);

        }
    }

}
