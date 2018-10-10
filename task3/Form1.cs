using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace task3
{
    public partial class Form1 : Form
    {
        private PointF p0,p1,p2,p3;

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            var font = new Font("Tahoma", 12, FontStyle.Bold);
            var br = new System.Drawing.SolidBrush(System.Drawing.Color.Red);
            Pen blackPen = new Pen(Color.Black, 1);
            blackPen.DashPattern =new float[] { 1,1 };

            e.Graphics.DrawLines(blackPen, new PointF[] { p0, p1, p2, p3 });
            e.Graphics.DrawEllipse(new Pen(Color.Black), p0.X - (float)1.5, p0.Y - (float)1.5, 3, 3);
            e.Graphics.DrawEllipse(new Pen(Color.Black), p1.X - (float)1.5, p1.Y - (float)1.5, 3, 3);
            e.Graphics.DrawEllipse(new Pen(Color.Black), p2.X - (float)1.5, p2.Y - (float)1.5, 3, 3);
            e.Graphics.DrawEllipse(new Pen(Color.Black), p3.X - (float)1.5, p3.Y - (float)1.5, 3, 3);
            e.Graphics.DrawString("P0", font, br, p0);
            e.Graphics.DrawString("P1", font, br, p1);
            e.Graphics.DrawString("P2", font, br, p2);
            e.Graphics.DrawString("P3", font, br, p3);
           

            List<PointF> l = new List<PointF>();
            for (float t = 0; t <= 1; t += (float)0.01) {
                l.Add(calculate_point(t));
                //e.Graphics.DrawEllipse(new Pen(Color.Red), p.X - (float)1.5, p.Y - (float)1.5, 3, 3);

            }
            e.Graphics.DrawLines(new Pen(Color.Aquamarine,2), l.ToArray());
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (radioButton1.Checked) {
                p0 = e.Location;
            }
            else if (radioButton2.Checked) {
                p1 = e.Location;
            }

            else if (radioButton3.Checked) {
                p2 = e.Location;
            }

            else if (radioButton4.Checked) {
                p3 = e.Location;
            }
            pictureBox1.Invalidate();
        }

        private PointF calculate_point(float t) {
            float mt = 1 - t;
            float mt2 = mt*mt;
            float x;
            float y;
            float t2 = t*t;
            x = p3.X*t2*t;
            y = p3.Y*t2*t;
            x += 3 * t2 * p2.X * mt;
            y += 3 * t2 * p2.Y * mt;
            x += 3 * t * p1.X * mt2;
            y += 3 * t * p1.Y * mt2;
            x += p0.X * mt2*mt;
            y += p0.Y * mt2*mt;
            return new PointF(x, y);

        }


        public Form1()
        {
            InitializeComponent();
            p0 = new PointF(100, 100);
            p1 = new PointF(100, 200);
            p2 = new PointF(200, 200);
            p3 = new PointF(200 ,100);

        }
    }
}
