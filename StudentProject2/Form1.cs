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
