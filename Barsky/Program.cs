using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MyWindowsFormsApp
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }

    public class MainForm : Form
    {
        private Rectangle viewport;
        private List<Tuple<Point, Point>> lines;

        public MainForm()
        {
            this.Text = "Barsky Line Clipping Algorithm";
            this.Size = new Size(800, 600);
            this.Paint += new PaintEventHandler(this.MainForm_Paint);

            viewport = new Rectangle(100, 100, 400, 300);
            lines = new List<Tuple<Point, Point>>
            {
                new Tuple<Point, Point>(new Point(50, 50), new Point(450, 450)),
                new Tuple<Point, Point>(new Point(150, 150), new Point(350, 350)),
                new Tuple<Point, Point>(new Point(300, 50), new Point(300, 450)),
                new Tuple<Point, Point>(new Point(50, 300), new Point(450, 300)),
                new Tuple<Point, Point>(new Point(0, 0), new Point(600, 600))
            };
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen viewportPen = new Pen(Color.Blue, 2);
            Pen linePen = new Pen(Color.Black, 1);
            Pen clippedLinePen = new Pen(Color.Red, 2);

            // 绘制视口
            g.DrawRectangle(viewportPen, viewport);

            // 绘制并裁剪线段
            foreach (var line in lines)
            {
                Point p1 = line.Item1;
                Point p2 = line.Item2;

                // 绘制原始线段
                g.DrawLine(linePen, p1, p2);

                // 裁剪线段
                if (BarskyLineClip(ref p1, ref p2, viewport))
                {
                    g.DrawLine(clippedLinePen, p1, p2);
                }
            }
        }

        private bool BarskyLineClip(ref Point p1, ref Point p2, Rectangle rect)
        {
            double t0 = 0.0, t1 = 1.0;
            double dx = p2.X - p1.X;
            double dy = p2.Y - p1.Y;

            double[] p = { -dx, dx, -dy, dy };
            double[] q = { p1.X - rect.Left, rect.Right - p1.X, p1.Y - rect.Top, rect.Bottom - p1.Y };

            for (int i = 0; i < 4; i++)
            {
                if (p[i] == 0 && q[i] < 0)
                {
                    return false; // Line is parallel to the boundary and outside the clipping window
                }

                double r = q[i] / p[i];
                if (p[i] < 0)
                {
                    if (r > t1)
                        return false;
                    else if (r > t0)
                        t0 = r;
                }
                else if (p[i] > 0)
                {
                    if (r < t0)
                        return false;
                    else if (r < t1)
                        t1 = r;
                }
            }

            Point newP1 = new Point((int)(p1.X + t0 * dx), (int)(p1.Y + t0 * dy));
            Point newP2 = new Point((int)(p1.X + t1 * dx), (int)(p1.Y + t1 * dy));

            p1 = newP1;
            p2 = newP2;

            return true;
        }
    }
}
