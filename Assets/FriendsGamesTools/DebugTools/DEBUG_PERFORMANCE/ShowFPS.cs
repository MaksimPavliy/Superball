using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.DebugTools
{
    public class ShowFPS : MonoBehaviour
    {
        public float updateInterval = 1;
        public float integrationPeriod = 3;
        public TextOrTMPro text;
#if DEBUG_PERFORMANCE
        List<float> dts = new List<float>();
        float updateTime;
        private void Start()
        {
            UpdateFPSVisible();
            if (fpsToggle!=null)
                fpsToggle.onValueChanged.AddListener(val => UpdateFPSVisible());
        }
        void Update()
        {
            dts.Add(Time.unscaledDeltaTime);
            if (updateTime <= Time.realtimeSinceStartup)
            {
                updateTime = Time.realtimeSinceStartup + updateInterval;
                int ind = 0;
                float dt = 0;
                while (ind < dts.Count && dt < integrationPeriod)
                {
                    dt += dts[dts.Count - ind - 1];
                    ind++;
                }
                dts.RemoveRange(0, dts.Count - ind);
                fpsFloat = ind == 0 ? 0 : (ind / dt);
                fps = Mathf.RoundToInt(fpsFloat);
                if (text != null)
                    text.text = $"FPS={fps}";
            }
        }
        public int fps { get; private set; }
        public float fpsFloat { get; private set; }
        [SerializeField] Toggle fpsToggle;
        [SerializeField] GameObject showFPSParent;
        void UpdateFPSVisible()
        {
            if (fpsToggle != null)
                showFPSParent.SetActive(fpsToggle.isOn);
        }
#endif
    }
}