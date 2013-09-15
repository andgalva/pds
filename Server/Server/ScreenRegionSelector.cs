using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Server
{
    public partial class ScreenRegionSelector : Form
    {
        private bool mouseLeftDown = false;
        private Graphics graphics;
        private Point pointClicked, pointClicking;
        private Pen drawingPen, erasingPen;
        private Rectangle rectangle;

        // oggetto che viene esportato (alla MainForm/Server), contiene le coordinate e dimensioni scelte dall'utente
        public Rectangle getRectangle() { return rectangle; }

        public ScreenRegionSelector()
        {
            InitializeComponent();
            this.Top = 0;
            this.Left = 0;
            this.Height = Screen.PrimaryScreen.Bounds.Bottom;
            this.Width = Screen.PrimaryScreen.Bounds.Width;
            label1.ForeColor = Color.Red;
            label1.Text = "Choose an area of the screen,\nuse left mouse button";
            drawingPen = new Pen(Color.Red, 4);
            drawingPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            erasingPen = new Pen(this.BackColor, 4);
            graphics = CreateGraphics();
        }

        private void Mouse_Down(object sender, MouseEventArgs e)
        {
            mouseLeftDown = true;
            pointClicked = new Point(System.Windows.Forms.Control.MousePosition.X, System.Windows.Forms.Control.MousePosition.Y);
            pointClicking = new Point(System.Windows.Forms.Control.MousePosition.X, System.Windows.Forms.Control.MousePosition.Y);
            rectangle = new Rectangle(pointClicked, new Size(0, 0));
        }

        private void Mouse_Up(object sender, MouseEventArgs e)
        {
            mouseLeftDown = false;
            Thread.Sleep(200);

            if (rectangle.Width < 10 || rectangle.Height < 10)
            {
                MessageBox.Show("You have to choose a bigger area (at least 10x10)", "Area automatically resized",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                rectangle.Width = 10;
                rectangle.Height = 10;
            }

            Close();
        }

        private void Mouse_Move(object sender, MouseEventArgs e)
        {
            if (!mouseLeftDown)
                return;

            // prima di disegnare il rettangolo devo cancellare il precedente...
            graphics.DrawRectangle(erasingPen, rectangle);
            pointClicking.X = Cursor.Position.X;
            pointClicking.Y = Cursor.Position.Y;
            rectangle.X = Math.Min(pointClicked.X, pointClicking.X);
            rectangle.Y = Math.Min(pointClicked.Y, pointClicking.Y);
            rectangle.Width = Math.Abs(pointClicked.X - pointClicking.X);
            rectangle.Height = Math.Abs(pointClicked.Y - pointClicking.Y);
            label1.Text = "from: " +rectangle.Location.ToString() + "\nto: {X=" +
                        (rectangle.X + rectangle.Width) + ",Y=" + (rectangle.Y + rectangle.Height) + "}";
            graphics.DrawRectangle(drawingPen, rectangle);
        }

        private void labelCenter(object sender, EventArgs e)
        {
            this.label1.Top = (this.Height - this.label1.Height) / 2;
            this.label1.Left = (this.Width - this.label1.Width) / 2;
        }

        private void ScreenRegionSelector_Load(object sender, EventArgs e)
        {

        }

    }
}