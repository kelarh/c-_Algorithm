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
        private float angle = 0;  // 旋转角度（以度为单位）

        public MainForm()
        {
            this.Text = "Rotation Algorithm";
            this.ClientSize = new Size(800, 600);
            this.Paint += new PaintEventHandler(MainForm_Paint);

            // 添加按钮触发旋转
            Button rotateButton = new Button();
            rotateButton.Text = "Rotate";
            rotateButton.Location = new Point(10, 10);
            rotateButton.Click += new EventHandler(RotateButton_Click);
            this.Controls.Add(rotateButton);
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            // 原始直线
            int x1 = 100, y1 = 100;
            int x2 = 700, y2 = 500;

            // 绘制原始直线
            DrawLine(e.Graphics, x1, y1, x2, y2, Color.White);

            // 计算旋转后的新坐标
            (int newX1, int newY1) = RotatePoint(x1, y1, angle);
            (int newX2, int newY2) = RotatePoint(x2, y2, angle);

            // 绘制旋转后的直线
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

        private (int, int) RotatePoint(int x, int y, float angle)
        {
            // 将角度转换为弧度
            float radians = angle * (float)Math.PI / 180;

            // 计算旋转后的新坐标
            int newX = (int)(x * Math.Cos(radians) - y * Math.Sin(radians));
            int newY = (int)(x * Math.Sin(radians) + y * Math.Cos(radians));

            return (newX, newY);
        }

        private void RotateButton_Click(object sender, EventArgs e)
        {
            // 更新旋转角度并重绘
            angle += 10;  // 每次旋转10度
            this.Invalidate();  // 触发重绘
        }
    }
}
