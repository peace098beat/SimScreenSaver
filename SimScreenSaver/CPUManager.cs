using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Collections;
using System.Collections.Generic;



namespace SimScreenSaver
{
    static public class CPUManager
    {
        static List<PerformanceCounter> performanceCounteres;
        const int BufferLength = 10;
        static List<List<int>> CounterBufferes;

        /// <summary>
        /// 初期化
        /// </summary>
        static public void Init()
        {
            int Ncpu = Environment.ProcessorCount;

            // もし無ければ初期化
            if (performanceCounteres == null)
            {
                // 配列作成
                performanceCounteres = new List<PerformanceCounter>(Ncpu);

                // 配列にPerformanceCounterを入れる
                for (var index = 0; index < Ncpu; index++)
                {
                    // プロセッサ毎の使用率を計測するPerformanceCounterを作成
                    var pc = new PerformanceCounter("Processor", "% Processor Time", index.ToString());
                    performanceCounteres.Add(pc);
                }

                // バッファの初期化
                CounterBufferes = new List<List<int>>(Ncpu);
                for (int i = 0; i < Ncpu; i++)
                {
                    var buffer = new List<int>(BufferLength);
                    for (int j = 0; j < buffer.Count; j++)
                    {
                        buffer[j] = 0;
                    }
                    CounterBufferes.Add(buffer);
                }

            }

        }

        static private void UpdateNextValues()
        {
            int Ncpu = Environment.ProcessorCount;

            Init();

            for (int i = 0; i < Ncpu; i++)
            {
                var buffer = CounterBufferes[i];
                var value = (int)performanceCounteres[i].NextValue();
                buffer.Add(value); // 後入れ
                if (buffer.Count > BufferLength)
                {
                    buffer.RemoveAt(0); // 先頭出し
                }
                Debug.Assert(buffer.Count <= BufferLength);
            }


        }

        /// <summary>
        /// CPU使用率を取得
        /// </summary>
        /// <returns></returns>


        static public int[] GetMaxValues()
        {
            UpdateNextValues();



            int Ncpu = Environment.ProcessorCount;
            int[] Results = new int[Ncpu];

            for (int i = 0; i < Ncpu; i++)
            {
                var buffer = CounterBufferes[i];
                Results[i] = buffer.Max(); // 最大値を取得
            }

            return Results;
        }

        static public int[] GetPeakToPeakValues()
        {
            UpdateNextValues();



            int Ncpu = Environment.ProcessorCount;
            int[] Results = new int[Ncpu];

            for (int i = 0; i < Ncpu; i++)
            {
                var buffer = CounterBufferes[i];
                Results[i] = buffer.Max() - buffer.Min(); // 最大値を取得
            }

            return Results;
        }

        static public int[] GetAveValues()
        {
            UpdateNextValues();

            int Ncpu = Environment.ProcessorCount;
            int[] Results = new int[Ncpu];

            for (int i = 0; i < Ncpu; i++)
            {
                Results[i] = (int)CounterBufferes[i].Average(); // 最大値を取得
            }

            return Results;
        }
    }



}
