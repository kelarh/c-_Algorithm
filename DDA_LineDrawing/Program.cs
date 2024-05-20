using System;
using System.Drawing;
using System.Windows.Forms;

namespace DDA_LineDrawing
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
            this.Text = "DDA Line Drawing Algorithm";
            this.Size = new Size(800, 600);
            this.Paint += new PaintEventHandler(OnPaint);
            this.BackColor = Color.Black; // 设置背景颜色为黑色
        }

        private void OnPaint(object? sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            DrawLineDDA(g, 100, 100, 700, 500);
        }

        private void DrawLineDDA(Graphics g, int x0, int y0, int x1, int y1)
        {
            int dx = x1 - x0;
            int dy = y1 - y0;
            int steps = Math.Max(Math.Abs(dx), Math.Abs(dy));

            float xIncrement = dx / (float)steps;
            float yIncrement = dy / (float)steps;

            float x = x0;
            float y = y0;

            for (int i = 0; i <= steps; i++)
            {
                g.FillRectangle(Brushes.White, x, y, 1, 1);
                x += xIncrement;
                y += yIncrement;
            }
        }
    }
}
