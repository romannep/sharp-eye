using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EyeXFramework;
using Tobii.EyeX.Framework;


//%%% TODO - какой то спец таймер для анимации без мерцания
namespace sharp_eye
{

    public partial class Form1 : Form
    {
        Graphics gr,fgr;      
        Pen p;        
        SolidBrush fon; 
        SolidBrush fig;
        Bitmap bm;
        private Form2 form2 = new Form2();
        

        private EyeXHost eyeX;

        double PCM=55.4, SPEED=4.0, FW=24.0, FH=16.0, FREQ=50.0, DS=0.3;
        double eyeK = 1.0;
        //Pixels per CM, cm per second, Field Width in cm, Field Heigth in cm, frames per sec, Dot Size in cm
        int DY = 30;
        //field Delta by Y

        double bcrad = 7, bcd = 3, ll0 = 8.5, scrad, scx, scy;
        double lineLen, ll1, ll2, ll3;
        int tmpx, tmpy;
        Point cP;
        double etmpx, etmpy;
        GazePointDataStream stream;

        List<Point> gazeData;

        int rad = 20;
        int centr = 200;
        int radius = 200;
        float angle = 0;
        int step = 1;

        private void button4_Click(object sender, EventArgs e)
        {
            MessageBox.Show("tB = " + form2.getText());
            form2.Show();
        }

        Point pointCM(double cmx, double cmy)
        {
            return new Point((int)(PCM*(FW / 2 + cmx)), (int)(PCM * (FH / 2 - cmy)));
        }
        double length45x(double length)
        {
            return length * Math.Sin(Math.PI / 4);
        }
        Point pointRl(double xc, double yc, double r, double startAngle, double length)
        {
            return pointCM(xc+r*Math.Cos(startAngle + length/r),yc+r*Math.Sin(startAngle+length/r));
        }
        void drawDot(int x, int y)
        {
            //gr.FillRectangle(fon, 0, 0, 500, 500);
            gr.DrawEllipse(p, x, y, (int)(DS*PCM), (int)(DS* PCM));
            gr.FillEllipse(fig, x, y, (int)(DS * PCM), (int)(DS * PCM));
            //updateGr();

        }
 
