using System;
using System.Drawing;
using System.Windows.Forms;

namespace Bresenham_CircleDrawing
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
        public MainForm()
        {
            this.Text = "Bresenham Circle Drawing Algorithm";
            this.Size = new Size(800, 600);
            this.Paint += new PaintEventHandler(OnPaint);
            this.BackColor = Color.Black; // 设置背景颜色为黑色
        }

        private void OnPaint(object? sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            int centerX = this.ClientSize.Width / 2;
            int centerY = this.ClientSize.Height / 2;
            int radius = Math.Min(this.ClientSize.Width, this.ClientSize.Height) / 4; // 圆的半径为窗口宽高的1/4
            DrawCircleBresenham(g, centerX, centerY, radius);
        }

        private void DrawCircleBresenham(Graphics g, int centerX, int centerY, int radius)
        {
            int x = 0;
            int y = radius;
            int d = 3 - 2 * radius;

            while (x <= y)
            {
                // 绘制8个对称点
                DrawPixel(g, centerX + x, centerY + y);
                DrawPixel(g, centerX - x, centerY + y);
                DrawPixel(g, centerX + x, centerY - y);
                DrawPixel(g, centerX - x, centerY - y);
                DrawPixel(g, centerX + y, centerY + x);
                DrawPixel(g, centerX - y, centerY + x);
                DrawPixel(g, centerX + y, centerY - x);
                DrawPixel(g, centerX - y, centerY - x);

                if (d < 0)
                {
                    d += 4 * x + 6;
                }
                else
                {
                    d += 4 * (x - y) + 10;
                    y--;
                }
                x++;
            }
        }

        private void DrawPixel(Graphics g, int x, int y)
        {
            g.FillRectangle(Brushes.White, x, y, 1, 1);
        }
    }
}
