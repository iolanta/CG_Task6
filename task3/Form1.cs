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
        //private PointF p0,p1,p2,p3;
        private BindingList<CurvePoint> plist = new BindingList<CurvePoint>();

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            var font = new Font("Tahoma", 5, FontStyle.Bold);
            var br = new System.Drawing.SolidBrush(System.Drawing.Color.Red);
            Pen blackPen = new Pen(Color.BlueViolet, 2);
            Pen dashed = new Pen(Color.Black, 1);
            dashed.DashPattern = new float[] { 1, 1 };
            foreach (var p in plist) {
                e.Graphics.DrawEllipse(new Pen(Color.Black),p.location.X - (float)1.5, p.location.Y - (float)1.5, 3, 3);
            }
            if (plist.Count > 1) 
                e.Graphics.DrawLines(dashed, plist.Select(p => { return p.location; }).ToArray());

            for (int i = 1; i < plist.Count-1; i+=2)
            {
                PointF p0,p1,p2,p3;

                int prev = i - 1; // p0
                int next = i + 2; // p3 

                if (prev == 0)
                    p0 = plist[prev].location;
                else
                    p0 = new PointF((plist[i].location.X + plist[prev].location.X) / 2, (plist[i].location.Y + plist[prev].location.Y) / 2);



                if (i == plist.Count - 1)
                {
                    p3 = plist[i].location;
                    var dif = new PointF((p3.X - p0.X) / 3, (p3.Y - p0.Y) / 3);
                    p1 = new PointF(p0.X + dif.X, p0.Y + dif.Y);
                    p2 = new PointF(p3.X - dif.X, p3.Y - dif.Y);
                }
                else if (i == plist.Count - 2)
                {
                    p1 = plist[i].location;
                    p3 = plist[i + 1].location;
                    p2 = new PointF((p3.X + p1.X) / 2, (p3.Y + p1.Y) / 2);
                }
                else {
                   p1 = plist[i].location;
                   p2= plist[i + 1].location;
                    if (next == plist.Count - 1)
                        p3 = plist[next].location;
                    else
                        p3 = new PointF((plist[next].location.X + p2.X) / 2, (plist[next].location.Y + p2.Y) / 2);
                }

               

               
                e.Graphics.DrawLines(blackPen, calcalute_curve(p0,p1,p2,p3));

            }

            
            

            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            plist.Add(new CurvePoint());
            plist.Last().location = new PointF(pictureBox1.Width / 2, pictureBox1.Height / 2);
            comboBox1.SelectedIndex = plist.Count - 1;
            pictureBox1.Invalidate();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex != -1) {
                plist.Remove(comboBox1.SelectedItem as CurvePoint);
            }
            pictureBox1.Invalidate();
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (comboBox1.SelectedIndex != -1)
                (comboBox1.SelectedItem as CurvePoint).location = e.Location;
            pictureBox1.Invalidate();
        }



 
        private PointF[] calcalute_curve(PointF p0, PointF p1, PointF p2, PointF p3) {
            float disc = (float) 0.001;
            int cnt = (int)(1 / disc) +1;
            PointF[] res = new PointF[cnt];

            float t = 0;
            for (int i = 0; i < cnt; i++)
            {
                float mt = 1 - t;
                float mt2 = mt * mt;
                float x;
                float y;
                float t2 = t * t;
                x = p3.X * t2 * t;
                y = p3.Y * t2 * t;
                x += 3 * t2 * p2.X * mt;
                y += 3 * t2 * p2.Y * mt;
                x += 3 * t * p1.X * mt2;
                y += 3 * t * p1.Y * mt2;
                x += p0.X * mt2 * mt;
                y += p0.Y * mt2 * mt;

                t += disc;
                res[i] = new PointF(x, y);
            }
            return res;
        }


        public Form1()
        {
            
            InitializeComponent();
            comboBox1.ValueMember = "location";
            comboBox1.DisplayMember = "text";
            comboBox1.DataSource = plist;

            //plist.Add(new CurvePoint());
           // plist.Add(new CurvePoint());
           // plist.Add(new CurvePoint());

        }
    }

    public class CurvePoint: INotifyPropertyChanged
    {
        private static int cnt =0;
        private string name;
        private PointF p;
        public CurvePoint() { name = "Point " + cnt.ToString();p = new PointF(0, 0);  cnt++; }
        public CurvePoint(string n, PointF _p) {
            name = n;
            p = _p;
        }

        public string text
        {
            get { return name; }
            set { name = value; }
        }

        public PointF location
        {
            get { return p; }
            set { p = value; }
        }

        private void RaisePropertyChanged(string propertyName)
        {
            var temp = PropertyChanged;
            if (temp != null)
                temp(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;


    }
}
