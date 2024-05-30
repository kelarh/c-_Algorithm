using System;
using System.Drawing;
using System.Windows.Forms;

namespace myproject
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }

    public class MainForm : Form
    {
        private int dx = 50;  // 平移量
        private int dy = 50;

        public MainForm()
        {
            this.Text = "Translation Algorithm";
            this.ClientSize = new Size(800, 600);
            this.Paint += new PaintEventHandler(MainForm_Paint);

            // 添加按钮触发平移
            Button translateButton = new Button();
            translateButton.Text = "Translate";
            translateButton.Location = new Point(10, 10);
            translateButton.Click += new EventHandler(TranslateButton_Click);
            this.Controls.Add(translateButton);
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            // 原始直线
            int x1 = 100, y1 = 100;
            int x2 = 700, y2 = 500;

            // 绘制原始直线
            DrawLine(e.Graphics, x1, y1, x2, y2, Color.White);

            // 计算平移后的新坐标
            int newX1 = x1 + dx;
            int newY1 = y1 + dy;
            int newX2 = x2 + dx;
            int newY2 = y2 + dy;

            // 绘制平移后的直线
            DrawLine(e.Graphics, newX1, newY1, newX2, newY2, Color.Red);
        }

        private void DrawLine(Graphics g, int x1, int y1, int x2, int y2, Color color)
        {
            // 计算起点和终点之间的差值
            int dx = x2 - x1;
            int dy = y2 - y1;

            // 计算步数
            int steps = Math.Max(Math.Abs(dx), Math.Abs(dy));

            // 计算每一步的增量
            float xIncrement = dx / (float)steps;
            float yIncrement = dy / (float)steps;

            // 初始化起点
            float x = x1;
            float y = y1;

            // 创建画笔
            Pen pen = new Pen(color, 1);

            // 画线
            for (int i = 0; i <= steps; i++)
            {
                g.DrawRectangle(pen, (int)Math.Round(x), (int)Math.Round(y), 1, 1);
                x += xIncrement;
                y += yIncrement;
            }
        }

        private void TranslateButton_Click(object sender, EventArgs e)
        {
            // 更新平移量并重绘
            dx += 10;
            dy += 10;
            this.Invalidate();  // 触发重绘
        }
    }
}
