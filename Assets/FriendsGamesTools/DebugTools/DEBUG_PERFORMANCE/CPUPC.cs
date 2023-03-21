#if DEBUG_PERFORMANCE
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace FriendsGamesTools.DebugTools
{
    public static class CPUPC
    {
        static TimeSpan PrevCPUPc = new TimeSpan(), CurrCPUPc;
        static float prevTime;
        public static int ReadCPUPC()
        {
            int ToReturn = 0;
            var currTime = Time.realtimeSinceStartup;
            PrevCPUPc = CurrCPUPc;
            CurrCPUPc = new TimeSpan(0);
            Process[] AllProcesses = Process.GetProcesses();
            for (int Cnt = 0; Cnt < AllProcesses.Length; Cnt++)
                CurrCPUPc += AllProcesses[Cnt].TotalProcessorTime;

            TimeSpan newCPUTime = CurrCPUPc - PrevCPUPc;
            var StatsRefresh = currTime - prevTime;
            ToReturn = (int)((100 * newCPUTime.TotalSeconds / StatsRefresh) / Environment.ProcessorCount);
            prevTime = currTime;
            return ToReturn;
        }
    }

    public static class CPUAndroid {
        static int CurrIdle, PrevIdle, CurrCPU, PrevCPU;
        static List<int> Last5CPUs = new List<int>();
        public static int ReadCPUAndroid()
        {
            try
            {
                string Line;

                // Read the file and display it line by line.            
                using (System.IO.StreamReader file = new System.IO.StreamReader("/proc/stat"))
                {
                    while ((Line = file.ReadLine()) != null)
                    {
                        if (Line.Contains("cpu"))
                        {
                            string StrToUse = Line.Remove(0, 4).Trim();
                            String[] Toks = System.Text.RegularExpressions.Regex.Split(StrToUse, @"\s+");

                            PrevIdle = CurrIdle;
                            PrevCPU = CurrCPU;
                            CurrIdle = Int32.Parse((Toks[3]));
                            CurrCPU = Int32.Parse((Toks[0])) + Int32.Parse((Toks[1])) + Int32.Parse((Toks[2]))
                                  + Int32.Parse((Toks[4])) + Int32.Parse((Toks[5])) + Int32.Parse((Toks[6]));
                            int IdleToUse = CurrIdle - PrevIdle;
                            int CPUToUse = CurrCPU - PrevCPU;

                            if (Last5CPUs.Count > 0)
                                Last5CPUs.RemoveAt(0);
                            Last5CPUs.Add((int)(100 * (CPUToUse) / (CPUToUse + IdleToUse)));
                            float TotCPU = 0;
                            for (int Cnt = 0; Cnt < Last5CPUs.Count; Cnt++)
                                TotCPU += Last5CPUs[Cnt];

                            return (int)(TotCPU / Last5CPUs.Count);
                        }
                    }
                }
            }
            catch //(Exception e)
            {
                //GlobalUtilities.MY_PRINTF_EXCEPTIONS(44, e);
#if UNITY_EDITOR
                //throw e;
#endif
            }

            return -1;
        }
    }
}
#endif