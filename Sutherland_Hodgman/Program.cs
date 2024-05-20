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
        private List<Point> polygon;
        private Rectangle viewport;

        public MainForm()
        {
            this.Text = "Polygon Clipping";
            this.Size = new Size(800, 600);
            this.DoubleBuffered = true;
            this.viewport = new Rectangle(100, 100, 400, 300);

            // 定义一个多边形
            polygon = new List<Point>
            {
                new Point(200, 100),
                new Point(300, 150),
                new Point(400, 200),
                new Point(300, 300),
                new Point(200, 350),
                new Point(100, 250)
            };
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            Pen polyPen = new Pen(Color.Black, 2);
            Pen viewportPen = new Pen(Color.Blue, 2);

            // 绘制多边形
            g.DrawPolygon(polyPen, polygon.ToArray());

            // 绘制视口
            g.DrawRectangle(viewportPen, viewport);

            // 裁剪多边形
            List<Point> clippedPolygon = SutherlandHodgmanClip(polygon, viewport);

            // 绘制裁剪后的多边形
            g.DrawPolygon(polyPen, clippedPolygon.ToArray());
        }

        private List<Point> SutherlandHodgmanClip(List<Point> polygon, Rectangle viewport)
        {
            List<Point> outputList = polygon.ToList();

            // 左，右，上，下
            List<Point> clipper = new List<Point> {
                new Point(viewport.Left, viewport.Top),
                new Point(viewport.Right, viewport.Top),
                new Point(viewport.Right, viewport.Bottom),
                new Point(viewport.Left, viewport.Bottom)
            };

            for (int edgeIndex = 0; edgeIndex < 4; edgeIndex++)
            {
                Point edgeStart = clipper[edgeIndex];
                Point edgeEnd = clipper[(edgeIndex + 1) % 4];

                List<Point> inputList = outputList.ToList();
                outputList.Clear();

                if (inputList.Count == 0)
                    break;

                Point s = inputList[inputList.Count - 1];

                foreach (Point p in inputList)
                {
                    if (IsInside(p, edgeStart, edgeEnd))
                    {
                        if (!IsInside(s, edgeStart, edgeEnd))
                        {
                            outputList.Add(ComputeIntersection(s, p, edgeStart, edgeEnd));
                        }
                        outputList.Add(p);
                    }
                    else if (IsInside(s, edgeStart, edgeEnd))
                    {
                        outputList.Add(ComputeIntersection(s, p, edgeStart, edgeEnd));
                    }
                    s = p;
                }
            }

            return outputList;
        }

        private bool IsInside(Point p, Point edgeStart, Point edgeEnd)
        {
            return (edgeEnd.X - edgeStart.X) * (p.Y - edgeStart.Y) > (edgeEnd.Y - edgeStart.Y) * (p.X - edgeStart.X);
        }

        private Point ComputeIntersection(Point p1, Point p2, Point edgeStart, Point edgeEnd)
        {
            int dx1 = p2.X - p1.X;
            int dy1 = p2.Y - p1.Y;
            int dx2 = edgeEnd.X - edgeStart.X;
            int dy2 = edgeEnd.Y - edgeStart.Y;
            int denom = dx1 * dy2 - dy1 * dx2;
            int numer = p1.X * dy2 - p1.Y * dx2 - edgeStart.X * dy2 + edgeStart.Y * dx2;
            int t = numer / denom;
            return new Point(p1.X + t * dx1, p1.Y + t * dy1);
        }
    }
}
