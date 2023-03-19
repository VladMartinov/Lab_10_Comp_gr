using System;
using System.Drawing;
using System.Windows.Forms;

namespace Lab_10_Comp.gr._
{
    public partial class Form1 : Form
    {
        /* Задаем область вывода */
        readonly float  xmin = 2.5f, xmax = 6.5f,
                        x_centr = (2.5f + 6.5f) / 2,
                        ymin = 1.5f, ymax = 5.5f,
                        y_centr = (1.5f + 5.5f) / 2;

        readonly Graphics gr; readonly Pen penLine;

        /* Инициализация формы */
        public Form1()
        {
            InitializeComponent();

            gr = pictureBox.CreateGraphics();
            penLine = new Pen(Brushes.BlueViolet, 2);
        }

        /* Метод преобразования вещественной координаты X в целую */
        private int IX(double x)
        {
            double xx = x * (pictureBox.Size.Width / 10.0) + 0.5;
            return (int)xx;
        }
        
        /* Метод преобразования вещественной координаты Y в целую */
        private int IY(double y)
        {
            double yy = pictureBox.Size.Height - y * (pictureBox.Size.Height / 7.0) + 0.5;
            return (int)yy;
        }
        
        /* Своя функция вычечивания линии (экран 10х7 условных единиц) */
        private void Draw(double x1, double y1, double x2, double y2)
        {
            Point point1 = new Point(IX(x1), IY(y1));
            Point point2 = new Point(IX(x2), IY(y2));
            gr.DrawLine(penLine, point1, point2);
        }
        
        /* Метод получение кода положения точки относительно окна по алгоритму Коєна-Сазерленда */
        private uint Code(double x, double y)
        {
            return (uint)((Convert.ToUInt16(x < xmin) << 3) |
            (Convert.ToUInt16(x > xmax) << 2) |
            (Convert.ToUInt16(y < ymin) << 1) |
            Convert.ToUInt16(y > ymax));
        }

        private PointF RotateCenter(double x, double y, int angle)
        {
            double Phi = (angle * Math.PI) / 180;

            PointF point1 = new PointF((float)(x_centr + (x - x_centr) * Math.Cos(Phi) - (y - y_centr) * Math.Sin(Phi)),
                                        (float)(y_centr + (x - x_centr) * Math.Sin(Phi) + (y - y_centr) * Math.Cos(Phi)));
        
            return point1;
        }
        
        /* Метод отсечения линий */
        private void Clip(double x1, double y1, double x2, double y2)
        {
            uint c1;
            uint c2;
            double dx, dy;
            c1 = Code(x1, y1);
            c2 = Code(x2, y2);
        
            while ((c1 | c2) != 0)
            {
                if ((c1 & c2) != 0) return;
                dx = x2 - x1;
                dy = y2 - y1;
                if (c1 != 0)
                {
                    if (x1 < xmin) { y1 += dy * (xmin - x1) / dx; x1 = xmin; }
                    else
                    if (x1 > xmax) { y1 += dy * (xmax - x1) / dx; x1 = xmax; }
                    else
                    if (y1 < ymin) { x1 += dx * (ymin - y1) / dy; y1 = ymin; }
                    else
                    if (y1 > ymax) { x1 += dx * (ymax - y1) / dy; y1 = ymax; }
                    c1 = Code(x1, y1);
                }
                else
                {
                    if (x2 < xmin) { y2 += dy * (xmin - x2) / dx; x2 = xmin; }
                    else
                    if (x2 > xmax) { y2 += dy * (xmax - x2) / dx; x2 = xmax; }
                    else
                    if (y2 < ymin) { x2 += dx * (ymin - y2) / dy; y2 = ymin; }
                    else
                    if (y2 > ymax) { x2 += dx * (ymax - y2) / dy; y2 = ymax; }
                    c2 = Code(x2, y2);
                }
            }

            PointF point1 = RotateCenter(x1, y1, 45);

            PointF point2 = RotateCenter(x2, y2, 45);

            Draw(point1.X, point1.Y, point2.X, point2.Y); // Соединяем точки линией
        }
        
        /* Основной код программы */
        private void Button_Click(object sender, EventArgs e)
        {
            int i; 
            double  r, pi, alpha, phi0, phi, 
                    x0, y0, 
                    x1, y1, 
                    x2, y2;

            pi = 4.0 * Math.Atan(1.0);
            alpha = 90.0 * pi / 180.0; 
            phi0 = 0.0; x0 = 5.0; y0 = 3.5;

            /* Вычерчивание границ окна */

            double Phi = (45 * Math.PI) / 180;

            PointF point1 = RotateCenter(xmin, ymin, 45);
            PointF point2 = RotateCenter(xmax, ymin, 45);
            PointF point4 = RotateCenter(xmax, ymax, 45);
            PointF point6 = RotateCenter(xmin, ymax, 45);

            Draw(point1.X, point1.Y, point2.X, point2.Y); Draw(point2.X, point2.Y, point4.X, point4.Y);
            Draw(point4.X, point4.Y, point6.X, point6.Y); Draw(point6.X, point6.Y, point1.X, point1.Y);

            /* В пределах границ окна вычерчиваются 20 правильных концентрических пятиугольников */
            for (r = 0.5; r < 10.5; r += 0.5)
            {
                x2 = x0 + r * Math.Cos(phi0); y2 = y0 + r * Math.Sin(phi0);
                for (i = 1; i <= 5; i++)
                {
                    phi = phi0 + i * alpha;

                    x1 = x2; y1 = y2;
                    x2 = x0 + r * Math.Cos(phi); y2 = y0 + r * Math.Sin(phi);
                    
                    /* Отсекаем по окну */
                    Clip(x1, y1, x2, y2);
                }
            }

            /* Подпись к лабораторной работе */
            string str = "Лабораторная работа №10.";
            
            Brush coralBrush = Brushes.Coral;
            Font boldTimesFont = new Font("Times New Roman", 13, FontStyle.Bold);
            SizeF sizefText = gr.MeasureString(str, boldTimesFont);

            gr.DrawString(  str, boldTimesFont, coralBrush,
                            (pictureBox.Size.Width - sizefText.Width) / 2, 0);
        }
    }
}