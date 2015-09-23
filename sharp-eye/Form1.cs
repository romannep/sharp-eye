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

namespace sharp_eye
{
    public partial class Form1 : Form
    {
        Graphics gr,fgr;      
        Pen p;        
        SolidBrush fon; 
        SolidBrush fig;
        Bitmap bm;

        private EyeXHost eyeX;

        double PCM=45.5, SPEED=7.0, FW=24.0, FH=16.0, FREQ=25.0;
        int DY = 30;

        int rad = 20;
        int centr = 200;
        int radius = 200;
        float angle = 0;
        int step = 1;
        double tmpx, tmpy;

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
        public Form1()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            gr.FillRectangle(fon, 0, 0, (int) (PCM * FW), (int)(PCM*FH));
            
            angle += step;
            if (angle>360) { angle = 0; timer1.Enabled = false; }
            double posx = centr + radius * Math.Sin(Math.PI/2 + angle / 360*Math.PI*2);
            double posy = centr + radius * Math.Cos(Math.PI/2 + angle / 360*Math.PI*2);
            drawCircle((int) posx, (int) posy);
            Invalidate();
            /*
            var point = this.PointToClient(new Point((int)tmpx ,(int)tmpy));
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
            Invalidate();

        }

        private void button1_Click(object sender, EventArgs e)
        {

            var centrPoint = pointCM(0, 0);
            var p1 = pointCM(length45x(8), length45x(8));
            gr.DrawLine(p, centrPoint.X, centrPoint.Y, p1.X, p1.Y);
            var p2 = pointRl(3.0,0,7,Math.PI/4*3,0);
            drawCircle(p2.X, p2.Y);
            p2 = pointRl(3.0, 0, 7, Math.PI / 4 * 3, -3);
            drawCircle(p2.X, p2.Y);
            p2 = pointRl(3.0, 0, 7, Math.PI / 4 * 3, -6);
            drawCircle(p2.X, p2.Y);
            p2 = pointRl(3.0, 0, 7, Math.PI / 4 * 3, -9);
            drawCircle(p2.X, p2.Y);
            p2 = pointRl(3.0, 0, 7, Math.PI / 4 * 3, -Math.PI*1.5*7);
            drawCircle(p2.X, p2.Y);

            Invalidate();
            /*
            eyeX = new EyeXHost();
            eyeX.Start();
            

            var stream = eyeX.CreateGazePointDataStream(GazePointDataMode.LightlyFiltered);
            stream.Next += Stream_Next;
            timer1.Enabled = true;
            */

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Stream_Next(object sender, GazePointEventArgs e)
        {
            //drawCircle(100, 100);
            //textBox1.Text = "Gaze point at " + e.X + " " + e.Y;
            tmpx = e.X;
            tmpy = e.Y;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            bm.Save("C:\\sharp-eye\\" + textBox1.Text + ".gif", System.Drawing.Imaging.ImageFormat.Gif);
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            fgr = e.Graphics;
            fgr.DrawImage(bm, 0, DY);
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
