using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SimScreenSaver
{

    /// <summary>
    /// CPUマネージャー
    /// </summary>
    public class SingleCPUManager
    {

        //
        // :Usage:
        //
        //  List<SingleCPUManager> SingleCPUList = new List<SingleCPUManager>();
        //  
        //  // Initialize
        //  for (int i = 0; i<SingleCPUManager.GetCPUCount(); i++)
        //  {
        //      SingleCPUList.Add(new SingleCPUManager(i, 200, 4, 30));
        //  }
        //  
        //  // Load Data
        //  int Ncpu = Environment.ProcessorCount;
        //  for (int k = 0; k<Ncpu; k++)
        //  {
        //      SingleCPUManager scpu = SingleCPUList[k];
        //      float[] cpu_values = scpu.GetQueBuffer().ToArray();
        //  }
        //


        PerformanceCounter Counter;
        int BufferLength;

        public int CPUIndex;

        int Tap;

        List<float> QueBuffer;
        private System.Windows.Forms.Timer timer;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="index">CPU番号</param>
        /// <param name="buffer_length">バッファ長さ</param>
        /// <param name="tap">移動平均tap</param>
        public SingleCPUManager(int index, int buffer_length, int tap, int interval_ms)
        {

            // Param
            this.BufferLength = buffer_length;
            this.CPUIndex = index;
            this.Tap = tap;
            if (this.Tap <= 0) this.Tap = 1;

            // Init
            Init();

            // タイマーの間隔(ミリ秒)
            timer = new System.Windows.Forms.Timer();
            timer.Interval = interval_ms;
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Update();
        }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="index"></param>
        /// <param name="buffer_length"></param>
        void Init()
        {
            // CPUカウンタ
            Counter = new PerformanceCounter("Processor", "% Processor Time", CPUIndex.ToString());

            // バッファ
            QueBuffer = new List<float>();

            // バッファの初期化
            for (int i = 0; i < BufferLength; i++)
            {
                QueBuffer.Add(0.0f);
            }
        }

        /// <summary>
        /// アップデート
        /// バッファにデータを追加
        /// </summary>
        void Update()
        {
            float v = Counter.NextValue();

            // 移動平均

            float ave = 0;
            for (int i = 0; i < Tap - 1; i++)
            {
                ave += QueBuffer[(QueBuffer.Count - 1) - i];
            }
            ave += v;
            ave /= Tap;
            v = ave;

            QueBuffer.Add(v);
            QueBuffer.RemoveAt(0);
        }


        /// <summary>
        /// バッファを渡す
        /// </summary>
        /// <returns></returns>
        public List<float> GetQueList()
        {
            return QueBuffer;
        }

        /// <summary>
        /// バッファを返す
        /// </summary>
        public float[] GetQueArray()
        {
            return QueBuffer.ToArray(); 
        }

        /// <summary>
        /// Utilities
        /// </summary>
        static public int GetCPUCount()
        {
            return Environment.ProcessorCount;
        }

    }
}
