using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        }

    }

}
