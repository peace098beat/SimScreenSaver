using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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


        public Form1()
        {
            InitializeComponent();


            // 「埋め込みリソース」から画像を取得
            System.Reflection.Assembly myAssembly =System.Reflection.Assembly.GetExecutingAssembly();
            
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
#if DEBUG
            Debug.WriteLine($"MouseMove {DateTime.Now}");
#else
            this.Close();
#endif
        }

        /// <summary>
        /// タイマー
        /// </summary>
        private void timer1_Tick(object sender, EventArgs e)
        {
            //pictureBox1.Refresh();
        }

        //==================================================================
        //==================================================================

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            int duration = 2; // sec
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


            Debug.WriteLine($"repaint {DateTime.Now}");
        }


    }
}
