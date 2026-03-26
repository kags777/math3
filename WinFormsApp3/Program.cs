using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;


namespace InterpolationApp
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }


    public partial class Form1 : Form
    {
        private TextBox txtFunction;
        private TextBox txtA;
        private TextBox txtB;
        private TextBox txtPointsCount;
        private Button btnBuild;
        private Chart chartMain;
        private ComboBox cmbFunction;
        private Label lblTimeLagrange;
        private Label lblTimeNewton;
        private Label lblTimeSLAU;
        private CheckBox chkShowOriginal;
        private CheckBox chkShowLagrange;
        private CheckBox chkShowNewton;
        private CheckBox chkShowSLAU;
        private NumericUpDown nudPointsCount;

        public Form1()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Интерполяция функций";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Создание панели управления
            Panel controlPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 150,
                BackColor = Color.LightGray
            };

            // Выбор функции
            Label lblFunction = new Label
            {
                Text = "Функция:",
                Location = new Point(10, 15),
                AutoSize = true
            };
            controlPanel.Controls.Add(lblFunction);

            cmbFunction = new ComboBox
            {
                Location = new Point(80, 12),
                Width = 300,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbFunction.Items.AddRange(new string[] {
                "f(x) = 1 / (1 + 25*x^2) [Функция Рунге]",
                "f(x) = sin(x)",
                "f(x) = x^2",
                "f(x) = exp(x)",
                "f(x) = |x|"
            });
            cmbFunction.SelectedIndex = 0;
            controlPanel.Controls.Add(cmbFunction);

            // Интервал
            Label lblA = new Label
            {
                Text = "a:",
                Location = new Point(400, 15),
                AutoSize = true
            };
            controlPanel.Controls.Add(lblA);

            txtA = new TextBox
            {
                Location = new Point(420, 12),
                Width = 60,
                Text = "-1"
            };
            controlPanel.Controls.Add(txtA);

            Label lblB = new Label
            {
                Text = "b:",
                Location = new Point(490, 15),
                AutoSize = true
            };
            controlPanel.Controls.Add(lblB);

            txtB = new TextBox
            {
                Location = new Point(510, 12),
                Width = 60,
                Text = "1"
            };
            controlPanel.Controls.Add(txtB);

            // Количество точек
            Label lblPoints = new Label
            {
                Text = "Количество точек:",
                Location = new Point(600, 15),
                AutoSize = true
            };
            controlPanel.Controls.Add(lblPoints);

            nudPointsCount = new NumericUpDown
            {
                Location = new Point(720, 12),
                Width = 60,
                Minimum = 3,
                Maximum = 50,
                Value = 10
            };
            controlPanel.Controls.Add(nudPointsCount);

            // Кнопка построения
            btnBuild = new Button
            {
                Text = "Построить графики",
                Location = new Point(820, 10),
                Width = 150,
                Height = 30
            };
            btnBuild.Click += BtnBuild_Click;
            controlPanel.Controls.Add(btnBuild);

            // Чекбоксы для отображения
            chkShowOriginal = new CheckBox
            {
                Text = "Исходная функция",
                Location = new Point(10, 50),
                Checked = true
            };
            controlPanel.Controls.Add(chkShowOriginal);

            chkShowLagrange = new CheckBox
            {
                Text = "Лагранж",
                Location = new Point(150, 50),
                Checked = true
            };
            controlPanel.Controls.Add(chkShowLagrange);

            chkShowNewton = new CheckBox
            {
                Text = "Ньютон",
                Location = new Point(250, 50),
                Checked = true
            };
            controlPanel.Controls.Add(chkShowNewton);

            chkShowSLAU = new CheckBox
            {
                Text = "СЛАУ",
                Location = new Point(350, 50),
                Checked = true
            };
            controlPanel.Controls.Add(chkShowSLAU);

            // Метки времени
            lblTimeLagrange = new Label
            {
                Text = "Время Лагранжа: -",
                Location = new Point(10, 85),
                AutoSize = true
            };
            controlPanel.Controls.Add(lblTimeLagrange);

            lblTimeNewton = new Label
            {
                Text = "Время Ньютона: -",
                Location = new Point(250, 85),
                AutoSize = true
            };
            controlPanel.Controls.Add(lblTimeNewton);

            lblTimeSLAU = new Label
            {
                Text = "Время СЛАУ: -",
                Location = new Point(500, 85),
                AutoSize = true
            };
            controlPanel.Controls.Add(lblTimeSLAU);

            // График
            chartMain = new Chart
            {
                Dock = DockStyle.Fill
            };
            ChartArea chartArea = new ChartArea();
            chartMain.ChartAreas.Add(chartArea);

            // Настройка осей
            chartArea.AxisX.Title = "x";
            chartArea.AxisY.Title = "f(x)";
            chartArea.AxisX.MajorGrid.Enabled = true;
            chartArea.AxisY.MajorGrid.Enabled = true;

            this.Controls.Add(chartMain);
            this.Controls.Add(controlPanel);
        }

        private void BtnBuild_Click(object sender, EventArgs e)
        {
            try
            {
                double a = double.Parse(txtA.Text);
                double b = double.Parse(txtB.Text);
                int n = (int)nudPointsCount.Value;

                Func<double, double> func = GetSelectedFunction();

                // Создание узлов интерполяции
                double[] xNodes = new double[n];
                double[] yNodes = new double[n];
                for (int i = 0; i < n; i++)
                {
                    xNodes[i] = a + i * (b - a) / (n - 1);
                    yNodes[i] = func(xNodes[i]);
                }

                // Точки для построения графиков
                int plotPoints = 500;
                double[] xPlot = new double[plotPoints];
                double[] yOriginal = new double[plotPoints];
                for (int i = 0; i < plotPoints; i++)
                {
                    xPlot[i] = a + i * (b - a) / (plotPoints - 1);
                    yOriginal[i] = func(xPlot[i]);
                }

                // Очистка графика
                chartMain.Series.Clear();

                // Построение исходной функции
                if (chkShowOriginal.Checked)
                {
                    Series originalSeries = new Series("Исходная функция")
                    {
                        ChartType = SeriesChartType.Line,
                        Color = Color.Black,
                        BorderWidth = 2
                    };
                    for (int i = 0; i < plotPoints; i++)
                    {
                        originalSeries.Points.AddXY(xPlot[i], yOriginal[i]);
                    }
                    chartMain.Series.Add(originalSeries);
                }

                // Добавление узлов интерполяции
                Series nodesSeries = new Series("Узлы интерполяции")
                {
                    ChartType = SeriesChartType.Point,
                    Color = Color.Red,
                    MarkerSize = 8,
                    MarkerStyle = MarkerStyle.Circle
                };
                for (int i = 0; i < n; i++)
                {
                    nodesSeries.Points.AddXY(xNodes[i], yNodes[i]);
                }
                chartMain.Series.Add(nodesSeries);

                // Метод Лагранжа
                if (chkShowLagrange.Checked)
                {
                    var watch = System.Diagnostics.Stopwatch.StartNew();
                    double[] yLagrange = LagrangeInterpolation(xNodes, yNodes, xPlot);
                    watch.Stop();
                    lblTimeLagrange.Text = $"Время Лагранжа: {watch.ElapsedTicks} ticks";

                    Series lagrangeSeries = new Series("Лагранж")
                    {
                        ChartType = SeriesChartType.Line,
                        Color = Color.Blue,
                        BorderWidth = 10
                    };
                    for (int i = 0; i < plotPoints; i++)
                    {
                        lagrangeSeries.Points.AddXY(xPlot[i], yLagrange[i]);
                    }
                    chartMain.Series.Add(lagrangeSeries);
                }

                // Метод Ньютона
                if (chkShowNewton.Checked)
                {
                    var watch = System.Diagnostics.Stopwatch.StartNew();
                    double[] yNewton = NewtonInterpolation(xNodes, yNodes, xPlot);
                    watch.Stop();
                    lblTimeNewton.Text = $"Время Ньютона: {watch.ElapsedTicks} ticks";

                    Series newtonSeries = new Series("Ньютон")
                    {
                        ChartType = SeriesChartType.Line,
                        Color = Color.Green,
                        BorderWidth = 8
                    };
                    for (int i = 0; i < plotPoints; i++)
                    {
                        newtonSeries.Points.AddXY(xPlot[i], yNewton[i]);
                    }
                    chartMain.Series.Add(newtonSeries);
                }

                // Метод СЛАУ
                if (chkShowSLAU.Checked)
                {
                    var watch = System.Diagnostics.Stopwatch.StartNew();
                    double[] coeffs = SolveSLAU(xNodes, yNodes);
                    double[] ySLAU = EvaluatePolynomial(coeffs, xPlot);
                    watch.Stop();
                    lblTimeSLAU.Text = $"Время СЛАУ: {watch.ElapsedTicks} ticks";

                    Series slauSeries = new Series("СЛАУ")
                    {
                        ChartType = SeriesChartType.Line,
                        Color = Color.Orange,
                        BorderWidth = 4
                    };
                    for (int i = 0; i < plotPoints; i++)
                    {
                        slauSeries.Points.AddXY(xPlot[i], ySLAU[i]);
                    }
                    chartMain.Series.Add(slauSeries);
                }

                // Легенда
                chartMain.Legends.Clear();
                Legend legend = new Legend();
                chartMain.Legends.Add(legend);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Func<double, double> GetSelectedFunction()
        {
            string selected = cmbFunction.SelectedItem.ToString();

            if (selected.Contains("1 / (1 + 25*x^2)"))
            {
                return x => 1.0 / (1.0 + 25.0 * x * x);
            }
            else if (selected.Contains("sin(x)"))
            {
                return x => Math.Sin(x);
            }
            else if (selected.Contains("x^2"))
            {
                return x => x * x;
            }
            else if (selected.Contains("exp(x)"))
            {
                return x => Math.Exp(x);
            }
            else if (selected.Contains("|x|"))
            {
                return x => Math.Abs(x);
            }

            return x => 1.0 / (1.0 + 25.0 * x * x);
        }

        private double[] LagrangeInterpolation(double[] xNodes, double[] yNodes, double[] xEval)
        {
            int n = xNodes.Length;
            double[] result = new double[xEval.Length];

            for (int i = 0; i < xEval.Length; i++)
            {
                double sum = 0;
                for (int j = 0; j < n; j++)
                {
                    double term = yNodes[j];
                    for (int k = 0; k < n; k++)
                    {
                        if (k != j)
                        {
                            term *= (xEval[i] - xNodes[k]) / (xNodes[j] - xNodes[k]);
                        }
                    }
                    sum += term;
                }
                result[i] = sum;
            }

            return result;
        }

        private double[] NewtonInterpolation(double[] xNodes, double[] yNodes, double[] xEval)
        {
            int n = xNodes.Length;
            double[] result = new double[xEval.Length];

            // Вычисление разделенных разностей
            double[,] dividedDiff = new double[n, n];
            for (int i = 0; i < n; i++)
            {
                dividedDiff[i, 0] = yNodes[i];
            }

            for (int j = 1; j < n; j++)
            {
                for (int i = 0; i < n - j; i++)
                {
                    dividedDiff[i, j] = (dividedDiff[i + 1, j - 1] - dividedDiff[i, j - 1]) /
                                        (xNodes[i + j] - xNodes[i]);
                }
            }

            // Вычисление значений интерполяционного многочлена
            for (int i = 0; i < xEval.Length; i++)
            {
                double value = dividedDiff[0, 0];
                double product = 1;

                for (int j = 1; j < n; j++)
                {
                    product *= (xEval[i] - xNodes[j - 1]);
                    value += dividedDiff[0, j] * product;
                }

                result[i] = value;
            }

            return result;
        }

        private double[] SolveSLAU(double[] xNodes, double[] yNodes)
        {
            int n = xNodes.Length;
            double[,] A = new double[n, n];
            double[] B = new double[n];

            // Построение матрицы Вандермонда
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    A[i, j] = Math.Pow(xNodes[i], j);
                }
                B[i] = yNodes[i];
            }

            // Решение СЛАУ методом Гаусса
            return GaussianElimination(A, B);
        }

        private double[] GaussianElimination(double[,] A, double[] B)
        {
            int n = B.Length;
            double[,] augmented = new double[n, n + 1];

            // Создание расширенной матрицы
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    augmented[i, j] = A[i, j];
                }
                augmented[i, n] = B[i];
            }

            // Прямой ход
            for (int k = 0; k < n; k++)
            {
                // Поиск главного элемента
                int maxRow = k;
                double maxVal = Math.Abs(augmented[k, k]);
                for (int i = k + 1; i < n; i++)
                {
                    if (Math.Abs(augmented[i, k]) > maxVal)
                    {
                        maxVal = Math.Abs(augmented[i, k]);
                        maxRow = i;
                    }
                }

                // Перестановка строк
                if (maxRow != k)
                {
                    for (int j = 0; j <= n; j++)
                    {
                        double temp = augmented[k, j];
                        augmented[k, j] = augmented[maxRow, j];
                        augmented[maxRow, j] = temp;
                    }
                }

                // Нормализация
                for (int i = k + 1; i < n; i++)
                {
                    double factor = augmented[i, k] / augmented[k, k];
                    for (int j = k; j <= n; j++)
                    {
                        augmented[i, j] -= factor * augmented[k, j];
                    }
                }
            }

            // Обратный ход
            double[] x = new double[n];
            for (int i = n - 1; i >= 0; i--)
            {
                x[i] = augmented[i, n];
                for (int j = i + 1; j < n; j++)
                {
                    x[i] -= augmented[i, j] * x[j];
                }
                x[i] /= augmented[i, i];
            }

            return x;
        }

        private double[] EvaluatePolynomial(double[] coeffs, double[] xEval)
        {
            double[] result = new double[xEval.Length];
            int n = coeffs.Length;

            for (int i = 0; i < xEval.Length; i++)
            {
                double value = 0;
                for (int j = 0; j < n; j++)
                {
                    value += coeffs[j] * Math.Pow(xEval[i], j);
                }
                result[i] = value;
            }

            return result;
        }
    }
}