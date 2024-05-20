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
            this.Text = "Midpoint Line Clipping Algorithm";
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
                if (MidpointLineClip(ref p1, ref p2, viewport))
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

        // 中点裁剪算法
        private bool MidpointLineClip(ref Point p1, ref Point p2, Rectangle rect)
        {
            int outcode1 = ComputeOutCode(p1, rect);
            int outcode2 = ComputeOutCode(p2, rect);

            // 完全在窗口内
            if ((outcode1 | outcode2) == 0)
            {
                return true;
            }

            // 完全在窗口外
            if ((outcode1 & outcode2) != 0)
            {
                return false;
            }

            // 递归裁剪
            return Clip(ref p1, ref p2, rect);
        }

        private bool Clip(ref Point p1, ref Point p2, Rectangle rect)
        {
            int outcode1 = ComputeOutCode(p1, rect);
            int outcode2 = ComputeOutCode(p2, rect);

            // 计算中点
            Point pm = new Point((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);

            int outcodeM = ComputeOutCode(pm, rect);

            // 如果中点等于其中一个端点，返回 false
            if (pm == p1 || pm == p2)
            {
                return false;
            }

            // 如果中点在窗口内，继续裁剪
            if (outcodeM == INSIDE)
            {
                if (outcode1 != INSIDE)
                {
                    p1 = pm;
                    return Clip(ref p1, ref p2, rect);
                }
                else
                {
                    p2 = pm;
                    return Clip(ref p1, ref p2, rect);
                }
            }
            else
            {
                if ((outcode1 & outcodeM) == 0)
                {
                    p2 = pm;
                    return Clip(ref p1, ref p2, rect);
                }
                else
                {
                    p1 = pm;
                    return Clip(ref p1, ref p2, rect);
                }
            }
        }
    }
}
