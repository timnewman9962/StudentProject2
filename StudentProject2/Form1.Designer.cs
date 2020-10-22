namespace StudentProject2
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series5 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series6 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series7 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series8 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.txbSymbol = new System.Windows.Forms.TextBox();
            this.dateStart = new System.Windows.Forms.DateTimePicker();
            this.dateEnd = new System.Windows.Forms.DateTimePicker();
            this.btnLoad = new System.Windows.Forms.Button();
            this.txbDebug = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lbData = new System.Windows.Forms.ListBox();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.cmbMovAvg1 = new System.Windows.Forms.ComboBox();
            this.txbMovAvg1 = new System.Windows.Forms.TextBox();
            this.btnMovAvg1 = new System.Windows.Forms.Button();
            this.btnMovAvg2 = new System.Windows.Forms.Button();
            this.txbMovAvg2 = new System.Windows.Forms.TextBox();
            this.cmbMovAvg2 = new System.Windows.Forms.ComboBox();
            this.chart2 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.btnOsc = new System.Windows.Forms.Button();
            this.cmbOsc = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart2)).BeginInit();
            this.SuspendLayout();
            // 
            // txbSymbol
            // 
            this.txbSymbol.Location = new System.Drawing.Point(9, 25);
            this.txbSymbol.Name = "txbSymbol";
            this.txbSymbol.Size = new System.Drawing.Size(94, 20);
            this.txbSymbol.TabIndex = 0;
            this.txbSymbol.Text = "Stock / Index";
            this.txbSymbol.Leave += new System.EventHandler(this.txbSymbol_Leave);
            // 
            // dateStart
            // 
            this.dateStart.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateStart.Location = new System.Drawing.Point(109, 25);
            this.dateStart.Name = "dateStart";
            this.dateStart.Size = new System.Drawing.Size(87, 20);
            this.dateStart.TabIndex = 1;
            // 
            // dateEnd
            // 
            this.dateEnd.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateEnd.Location = new System.Drawing.Point(202, 25);
            this.dateEnd.Name = "dateEnd";
            this.dateEnd.Size = new System.Drawing.Size(87, 20);
            this.dateEnd.TabIndex = 2;
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(295, 25);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(81, 20);
            this.btnLoad.TabIndex = 3;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // txbDebug
            // 
            this.txbDebug.Location = new System.Drawing.Point(9, 51);
            this.txbDebug.Name = "txbDebug";
            this.txbDebug.Size = new System.Drawing.Size(367, 20);
            this.txbDebug.TabIndex = 7;
            this.txbDebug.TabStop = false;
            this.txbDebug.Text = "Status";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Symbol";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(106, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Start Date";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(199, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "End Date";
            // 
            // lbData
            // 
            this.lbData.FormattingEnabled = true;
            this.lbData.Location = new System.Drawing.Point(382, 9);
            this.lbData.Name = "lbData";
            this.lbData.Size = new System.Drawing.Size(406, 56);
            this.lbData.TabIndex = 9;
            // 
            // chart1
            // 
            chartArea1.AxisY.IntervalAutoMode = System.Windows.Forms.DataVisualization.Charting.IntervalAutoMode.VariableCount;
            chartArea1.AxisY.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Number;
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            legend1.Enabled = false;
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(9, 106);
            this.chart1.Name = "chart1";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Candlestick;
            series1.CustomProperties = "PriceDownColor=192\\, 0\\, 0, PriceUpColor=Green";
            series1.IsXValueIndexed = true;
            series1.Legend = "Legend1";
            series1.MarkerBorderColor = System.Drawing.Color.WhiteSmoke;
            series1.MarkerColor = System.Drawing.Color.Transparent;
            series1.Name = "s1";
            series1.YValuesPerPoint = 4;
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series2.CustomProperties = "IsXAxisQuantitative=True";
            series2.IsXValueIndexed = true;
            series2.Legend = "Legend1";
            series2.Name = "serMovAvg1";
            series3.ChartArea = "ChartArea1";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series3.CustomProperties = "IsXAxisQuantitative=True";
            series3.IsXValueIndexed = true;
            series3.Legend = "Legend1";
            series3.Name = "serMovAvg2";
            this.chart1.Series.Add(series1);
            this.chart1.Series.Add(series2);
            this.chart1.Series.Add(series3);
            this.chart1.Size = new System.Drawing.Size(781, 332);
            this.chart1.TabIndex = 10;
            this.chart1.Text = "chart1";
            this.chart1.Resize += new System.EventHandler(this.OnChart1Resize);
            // 
            // cmbMovAvg1
            // 
            this.cmbMovAvg1.FormattingEnabled = true;
            this.cmbMovAvg1.Items.AddRange(new object[] {
            "",
            "SMA",
            "EMA",
            "WMA",
            "DEMA",
            "TEMA",
            "TRIMA",
            "KAMA",
            "T3"});
            this.cmbMovAvg1.Location = new System.Drawing.Point(12, 77);
            this.cmbMovAvg1.Name = "cmbMovAvg1";
            this.cmbMovAvg1.Size = new System.Drawing.Size(58, 21);
            this.cmbMovAvg1.TabIndex = 11;
            // 
            // txbMovAvg1
            // 
            this.txbMovAvg1.Location = new System.Drawing.Point(76, 77);
            this.txbMovAvg1.Name = "txbMovAvg1";
            this.txbMovAvg1.Size = new System.Drawing.Size(42, 20);
            this.txbMovAvg1.TabIndex = 12;
            // 
            // btnMovAvg1
            // 
            this.btnMovAvg1.Location = new System.Drawing.Point(124, 77);
            this.btnMovAvg1.Name = "btnMovAvg1";
            this.btnMovAvg1.Size = new System.Drawing.Size(45, 21);
            this.btnMovAvg1.TabIndex = 13;
            this.btnMovAvg1.Text = "Apply";
            this.btnMovAvg1.UseVisualStyleBackColor = true;
            this.btnMovAvg1.Click += new System.EventHandler(this.OnBnMovAvg1);
            // 
            // btnMovAvg2
            // 
            this.btnMovAvg2.Location = new System.Drawing.Point(314, 79);
            this.btnMovAvg2.Name = "btnMovAvg2";
            this.btnMovAvg2.Size = new System.Drawing.Size(45, 21);
            this.btnMovAvg2.TabIndex = 16;
            this.btnMovAvg2.Text = "Apply";
            this.btnMovAvg2.UseVisualStyleBackColor = true;
            this.btnMovAvg2.Click += new System.EventHandler(this.OnBnMovAvg2);
            // 
            // txbMovAvg2
            // 
            this.txbMovAvg2.Location = new System.Drawing.Point(266, 79);
            this.txbMovAvg2.Name = "txbMovAvg2";
            this.txbMovAvg2.Size = new System.Drawing.Size(42, 20);
            this.txbMovAvg2.TabIndex = 15;
            // 
            // cmbMovAvg2
            // 
            this.cmbMovAvg2.FormattingEnabled = true;
            this.cmbMovAvg2.Items.AddRange(new object[] {
            "",
            "SMA",
            "EMA",
            "WMA",
            "DEMA",
            "TEMA",
            "TRIMA",
            "KAMA",
            "T3"});
            this.cmbMovAvg2.Location = new System.Drawing.Point(202, 79);
            this.cmbMovAvg2.Name = "cmbMovAvg2";
            this.cmbMovAvg2.Size = new System.Drawing.Size(58, 21);
            this.cmbMovAvg2.TabIndex = 14;
            // 
            // chart2
            // 
            chartArea2.AxisX.MajorGrid.LineColor = System.Drawing.Color.Silver;
            chartArea2.AxisY.MajorGrid.LineColor = System.Drawing.Color.Silver;
            chartArea2.Name = "ChartArea1";
            this.chart2.ChartAreas.Add(chartArea2);
            legend2.Enabled = false;
            legend2.Name = "Legend1";
            this.chart2.Legends.Add(legend2);
            this.chart2.Location = new System.Drawing.Point(9, 444);
            this.chart2.Name = "chart2";
            series4.ChartArea = "ChartArea1";
            series4.Color = System.Drawing.Color.DarkGreen;
            series4.CustomProperties = "IsXAxisQuantitative=False";
            series4.IsVisibleInLegend = false;
            series4.IsXValueIndexed = true;
            series4.Legend = "Legend1";
            series4.Name = "s3";
            series5.ChartArea = "ChartArea1";
            series5.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series5.Color = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            series5.CustomProperties = "IsXAxisQuantitative=False";
            series5.IsVisibleInLegend = false;
            series5.IsXValueIndexed = true;
            series5.Legend = "Legend1";
            series5.Name = "s2";
            series6.ChartArea = "ChartArea1";
            series6.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series6.Color = System.Drawing.Color.Blue;
            series6.CustomProperties = "IsXAxisQuantitative=False";
            series6.IsVisibleInLegend = false;
            series6.IsXValueIndexed = true;
            series6.Legend = "Legend1";
            series6.Name = "s1";
            series7.ChartArea = "ChartArea1";
            series7.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series7.Color = System.Drawing.Color.Black;
            series7.IsXValueIndexed = true;
            series7.Legend = "Legend1";
            series7.Name = "lowThresh";
            series8.ChartArea = "ChartArea1";
            series8.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series8.Color = System.Drawing.Color.Black;
            series8.IsXValueIndexed = true;
            series8.Legend = "Legend1";
            series8.Name = "highThresh";
            this.chart2.Series.Add(series4);
            this.chart2.Series.Add(series5);
            this.chart2.Series.Add(series6);
            this.chart2.Series.Add(series7);
            this.chart2.Series.Add(series8);
            this.chart2.Size = new System.Drawing.Size(781, 93);
            this.chart2.SuppressExceptions = true;
            this.chart2.TabIndex = 17;
            this.chart2.Text = "chart2";
            // 
            // btnOsc
            // 
            this.btnOsc.Location = new System.Drawing.Point(493, 79);
            this.btnOsc.Name = "btnOsc";
            this.btnOsc.Size = new System.Drawing.Size(45, 21);
            this.btnOsc.TabIndex = 19;
            this.btnOsc.Text = "Apply";
            this.btnOsc.UseVisualStyleBackColor = true;
            this.btnOsc.Click += new System.EventHandler(this.OnBnOsc);
            // 
            // cmbOsc
            // 
            this.cmbOsc.FormattingEnabled = true;
            this.cmbOsc.Items.AddRange(new object[] {
            "",
            "RSI",
            "STOCH",
            "Fast STOCH",
            "STOCH RSI",
            "MACD",
            "DX",
            "ADX",
            "CCI",
            "ULT OSC"});
            this.cmbOsc.Location = new System.Drawing.Point(400, 79);
            this.cmbOsc.Name = "cmbOsc";
            this.cmbOsc.Size = new System.Drawing.Size(87, 21);
            this.cmbOsc.TabIndex = 18;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 546);
            this.Controls.Add(this.btnOsc);
            this.Controls.Add(this.cmbOsc);
            this.Controls.Add(this.chart2);
            this.Controls.Add(this.btnMovAvg2);
            this.Controls.Add(this.txbMovAvg2);
            this.Controls.Add(this.cmbMovAvg2);
            this.Controls.Add(this.btnMovAvg1);
            this.Controls.Add(this.txbMovAvg1);
            this.Controls.Add(this.cmbMovAvg1);
            this.Controls.Add(this.chart1);
            this.Controls.Add(this.lbData);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txbDebug);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.dateEnd);
            this.Controls.Add(this.dateStart);
            this.Controls.Add(this.txbSymbol);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.OnFormLoad);
            this.Shown += new System.EventHandler(this.OnFormShown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
            this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.OnPreviewKeyDown);
            this.Resize += new System.EventHandler(this.OnResize);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txbSymbol;
        private System.Windows.Forms.DateTimePicker dateStart;
        private System.Windows.Forms.DateTimePicker dateEnd;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.TextBox txbDebug;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListBox lbData;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.ComboBox cmbMovAvg1;
        private System.Windows.Forms.TextBox txbMovAvg1;
        private System.Windows.Forms.Button btnMovAvg1;
        private System.Windows.Forms.Button btnMovAvg2;
        private System.Windows.Forms.TextBox txbMovAvg2;
        private System.Windows.Forms.ComboBox cmbMovAvg2;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart2;
        private System.Windows.Forms.Button btnOsc;
        private System.Windows.Forms.ComboBox cmbOsc;
    }
}

