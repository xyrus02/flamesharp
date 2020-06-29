namespace Net.Ktrix.Flamesharp.Cli
{
	partial class DiagnosticsWindow
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
			System.Windows.Forms.DataVisualization.Charting.StripLine stripLine1 = new System.Windows.Forms.DataVisualization.Charting.StripLine();
			System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
			System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
			System.Windows.Forms.DataVisualization.Charting.StripLine stripLine2 = new System.Windows.Forms.DataVisualization.Charting.StripLine();
			System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
			System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
			System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
			this.histogramPanel = new System.Windows.Forms.Panel();
			this.brightness = new System.Windows.Forms.DataVisualization.Charting.Chart();
			this.peakPoint = new System.Windows.Forms.DataVisualization.Charting.Chart();
			((System.ComponentModel.ISupportInitialize)(this.brightness)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.peakPoint)).BeginInit();
			this.SuspendLayout();
			// 
			// histogramPanel
			// 
			this.histogramPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.histogramPanel.Location = new System.Drawing.Point(12, 12);
			this.histogramPanel.Name = "histogramPanel";
			this.histogramPanel.Size = new System.Drawing.Size(512, 512);
			this.histogramPanel.TabIndex = 2;
			// 
			// brightness
			// 
			this.brightness.BackColor = System.Drawing.Color.Black;
			chartArea1.AxisX.IsLabelAutoFit = false;
			chartArea1.AxisX.LabelStyle.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			chartArea1.AxisX.LabelStyle.ForeColor = System.Drawing.Color.White;
			chartArea1.AxisX.LabelStyle.Interval = 0D;
			chartArea1.AxisX.LabelStyle.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Number;
			chartArea1.AxisX.LineColor = System.Drawing.Color.Gray;
			stripLine1.BorderColor = System.Drawing.Color.White;
			stripLine1.ForeColor = System.Drawing.Color.White;
			chartArea1.AxisX.StripLines.Add(stripLine1);
			chartArea1.AxisX.Title = "Samples";
			chartArea1.AxisX.TitleFont = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			chartArea1.AxisX.TitleForeColor = System.Drawing.Color.White;
			chartArea1.AxisY.IsLabelAutoFit = false;
			chartArea1.AxisY.LabelStyle.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			chartArea1.AxisY.LabelStyle.ForeColor = System.Drawing.Color.White;
			chartArea1.AxisY.LabelStyle.Interval = 0D;
			chartArea1.AxisY.LabelStyle.IntervalOffset = 0D;
			chartArea1.AxisY.LabelStyle.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Number;
			chartArea1.AxisY.LineColor = System.Drawing.Color.DimGray;
			chartArea1.AxisY.TextOrientation = System.Windows.Forms.DataVisualization.Charting.TextOrientation.Rotated270;
			chartArea1.AxisY.Title = "Brightness";
			chartArea1.AxisY.TitleFont = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			chartArea1.AxisY.TitleForeColor = System.Drawing.Color.White;
			chartArea1.BackColor = System.Drawing.Color.Black;
			chartArea1.BorderColor = System.Drawing.Color.Gray;
			chartArea1.BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
			chartArea1.Name = "bcArea";
			this.brightness.ChartAreas.Add(chartArea1);
			this.brightness.Location = new System.Drawing.Point(540, 12);
			this.brightness.Name = "brightness";
			this.brightness.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Bright;
			series1.ChartArea = "bcArea";
			series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
			series1.LabelForeColor = System.Drawing.Color.White;
			series1.Name = "bcSeries";
			series1.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Int32;
			series1.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
			this.brightness.Series.Add(series1);
			this.brightness.Size = new System.Drawing.Size(368, 241);
			this.brightness.TabIndex = 4;
			// 
			// peakPoint
			// 
			this.peakPoint.BackColor = System.Drawing.Color.Black;
			chartArea2.AxisX.IsLabelAutoFit = false;
			chartArea2.AxisX.LabelStyle.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			chartArea2.AxisX.LabelStyle.ForeColor = System.Drawing.Color.White;
			chartArea2.AxisX.LabelStyle.Interval = 0D;
			chartArea2.AxisX.LabelStyle.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Number;
			chartArea2.AxisX.LineColor = System.Drawing.Color.Gray;
			stripLine2.BorderColor = System.Drawing.Color.White;
			stripLine2.ForeColor = System.Drawing.Color.White;
			chartArea2.AxisX.StripLines.Add(stripLine2);
			chartArea2.AxisX.Title = "Samples";
			chartArea2.AxisX.TitleFont = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			chartArea2.AxisX.TitleForeColor = System.Drawing.Color.White;
			chartArea2.AxisY.IsLabelAutoFit = false;
			chartArea2.AxisY.LabelStyle.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			chartArea2.AxisY.LabelStyle.ForeColor = System.Drawing.Color.White;
			chartArea2.AxisY.LabelStyle.Interval = 0D;
			chartArea2.AxisY.LabelStyle.IntervalOffset = 0D;
			chartArea2.AxisY.LabelStyle.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Number;
			chartArea2.AxisY.LineColor = System.Drawing.Color.DimGray;
			chartArea2.AxisY.TextOrientation = System.Windows.Forms.DataVisualization.Charting.TextOrientation.Rotated270;
			chartArea2.AxisY.Title = "Accumulation";
			chartArea2.AxisY.TitleFont = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			chartArea2.AxisY.TitleForeColor = System.Drawing.Color.White;
			chartArea2.BackColor = System.Drawing.Color.Black;
			chartArea2.BorderColor = System.Drawing.Color.Gray;
			chartArea2.BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
			chartArea2.Name = "bcArea";
			this.peakPoint.ChartAreas.Add(chartArea2);
			legend1.Alignment = System.Drawing.StringAlignment.Far;
			legend1.BackColor = System.Drawing.Color.Black;
			legend1.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Bottom;
			legend1.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			legend1.ForeColor = System.Drawing.Color.White;
			legend1.IsTextAutoFit = false;
			legend1.LegendStyle = System.Windows.Forms.DataVisualization.Charting.LegendStyle.Column;
			legend1.Name = "Legend1";
			legend1.TableStyle = System.Windows.Forms.DataVisualization.Charting.LegendTableStyle.Tall;
			this.peakPoint.Legends.Add(legend1);
			this.peakPoint.Location = new System.Drawing.Point(540, 259);
			this.peakPoint.Name = "peakPoint";
			this.peakPoint.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Excel;
			series2.ChartArea = "bcArea";
			series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
			series2.LabelForeColor = System.Drawing.Color.White;
			series2.Legend = "Legend1";
			series2.Name = "High acc";
			series2.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Int32;
			series2.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
			series3.ChartArea = "bcArea";
			series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
			series3.Legend = "Legend1";
			series3.Name = "Low acc";
			this.peakPoint.Series.Add(series2);
			this.peakPoint.Series.Add(series3);
			this.peakPoint.Size = new System.Drawing.Size(368, 264);
			this.peakPoint.TabIndex = 5;
			// 
			// DiagnosticsWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Black;
			this.ClientSize = new System.Drawing.Size(920, 535);
			this.Controls.Add(this.peakPoint);
			this.Controls.Add(this.brightness);
			this.Controls.Add(this.histogramPanel);
			this.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "DiagnosticsWindow";
			this.ShowIcon = false;
			this.Text = "Control panel";
			((System.ComponentModel.ISupportInitialize)(this.brightness)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.peakPoint)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion
		internal System.Windows.Forms.Panel histogramPanel;
		internal System.Windows.Forms.DataVisualization.Charting.Chart brightness;
		internal System.Windows.Forms.DataVisualization.Charting.Chart peakPoint;
	}
}