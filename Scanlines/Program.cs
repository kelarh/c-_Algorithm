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

        public MainForm()
        {
            this.Text = "Scanline Fill Algorithm";
            this.Size = new Size(800, 600);
            this.Paint += new PaintEventHandler(this.MainForm_Paint);

            // 定义一个多边形
            polygon = new List<Point>
            {
                new Point(100, 150),
                new Point(200, 250),
                new Point(300, 200),
                new Point(250, 100),
                new Point(150, 50)
            };
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // 绘制多边形的边界
            g.DrawPolygon(Pens.Black, polygon.ToArray());

            // 使用扫描线填充算法填充多边形
            ScanlineFill(g, polygon);
        }

        private void ScanlineFill(Graphics g, List<Point> polygon)
        {
            // 找到多边形的 y 范围
            int minY = polygon.Min(p => p.Y);
            int maxY = polygon.Max(p => p.Y);

            // 创建一个活动边表
            List<Edge> activeEdges = new List<Edge>();

            for (int y = minY; y <= maxY; y++)
            {
                // 添加新的边到活动边表
                for (int i = 0; i < polygon.Count; i++)
                {
                    Point p1 = polygon[i];
                    Point p2 = polygon[(i + 1) % polygon.Count];

                    if (p1.Y == p2.Y) // 忽略水平边
                        continue;

                    if (p1.Y < p2.Y)
                        activeEdges.Add(new Edge(p1, p2));
                    else
                        activeEdges.Add(new Edge(p2, p1));
                }

                // 移除 y 超过当前扫描线的边
                activeEdges.RemoveAll(edge => edge.YMax <= y);

                // 排序活动边表
                activeEdges.Sort((e1, e2) => e1.X.CompareTo(e2.X));

                // 填充扫描线之间的像素
                for (int i = 0; i < activeEdges.Count; i += 2)
                {
                    int xStart = (int)activeEdges[i].X;
                    int xEnd = (int)activeEdges[i + 1].X;

                    for (int x = xStart; x < xEnd; x++)
                    {
                        g.FillRectangle(Brushes.Red, x, y, 1, 1);
                    }

                    // 更新边的 x 值
                    activeEdges[i].X += activeEdges[i].InverseSlope;
                    activeEdges[i + 1].X += activeEdges[i + 1].InverseSlope;
                }
            }
        }

        private class Edge
        {
            public float X { get; set; }
            public int YMax { get; }
            public float InverseSlope { get; }

            public Edge(Point p1, Point p2)
            {
                X = p1.X;
                YMax = p2.Y;
                InverseSlope = (float)(p2.X - p1.X) / (p2.Y - p1.Y);
            }
        }
    }
}
