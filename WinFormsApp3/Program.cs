using System;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace WinFormsApp4  
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }


    public class Form1 : Form
    {
        public Form1()
        {
            string areaName = "MainArea";
            ChartArea area = new ChartArea(areaName);
            SetupArea(area);

            Chart myChart = new Chart();
            myChart.Dock = DockStyle.Fill;
            myChart.Name = "chart1";
            myChart.ChartAreas.Add(area);

            // === Õ‹ﬁ“ŒÕ ===
            Series seriesNewton = new Series("Newton");
            SetupSeries(seriesNewton, areaName, 10, System.Drawing.Color.Green);
            for (double x = 0; x <= 10; x += 0.1)
            {
                double y = newton(x);
                seriesNewton.Points.AddXY(x, y);
            }
            myChart.Series.Add(seriesNewton);

            // === œŒÀ»ÕŒÃ (ÍÓ˝ÙÙËˆËÂÌÚ˚ ËÁ —À¿”) ===
            Series seriesPolinomial = new Series("Polinom");
            SetupSeries(seriesPolinomial, areaName, 7, System.Drawing.Color.Yellow);
            for (double x = 0; x <= 10; x += 0.1)
            {
                double y = polynomial(x);
                seriesPolinomial.Points.AddXY(x, y);
            }
            myChart.Series.Add(seriesPolinomial);

            // === À¿√–¿Õ∆ ===
            Series seriesLagrange = new Series("Lagrange");
            SetupSeries(seriesLagrange, areaName, 2, System.Drawing.Color.Red);
            for (double x = 0; x <= 10; x += 0.1)
            {
                double y = lagrange(x);
                seriesLagrange.Points.AddXY(x, y);
            }
            myChart.Series.Add(seriesLagrange);

            // === »—’ŒƒÕ€≈ “Œ◊ » (Ã¿– ≈–€) ===
            Series seriesPoints = new Series("»ÒıÓ‰Ì˚Â ÚÓ˜ÍË");
            seriesPoints.ChartType = SeriesChartType.Point;
            seriesPoints.ChartArea = areaName;
            seriesPoints.MarkerStyle = MarkerStyle.Circle;
            seriesPoints.MarkerSize = 6;
            seriesPoints.MarkerColor = System.Drawing.Color.Blue;
            seriesPoints.Color = System.Drawing.Color.Blue;

            // ƒÓ·‡‚ÎˇÂÏ 5 ÚÓ˜ÂÍ ËÁ Ú‡·ÎËˆ˚
            seriesPoints.Points.AddXY(2, 6);
            seriesPoints.Points.AddXY(4, 6);
            seriesPoints.Points.AddXY(5, 1);
            seriesPoints.Points.AddXY(6, -1);
            seriesPoints.Points.AddXY(7, 11);

            myChart.Series.Add(seriesPoints);

            this.Controls.Add(myChart);
        }

        static void SetupArea(ChartArea area)
        {
            area.AxisX.Minimum = 0;
            area.AxisX.Maximum = 10;
            area.AxisY.Minimum = -5;
            area.AxisY.Maximum = 15;

            area.AxisX.MajorGrid.Enabled = true;
            area.AxisY.MajorGrid.Enabled = true;
            area.AxisX.MajorGrid.Interval = 1;
            area.AxisY.MajorGrid.Interval = 1;

            area.AxisX.LabelStyle.Interval = 1;
            area.AxisX.LabelStyle.Format = "{0:F0}";
            area.AxisY.LabelStyle.Interval = 5;
            area.AxisY.LabelStyle.Format = "{0:F0}";
        }

        static void SetupSeries(Series series, string areaName, int width, System.Drawing.Color color)
        {
            series.ChartType = SeriesChartType.Line;
            series.ChartArea = areaName;
            series.BorderWidth = width;
            series.Color = color;
        }

        static double polynomial(double x)
        {
            return 0.2083 * Math.Pow(x, 4) - 2.75 * Math.Pow(x, 3) + 11.292 * Math.Pow(x, 2) - 15.75 * x + 11;
        }

        static double lagrange(double x)
        {
            return
                (1f / 20) * (x - 4) * (x - 5) * (x - 6) * (x - 7)
                - (1f / 2) * (x - 2) * (x - 5) * (x - 6) * (x - 7)
                + (1f / 6) * (x - 2) * (x - 4) * (x - 6) * (x - 7)
                + (1f / 8) * (x - 2) * (x - 4) * (x - 5) * (x - 7)
                + (11f / 30) * (x - 2) * (x - 4) * (x - 5) * (x - 6);
        }

        static double newton(double x)
        {
            return
                6 - (5f / 3) * (x - 2) * (x - 4)
                + (19f / 24) * (x - 2) * (x - 4) * (x - 5)
                + (5f / 24) * (x - 2) * (x - 4) * (x - 5) * (x - 6);
        }
    }
}