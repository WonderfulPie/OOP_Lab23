using System;
using System.Drawing;
using System.Windows.Forms;

namespace Lab23
{
    public partial class Form1 : Form
    {
        private double a = 1; // Новий параметр a
        private double tStart = -5;
        private double tEnd = 5;

        private new const int Margin = 40;
        private const int TickSpacing = 50; // Відстань між позначками на осях у пікселях

        private Label labelDomain;
        private PictureBox pictureBoxGraph;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Ініціалізація компонентів
            this.Text = "Побудова графіка параметричної функції";
            this.Size = new Size(1000, 600);

            // Написи (мітки)
            Label labelA = new Label { Text = "a:", Location = new Point(10, 10), Width = 100 };
            Label labelTStart = new Label { Text = "t (від):", Location = new Point(10, 40), Width = 100 };
            Label labelTEnd = new Label { Text = "t (до):", Location = new Point(10, 70), Width = 100 };

            // Поля для тексту
            TextBox textBoxA = new TextBox { Location = new Point(120, 10), Width = 100, Text = a.ToString() };
            TextBox textBoxTStart = new TextBox { Location = new Point(120, 40), Width = 100, Text = tStart.ToString() };
            TextBox textBoxTEnd = new TextBox { Location = new Point(120, 70), Width = 100, Text = tEnd.ToString() };

            // Кнопка
            Button buttonDraw = new Button { Text = "Побудувати графік", Location = new Point(10, 100), Width = 210 };
            buttonDraw.Click += (s, ev) =>
            {
                if (double.TryParse(textBoxA.Text, out a) &&
                    double.TryParse(textBoxTStart.Text, out tStart) &&
                    double.TryParse(textBoxTEnd.Text, out tEnd))
                {
                    pictureBoxGraph.Invalidate();
                }
                else
                {
                    MessageBox.Show("Введіть коректні значення параметрів.");
                }
            };

            // PictureBox
            pictureBoxGraph = new PictureBox { Location = new Point(250, 10), Size = new Size(700, 550), BorderStyle = BorderStyle.FixedSingle };
            pictureBoxGraph.Paint += PictureBoxGraph_Paint;

            // Мітка для області визначення функцій
            labelDomain = new Label { Text = "", Location = new Point(10, 140), Width = 210 };

            // Додавання компонентів на форму
            this.Controls.Add(labelA);
            this.Controls.Add(labelTStart);
            this.Controls.Add(labelTEnd);
            this.Controls.Add(textBoxA);
            this.Controls.Add(textBoxTStart);
            this.Controls.Add(textBoxTEnd);
            this.Controls.Add(buttonDraw);
            this.Controls.Add(pictureBoxGraph);
        }

        private void PictureBoxGraph_Paint(object sender, PaintEventArgs e)
        {
            var graphics = e.Graphics;
            graphics.Clear(Color.White);

            // Виведення системи координат
            DrawAxes(graphics);

            // Параметричні рівняння кривих
            double FunctionX(double t)
            {
                return a * (t - Math.Sin(t));
            }
            double FunctionY(double t)
            {
                return a * (t - Math.Cos(t));
            }

            // Масштабування графіка
            var scaleX = TickSpacing / 2.0;  // Масштабування по x
            var scaleY = TickSpacing / 2.0; // Масштабування по y

            // Зміщення центру координат
            var centerX = (pictureBoxGraph.Width / 2.0) * TickSpacing / TickSpacing;
            var centerY = (pictureBoxGraph.Height / 2.0) * TickSpacing / TickSpacing;

            var pen = new Pen(Color.Red, 2);
            var path = new System.Drawing.Drawing2D.GraphicsPath();

            // Генерація кривої
            bool firstPoint = true;

            for (double t = tStart; t <= tEnd; t += 0.01) // Збільшуємо кількість точок для плавності кривої
            {
                var x = FunctionX(t);
                var y = FunctionY(t);

                if (double.IsNaN(x) || double.IsNaN(y) || double.IsInfinity(x) || double.IsInfinity(y))
                {
                    continue;
                }

                x = x * scaleX + centerX;
                y = -y * scaleY + centerY;

                if (x < Margin || x > pictureBoxGraph.Width - Margin || y < Margin || y > pictureBoxGraph.Height - Margin)
                {
                    continue;
                }

                if (firstPoint)
                {
                    path.StartFigure();
                    path.AddLine((float)x, (float)y, (float)x, (float)y);
                    firstPoint = false;
                }
                else
                {
                    path.AddLine(path.GetLastPoint(), new PointF((float)x, (float)y));
                }
            }

            if (path.PointCount > 0)
            {
                graphics.DrawPath(pen, path);
            }
            else
            {
                MessageBox.Show("Не вдалося побудувати графік. Перевірте значення параметрів.");
            }
        }

        private void DrawAxes(Graphics graphics)
        {
            var centerX = pictureBoxGraph.Width / 2.0;
            var centerY = pictureBoxGraph.Height / 2.0;

            var penAxis = new Pen(Color.Black, 2);
            var font = new Font("Times New Roman", 8);
            var brush = Brushes.Black;

            // Вертикальна вісь
            graphics.DrawLine(penAxis, (float)centerX, Margin, (float)centerX, pictureBoxGraph.Height - Margin);
            // Горизонтальна вісь
            graphics.DrawLine(penAxis, Margin, (float)centerY, pictureBoxGraph.Width - Margin, (float)centerY);

            // Поділки на осях
            for (int i = Margin; i <= pictureBoxGraph.Width - Margin; i += TickSpacing)
            {
                if ((int)centerX != i)
                {
                    int xVal = (int)((i - centerX) / (TickSpacing / 2));
                    graphics.DrawString(xVal.ToString(), font, brush, i - 10, (float)centerY + 5);
                }
            }

            for (int i = Margin; i <= pictureBoxGraph.Height - Margin; i += TickSpacing)
            {
                if ((int)centerY != i)
                {
                    int yVal = (int)((centerY - i) / (TickSpacing / 2));
                    graphics.DrawString(yVal.ToString(), font, brush, (float)centerX + 5, i - 10);
                }
            }

            // Назви осей
            var axisFont = new Font("Times New Roman", 10);
            graphics.DrawString("X", axisFont, brush, pictureBoxGraph.Width - Margin + 10, (float)centerY - 20);
            graphics.DrawString("Y", axisFont, brush, (float)centerX + 10, Margin - 20);
        }
    }
}