        public Form1()
        {
            InitializeComponent();
            DoubleBuffered = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //gazeData.Add(this.PointToClient(new Point((int)((int)etmpx * 0.8), (int)((int)etmpy * 0.8)-DY)));
            //gazeData.Add(new Point((int)etmpx - this.Location.X, (int)etmpy-this.Location.Y));

            gr.FillRectangle(fon, 0, 0, (int) (PCM * FW), (int)(PCM*FH));

            if (lineLen < ll0)
            {
                cP = pointCM(length45x(lineLen), length45x(lineLen));
                //cP = pointCM(-5, -5);
            }
            else if (lineLen < ll1 + ll0)
            {
                cP = pointRl(scx, scy, scrad, Math.PI / 4 * 3, ll0 - lineLen);
                //cP = pointCM(0, 0);
            }
            else if (lineLen < ll1 + ll0 + ll2)
            {
                cP = pointRl(scx, scy, scrad, 0 - Math.PI / 4, 0);
                cP.X -= (int)(PCM * length45x(lineLen - ll0 - ll1));
                cP.Y += (int)(PCM * length45x(lineLen - ll0 - ll1));
                //cP = pointCM(5, 5);
            }
            else if (lineLen < ll1 + ll0 + ll2 + ll3)
            {
                cP = pointRl(0 - bcd, 0, bcrad, 0 - Math.PI / 4, ll0 + ll1 + ll2 - lineLen);
                //cP = pointCM(-5, 5);
            }
            // /*
            else if (lineLen < ll1 + ll0 + ll2 * 2 + ll3)
            {
                cP = pointRl(0 - bcd, 0, bcrad, Math.PI / 4, 0);
                cP.X += (int)(PCM * length45x(lineLen - ll0 - ll1 - ll2 - ll3));
                cP.Y += (int)(PCM * length45x(lineLen - ll0 - ll1 - ll2 - ll3));
            }
            else if (lineLen < ll0 + ll1 * 2 + ll2 * 2 + ll3)
            {
                cP = pointRl(scx, 0 - scy, scrad, Math.PI / 4, ll1 + ll0 + ll2 * 2 + ll3 - lineLen);
            }
            else if (lineLen < ll0 * 3 + ll1 * 2 + ll2 * 2 + ll3)
            {
                cP = pointRl(scx, 0 - scy, scrad, 0 - Math.PI * 3 / 4, 0);
                cP.X -= (int)(PCM * length45x(lineLen - (ll0 + ll1 * 2 + ll2 * 2 + ll3)));
                cP.Y -= (int)(PCM * length45x(lineLen - (ll0 + ll1 * 2 + ll2 * 2 + ll3)));
            }
            else if (lineLen < ll0 * 3 + ll1 * 3 + ll2 * 2 + ll3)
            {
                cP = pointRl(0 - scx, scy, scrad, Math.PI / 4, lineLen - (ll0 * 3 + ll1 * 2 + ll2 * 2 + ll3));
            }
            else if (lineLen < ll0 * 3 + ll1 * 3 + ll2 * 3 + ll3)
            {
                cP = pointRl(0 - scx, scy, scrad, Math.PI * 5 / 4, 0);
                cP.X += (int)(PCM * length45x(lineLen - (ll0 * 3 + ll1 * 3 + ll2 * 2 + ll3)));
                cP.Y += (int)(PCM * length45x(lineLen - (ll0 * 3 + ll1 * 3 + ll2 * 2 + ll3)));
            }
            else if (lineLen < ll0 * 3 + ll1 * 3 + ll2 * 3 + ll3 * 2)
            {
                cP = pointRl(bcd, 0, bcrad, 0 - Math.PI * 3 / 4, lineLen - (ll0 * 3 + ll1 * 3 + ll2 * 3 + ll3));
            }
            else if (lineLen < ll0 * 3 + ll1 * 3 + ll2 * 4 + ll3 * 2)
            {
                cP = pointRl(bcd, 0, bcrad, Math.PI * 3 / 4, 0);
                cP.X -= (int)(PCM * length45x(lineLen - (ll0 * 3 + ll1 * 3 + ll2 * 3 + ll3 * 2)));
                cP.Y += (int)(PCM * length45x(lineLen - (ll0 * 3 + ll1 * 3 + ll2 * 3 + ll3 * 2)));
            }
            else if (lineLen < ll0 * 3 + ll1 * 4 + ll2 * 4 + ll3 * 2)
            {
                cP = pointRl(0 - scx, 0 - scy, scrad, Math.PI * 3 / 4, lineLen - (ll0 * 3 + ll1 * 3 + ll2 * 4 + ll3 * 2));
            }
            else if (lineLen < ll0 * 4 + ll1 * 4 + ll2 * 4 + ll3 * 2)
            {
                cP = pointRl(0 - scx, 0 - scy, scrad, 0 - Math.PI * 1 / 4, 0);
                cP.X += (int)(PCM * length45x(lineLen - (ll0 * 3 + ll1 * 4 + ll2 * 4 + ll3 * 2)));
                cP.Y -= (int)(PCM * length45x(lineLen - (ll0 * 3 + ll1 * 4 + ll2 * 4 + ll3 * 2)));
            }
            // */
            else
            {
                timer1.Enabled = false;
                stream.Next += null;
                stream.Dispose();
                gr.FillRectangle(fon, 0, 0, (int)(PCM * FW), (int)(PCM * FH));
                for (var i =3; i<gazeData.Count; i++)
                {
                    gr.DrawLine(p, gazeData[i-1].X, gazeData[i-1].Y, gazeData[i].X, gazeData[i].Y);
                }
                cP = pointCM(-5, -5);
                drawDot(cP.X, cP.Y);
                cP = pointCM(0, 0);
                drawDot(cP.X, cP.Y);
                cP = pointCM(5, 5);
                drawDot(cP.X, cP.Y);
                cP = pointCM(-5, 5);
                drawDot(cP.X, cP.Y);

            }

            drawDot(cP.X, cP.Y);
            lineLen += SPEED / FREQ;

            /*
            angle += step;
            if (angle>360) { angle = 0; timer1.Enabled = false; }
            double posx = centr + radius * Math.Sin(Math.PI/2 + angle / 360*Math.PI*2);
            double posy = centr + radius * Math.Cos(Math.PI/2 + angle / 360*Math.PI*2);
            drawCircle((int) posx, (int) posy);
            */

            Invalidate();
            

            /*
            textBox1.Text = "Coords x:" + (int)tmpx + " y:" + (int)tmpy;
            drawCircle(point.X, point.Y);
            */
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            bm = new Bitmap((int)(PCM * FW), (int)(PCM * FH), System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            gr = Graphics.FromImage(bm);
            p = new Pen(Color.Black);           // задали цвет для карандаша 
            fon = new SolidBrush(Color.White); // и для заливки
            fig = new SolidBrush(Color.Black);
            gr.FillRectangle(fon, 0, 0, (int)(PCM * FW), (int)(PCM * FH));
            cP = pointCM(0, 0);
            drawDot(cP.X, cP.Y);


            eyeX = new EyeXHost();
            eyeX.Start();

            gazeData = new List<Point>();
            /*
            gazeData.Add(new Point(50, 50));
            gazeData.Add(new Point(500, 150));
            for (var i = 1; i < gazeData.Count; i++)
            {
                gr.DrawLine(p, gazeData[i - 1].X, gazeData[i - 1].Y, gazeData[i].X, gazeData[i].Y);
            }
            gazeData.Clear();
            */

            Invalidate();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //fill consts
            scrad = (bcrad - bcd * Math.Cos(Math.PI / 4))/2;
            scx = length45x(ll0) + scrad * Math.Cos(Math.PI / 4);
            scy = length45x(ll0) - scrad * Math.Cos(Math.PI / 4);
            ll1 = Math.PI * scrad;
            ll2 = (ll0 + scrad * 2 - bcrad) + bcd / Math.Cos(Math.PI / 4);
            ll3 = Math.PI * bcrad * 1.5;
            lineLen = 0;
            gazeData.Clear();
            //gazeData.Add(pointCM(0, 0));
            //set timer
            timer1.Interval = (int)( 1 / FREQ * 1000);
            timer1.Enabled = true;
            //start streaming gaze data
            stream = eyeX.CreateGazePointDataStream(GazePointDataMode.Unfiltered);
            stream.Next += Stream_Next;



            /*
            var centrPoint = pointCM(0, 0);
            //var p1 = pointCM(length45x(8), length45x(8));
            //gr.DrawLine(p, centrPoint.X, centrPoint.Y, p1.X, p1.Y);
            var p2 = pointRl(3.0,0,7,Math.PI/4*3,0);
            drawDot(p2.X, p2.Y);
            p2 = pointRl(3.0, 0, 7, Math.PI / 4 * 3, -3);
            drawDot(p2.X, p2.Y);
            p2 = pointRl(3.0, 0, 7, Math.PI / 4 * 3, -6);
            drawDot(p2.X, p2.Y);
            p2 = pointRl(3.0, 0, 7, Math.PI / 4 * 3, -8);
            drawDot(p2.X, p2.Y);
            p2 = pointRl(3.0, 0, 7, Math.PI / 4 * 3, -Math.PI*1.5*7);
            drawDot(p2.X, p2.Y);
            
            Invalidate();
            */


            
            
            

            /*
            timer1.Enabled = true;
            */

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        private void ToList(Point point)
        {
          
            if (InvokeRequired)
                Invoke((Action<Point>)ToList, point);
            else
                gazeData.Add(this.PointToClient(point));
        }

        private void Stream_Next(object sender, GazePointEventArgs e)
        {
            //drawCircle(100, 100);
            //textBox1.Text = "Gaze point at " + e.X + " " + e.Y;
            etmpx = e.X;
            etmpy = e.Y;
            ToList(new Point((int)((int)etmpx * eyeK), (int)((int)etmpy * eyeK) - DY));
            //gazeData.Add(this.PointToClient(new Point((int)etmpx, (int)etmpy)));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            bm.Save("C:\\sharp-eye\\" + textBox1.Text + ".gif", System.Drawing.Imaging.ImageFormat.Gif);
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            fgr = e.Graphics;
            fgr.DrawImage(bm, 0, DY);
            label2.Text = "Form location " + this.Location.X + " " + this.Location.Y + " " ;
       }
        void updateGr()
        {
            
        }
        void drawCircle(int x, int y)
        {
            //gr.FillRectangle(fon, 0, 0, 500, 500);
            gr.DrawEllipse(p, x, y, rad, rad);
            gr.FillEllipse(fig, x, y, rad, rad);
            //updateGr();

        }
    }
}
