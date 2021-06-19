using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Flyproj
{
    public partial class Flight : Window
    { // Таймер
        public DispatcherTimer timer = new DispatcherTimer();
        public double t;

        // Определение скорости, градусов, массы, координат, высоты, ширины и коэффициента масштабирования для канваса
        public double v, d, m, x, y, w, h, coeffw, coeffh;

        private double vx, vy;

        private double maxH, maxW;
        public double Resx(double t)
        { // Вычисление проекции силы сопротивления на Ох
            return t * t * Math.Cos(t);
        }

        private void speed_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        public double Resy(double t)
        { // Вычисление проекции силы сопротивления на Оу
            return t * Math.Sin(t);
        }

        public Flight()
        {
            InitializeComponent();
            timer.Tick += new EventHandler(OnTimer);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 10);
        }

        public void Canvass()
        {
            w = canvas.ActualWidth / 2;
            h = canvas.ActualHeight / 2;

            maxH = v * v * Math.Sin(Angle(d)) * Math.Sin(Angle(d)) / (2 * 9.81);
            maxW = v * v * Math.Sin(Angle(2 * d)) / 9.81;

            coeffh = h / maxH;
            coeffw = w / maxW;
        }

        public void Start()
        {
            x = y = t = 0;
            poliline.Points.Clear();
            Canvass();
            vx = v * Math.Cos(Angle(d));
            vy = v * Math.Sin(Angle(d));
            timer.Start();
        }

        public void Output()
        {
            v = Convert.ToDouble(speed.Text);
            d = Convert.ToDouble(angle.Text);
            m = Convert.ToDouble(mass.Text);
        }

        public double Angle(double d)
        {
            return Math.PI * d / 180;
        }

        public void Button_Click(object sender, RoutedEventArgs e)
        {
            Output();
            Start();
        }

        public void OnTimer(object sender, EventArgs e)
        {
            t += 0.01;
            //double vx = v * Math.Cos(Angle(d));
            //double vy = v * Math.Sin(Angle(d));
            /*
            x = x + t * vx;
            y = y + t * vy;

            vx = vx - t * (Resx(t) * vx / m);
            vy = vy - t * (9.8 + Resy(t) * vy / m);
            */
            x = x + t * vx * (1 - (t * Resx(t) / m));
            y = y + t * (vy - t * (9.8 - vy * Resy(t) / m));
            if (y < 0) y = 0;

            vx = vx * (1 - t * Resx(t) / m);
            vy = vy - t * (9.8 - Resy(t) * vy / m);

            poliline.Points.Add(new Point(coeffw * x, h - coeffh * y));
            Canvas.SetLeft(ellipse, poliline.Points.Last().X - ellipse.Width / 2.0);
            Canvas.SetTop(ellipse, poliline.Points.Last().Y - ellipse.Height / 2.0);

            if (y <= 0) timer.Stop();
        }
    }
}

