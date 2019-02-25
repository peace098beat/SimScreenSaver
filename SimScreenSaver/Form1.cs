using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace SimScreenSaver
{
    public partial class Form1 : Form
    {
        public Bitmap Icon1 { get; }
        public Bitmap Icon2 { get; }
        public Bitmap Icon3 { get; }
        public Bitmap Icon4 { get; }
        public Bitmap[] Icons { get; }


        public Stopwatch sw = Stopwatch.StartNew();

        List<SingleCPUManager> SingleCPUList = new List<SingleCPUManager>();

        public Form1()
        {
            InitializeComponent();


            for (int i = 0; i < SingleCPUManager.GetCPUCount(); i++)
            {
                SingleCPUList.Add(new SingleCPUManager(i, 200, 4, 30));
            }

            //バージョンの取得
            System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
            System.Version ver = asm.GetName().Version;
            label_version.Text = $"Assembly Version {ver.ToString()}";

            // 最大化
            this.WindowState = FormWindowState.Maximized;

            // 「埋め込みリソース」から画像を取得
            System.Reflection.Assembly myAssembly = System.Reflection.Assembly.GetExecutingAssembly();

            Icon1 = new Bitmap(myAssembly.GetManifestResourceStream(@"SimScreenSaver.img.v2-02.png"));
            Icon2 = new Bitmap(myAssembly.GetManifestResourceStream(@"SimScreenSaver.img.v2-03.png"));
            Icon3 = new Bitmap(myAssembly.GetManifestResourceStream(@"SimScreenSaver.img.v2-04.png"));
            Icon4 = new Bitmap(myAssembly.GetManifestResourceStream(@"SimScreenSaver.img.v2-05.png"));
            Icons = new Bitmap[] { Icon1, Icon2, Icon3, Icon4 };
        }

        /// <summary>
        /// 終了処理
        /// </summary>
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            // 保留
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// タイマー
        /// </summary>
        private void timer1_Tick(object sender, EventArgs e)
        {
            pictureBox1.Refresh();
        }

        //==================================================================
        //==================================================================

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Debug.WriteLine($"repaint {DateTime.Now}");

            // -------------- iconを表示
            int duration = 3; // sec
            int index = (sw.Elapsed.Seconds / duration) % Icons.Length;

            Graphics g = e.Graphics;

            Bitmap icon = this.Icons[index];

            Rectangle IconRect = new Rectangle();
            IconRect.Width = pictureBox1.Width / 2;
            IconRect.Height = pictureBox1.Height / 2;
            IconRect.X = (pictureBox1.Width / 2) - (IconRect.Width / 2);
            IconRect.Y = (pictureBox1.Height / 2) - (IconRect.Height / 2);

            // iconを描画
            // aspect比を合わせる
            if (pictureBox1.Width < pictureBox1.Height)
            {
                // 縦長
                double aspect = (double)icon.Height / icon.Width;
                double h = aspect * IconRect.Width;

                g.DrawImage(this.Icons[index], IconRect.X, IconRect.Y, IconRect.Width, (int)h);
            }
            else
            {
                // 縦長
                double aspect = (double)icon.Width / icon.Height;
                double w = aspect * IconRect.Height;
                g.DrawImage(this.Icons[index], IconRect.X, IconRect.Y, (int)w, IconRect.Height);
            }

            // -------------- /iconを表示



            // -------------- CPU使用率を表示

            int[] ave = CPUManager.GetAveValues();
            int[] max = CPUManager.GetMaxValues();
            int[] ptp = CPUManager.GetPeakToPeakValues();



            label_CPU.Text = "";
            label_CPU.Text += String.Join(", ", ave) + Environment.NewLine;
            label_CPU.Text += String.Join(", ", max) + Environment.NewLine;
            label_CPU.Text += String.Join(", ", ptp) + Environment.NewLine;


            // -------------- CPU使用率を表示

            int Ncpu = Environment.ProcessorCount;

            int W = pictureBox1.Width;
            int H = pictureBox1.Height;

            RectangleF ChartArea = new RectangleF(W*0.1f, 100, W*0.8f, (H * 0.1f));

            for (int k = 0; k < Ncpu; k++)
            {
                SingleCPUManager scpu = SingleCPUList[k];

                float[] cpu_values = scpu.GetQueArray();

                int N = cpu_values.Length;
                PointF[] Origins = new PointF[N];
                for (int i = 0; i < cpu_values.Length; i++)
                {
                    Origins[i].X = i;
                    Origins[i].Y = cpu_values[i];
                }

                float X0 = ChartArea.X + (ChartArea.Width / Ncpu) * k;
                float Y0 = ChartArea.Y ;

                Pen pen = new Pen(Color.Green, 1);

                RectangleF TargetScreen = new RectangleF();
                this.DrawLines(e, pen, Origins, 
                    new RectangleF(0, 0, cpu_values.Length*1.2f, 120),
                    new RectangleF(X0, Y0, ChartArea.Width / Ncpu, ChartArea.Height));

            }


        }


        /// <summary>
        /// 正規化座標へ変換
        /// x[0,1], y[0,1]の範囲
        /// </summary>
        /// <param name="Origins">ワールド座標の点</param>
        /// <param name="ClipRect">ワールド座標のクリップ領域. xy:左下. </param>
        static PointF Norm(PointF Origin, RectangleF ClipRect)
        {
            PointF NormPoint = new PointF();

            NormPoint.X = (Origin.X - ClipRect.X) / (ClipRect.Width) + ClipRect.X;
            NormPoint.Y = (Origin.Y - ClipRect.Y) / (ClipRect.Height) + ClipRect.Y;

            return NormPoint;
        }

        /// <summary>
        /// 座標変換するぜ
        /// </summary>
        /// <param name="Origin"></param>
        /// <param name="ClipRect"></param>
        /// <param name="DrawRect"></param>
        static PointF ScreenPosition(PointF Origin, RectangleF ClipRect, RectangleF DrawRect)
        {
            PointF norm = Norm(Origin, ClipRect);

            PointF Screen = new PointF();

            Screen.X = (norm.X * DrawRect.Width) + DrawRect.X;
            Screen.Y = ((1f - norm.Y) * DrawRect.Height) + DrawRect.Y;


            return Screen;

        }

        /// <summary>
        /// 指定した位置にラインを描画
        /// </summary>
        /// <param name="e"></param>
        /// <param name="pen"></param>
        /// <param name="Origins">ワールド座標の点</param>
        /// <param name="ClipRect">ワールド座標のクリップ領域. xy:左下. </param>
        /// <param name="DrawRect">描画先のスクリーン座標. xy:左上</param>
        private void DrawLines(PaintEventArgs e, Pen pen, PointF[] Origins, RectangleF ClipRect, RectangleF DrawRect)
        {
            int N = Origins.Length;

            PointF[] ScrPoints = new PointF[N];

            for (int i = 0; i < Origins.Length; i++)
            {
                PointF _ScrPoint = ScreenPosition(Origins[i], ClipRect, DrawRect);

                ScrPoints[i] = _ScrPoint;
            }

            e.Graphics.DrawLines(pen, ScrPoints);

        }

        //==================================================================


    }
}
