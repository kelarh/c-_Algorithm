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
            this.Text = "Cohen-Sutherland Line Clipping Algorithm";
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
                if (CohenSutherlandClip(ref p1, ref p2, viewport))
                {
                    g.DrawLine(clippedLinePen, p1, p2);
                }
            }
        }

        // 定义区域码
        private const int INSIDE = 0; // 0000
        private const int LEFT = 1;   // 0001
        private const int RIGHT = 2;  // 0010
        private const int BOTTOM = 4; // 0100
        private const int TOP = 8;    // 1000

        // 计算点的区域码
        private int ComputeOutCode(Point p, Rectangle rect)
        {
            int code = INSIDE;

            if (p.X < rect.Left)
                code |= LEFT;
            else if (p.X > rect.Right)
                code |= RIGHT;
            if (p.Y < rect.Top)
                code |= TOP;
            else if (p.Y > rect.Bottom)
                code |= BOTTOM;

            return code;
        }

        // Cohen-Sutherland 裁剪算法
        private bool CohenSutherlandClip(ref Point p1, ref Point p2, Rectangle rect)
        {
            int outcode1 = ComputeOutCode(p1, rect);
            int outcode2 = ComputeOutCode(p2, rect);
            bool accept = false;

            while (true)
            {
                if ((outcode1 | outcode2) == 0)
                {
                    accept = true;
                    break;
                }
                else if ((outcode1 & outcode2) != 0)
                {
                    break;
                }
                else
                {
                    int outcodeOut = (outcode1 != 0) ? outcode1 : outcode2;
                    Point p = new Point();

                    if ((outcodeOut & TOP) != 0)
                    {
                        p.X = p1.X + (p2.X - p1.X) * (rect.Top - p1.Y) / (p2.Y - p1.Y);
                        p.Y = rect.Top;
                    }
                    else if ((outcodeOut & BOTTOM) != 0)
                    {
                        p.X = p1.X + (p2.X - p1.X) * (rect.Bottom - p1.Y) / (p2.Y - p1.Y);
                        p.Y = rect.Bottom;
                    }
                    else if ((outcodeOut & RIGHT) != 0)
                    {
                        p.Y = p1.Y + (p2.Y - p1.Y) * (rect.Right - p1.X) / (p2.X - p1.X);
                        p.X = rect.Right;
                    }
                    else if ((outcodeOut & LEFT) != 0)
                    {
                        p.Y = p1.Y + (p2.Y - p1.Y) * (rect.Left - p1.X) / (p2.X - p1.X);
                        p.X = rect.Left;
                    }

                    if (outcodeOut == outcode1)
                    {
                        p1 = p;
                        outcode1 = ComputeOutCode(p1, rect);
                    }
                    else
                    {
                        p2 = p;
                        outcode2 = ComputeOutCode(p2, rect);
                    }
                }
            }

            return accept;
        }
    }
}
