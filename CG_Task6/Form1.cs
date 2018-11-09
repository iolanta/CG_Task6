using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace CG_Task6
{
    public partial class Form1 : Form
    {
        double turn_angle;
        double start_angle;
        string start;
        Dictionary<char, string> rules;
        char[] list_n;
        string fractal;
        int depth;
        Frac_point start_node;
        PointF max_p, min_p;
        bool rand_turn;
        Random rand_ang = new Random();
        int max_lvl = 0;

        public Form1()
        {
            InitializeComponent();
            turn_angle = 0;
            start_angle = 0;
            rules = new Dictionary<char, string>();
            start_node = new Frac_point(new PointF(0, 0));
            max_p = new PointF(0, 0);
            min_p = new PointF(0, 0);
            depth = 2; 
            rand_turn = false;
        }

        private void parse_input(string str)
        {
            string[] lst = str.Split(new string[] { "\r\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
            list_n = new char[lst.GetLength(0)-1];
            int index = 0;
            int angle = 0;

            while (!char.IsDigit(lst[0][index]))
            {
                start += lst[0][index];
                ++index;
            }
            
            while (!char.IsSymbol(lst[0][index]))
            {
                angle = angle * 10 + (int)(lst[0][index] - '0');
                ++index;
            }
            turn_angle = angle * Math.PI / 180;

            angle = 0;
            ++index;
            while(!char.IsSymbol(lst[0][index]))
            {
                angle = angle * 10 + (int)(lst[0][index] - '0');
                ++index;
            }
            start_angle = angle * Math.PI / 180;

            for (int i = 1; i < lst.GetLength(0); ++i)
            {
                char n = lst[i][0];
                list_n[i - 1] = n;
                string r = "";
                index = 2;

                while (index != lst[i].Length)
                {
                    r += lst[i][index];
                    ++index;
                }

                rules.Add(n, r);
                
            }
        }

        private int add_rule(int ind)
        {
            string s = rules[fractal[ind]];
            fractal = fractal.Remove(ind, 1);
            fractal = fractal.Insert(ind, s);
            return s.Length+ind;
        }
        
        private void apply_rules()
        {
            fractal = start;
            int d = depth;
            int i = 0;
            while(d != 0) 
            {
                while (true)
                {
                    if (i >= fractal.Length)
                        break;
                    int ind = fractal.IndexOfAny(list_n, i);
                    if (ind == -1)
                        break;
                    i = add_rule(ind);
                }
                --d;
                i = 0;
            }
        }

        private void create_fractal()
        {
            Frac_point cur = start_node;
            Stack<Tuple<Frac_point, double>> st = new Stack<Tuple<Frac_point, double>>();
            int lvl = 0;

            double angle = start_angle;
            foreach(char c in fractal)
            {
                if (c == '+')
                {
                    if (rand_turn)
                        angle += turn_angle + (rand_ang.Next(-20, 20) * Math.PI / 180);
                    else
                        angle += turn_angle;
                    ++lvl;
                }
                else if (c == '-')
                {
                    if (rand_turn)
                        angle -= turn_angle + (rand_ang.Next(-20, 20) * Math.PI / 180);
                    else
                        angle -= turn_angle;
                    ++lvl;
                }

                else if (c == '[')
                {
                    st.Push(new Tuple<Frac_point, double>(cur, angle));
                }
                else if (c == ']')
                {
                    var t = st.Pop();
                    lvl = t.Item1.level;
                    cur = t.Item1;
                    angle = t.Item2;
                }
                else if (c == '@')
                {
                    rand_turn = true;
                }
                else if (c == 'F')
                {
                    max_lvl = Math.Max(max_lvl, lvl);
                    PointF next = new PointF(cur.p.X + (float)Math.Cos(angle) * 10, cur.p.Y + (float)Math.Sin(angle) * 10);
                    min_p.X = Math.Min(min_p.X, next.X);
                    min_p.Y = Math.Min(min_p.Y, next.Y);
                    max_p.X = Math.Max(max_p.X, next.X);
                    max_p.Y = Math.Max(max_p.Y, next.Y);

                    Frac_point t = new Frac_point(next);
                    t.level = lvl;
                    cur.fpoints.Add(t);
                    cur = t;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
                return;
            
            string filename = openFileDialog1.FileName;
            string fileText = System.IO.File.ReadAllText(filename);
            fileText = fileText.Replace(" ", string.Empty);
            rules.Clear();
            start_node = new Frac_point(new PointF(0, 0));
            max_p = new PointF(0, 0);
            min_p = new PointF(0, 0);
            start = "";

            parse_input(fileText);
            apply_rules();
            create_fractal();
            pictureBox1.Invalidate();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            depth = (int)numericUpDown1.Value;
            if (fractal == null)
                return;
            start_node = new Frac_point(new PointF(0, 0));
            max_p = new PointF(0, 0);
            min_p = new PointF(0, 0);
            
            apply_rules();
            create_fractal();
            pictureBox1.Invalidate();
        }

        private static int[] Interpolate(int i0, int d0, int i1, int d1)
        {
            if (i0 == i1)
            {
                return new int[] { d0 };
            }
            int[] res;
            float a = (float)(d1 - d0) / (i1 - i0);
            float val = d0;
            res = new int[i1 - i0 + 1];
            d1 = 0;
            for (int i = i0; i <= i1; i++)
            {
                res[d1] = d0;
                val += a;
                d0 = (int)val;
                ++d1;
            }

            return res;
        }

        private static float[] FInterpolate(int i0, float d0, int i1, float d1)
        {
            if (i0 == i1)
            {
                return new float[] { d0 };
            }
            float[] res;
            float a = (d1 - d0) / (i1 - i0);
            float val = d0;
            res = new float[i1 - i0 + 1];
            int ind =  0;
            for (int i = i0; i <= i1; i++)
            {
                res[ind] = val;
                val += a;
                ++ind;
            }

            return res;
        }


        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {

            float w = max_p.X - min_p.X;
            float h = max_p.Y - min_p.Y;
            var scale = Math.Min((float)(pictureBox1.Width-75) / w, (float)(pictureBox1.Height- 75) / h);
            if (w == 0 || h == 0)
                scale = 1;
            // Перемещаем (0, 0) в центр окна и устанавливаем масштаб
            e.Graphics.ScaleTransform(scale, scale);
            e.Graphics.TranslateTransform(-min_p.X+50/scale,-min_p.Y + 50 / scale);

            Color c1 = Color.DeepPink;
            Color c2 = Color.DeepSkyBlue;
            float sw = 5f;
            float fw = 0.1f;
            int[] R = Interpolate(0, c1.R, max_lvl, c2.R);
            int[] G = Interpolate(0, c1.G, max_lvl, c2.G);
            int[] B = Interpolate(0, c1.B, max_lvl, c2.B);
            float[] W = FInterpolate(0, sw, max_lvl, fw);

            Stack<Frac_point> s = new Stack<Frac_point>();
            s.Push(start_node);
            while (s.Count > 0)
            {
                Frac_point cur = s.Pop();
                foreach(var fp in cur.fpoints)
                {
                    e.Graphics.DrawLine(new Pen(Color.FromArgb(R[cur.level], G[cur.level], B[cur.level]), W[cur.level]), cur.p, fp.p);
                    s.Push(fp);
                }
            }
        }
    }

    public class Frac_point
    {
        public PointF p;
        public List<Frac_point> fpoints;
        public int level;

        public Frac_point(PointF _p)
        {
            p = _p;
            fpoints = new List<Frac_point>();
        }
    }
}
