#if DEBUG_PERFORMANCE
using System.Threading;
using UnityEngine;

namespace FriendsGamesTools.DebugTools
{
    public class TargetFrameRateEditorDebug : MonoBehaviour
    {
        public enum HowToChangeFPS
        {
            ApplicationTargetFrameRate,
            Sleep
        }
        public HowToChangeFPS how;
        public int targetFrameRate = 30;
        void Start()
        {
            if (Application.isEditor && how == HowToChangeFPS.ApplicationTargetFrameRate)
                Application.targetFrameRate = targetFrameRate;
        }
        void Update()
        {
            if (how == HowToChangeFPS.Sleep)
                UpdateSleep();
        }

#region Sleep
        int waitingCount = 0;
        const int inc = 1;
        void UpdateSleep()
        {
#if UNITY_EDITOR
            Application.targetFrameRate = targetFrameRate;
            float existingFrameRate = 1f / Time.deltaTime;
            if (targetFrameRate == -1)
                return;
            if (existingFrameRate > targetFrameRate)
            {
                waitingCount += inc;
            }
            else
            {
                waitingCount -= inc;
                if (waitingCount < 0)
                    waitingCount = 0;
            }
            Thread.Sleep(waitingCount);
            //Debug.Log(existingFrameRate);
#endif
        }
#endregion
    }
}
#endif