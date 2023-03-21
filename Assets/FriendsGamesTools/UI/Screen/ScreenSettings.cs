using System;
using System.Collections.Generic;
using UnityEngine;

namespace FriendsGamesTools.UI
{
    public class ScreenSettings : SettingsScriptable<ScreenSettings>
    {
        public ScreenMargins marginsInEditor;

#if UI
        public static ScreenMargins margins
        {
            get
            {
                ScreenMargins margins;
                if (!Application.isEditor || instance == null)
                    margins = new ScreenMargins
                    {
                        bottomMargin = Mathf.RoundToInt(Screen.safeArea.yMin),
                        leftMargin = Mathf.RoundToInt(Screen.safeArea.xMin),
                        topMargin = Mathf.RoundToInt(Screen.height - Screen.safeArea.yMax),
                        rightMargin = Mathf.RoundToInt(Screen.width - Screen.safeArea.xMax)
                    };
                else
                    margins = instance.marginsInEditor;
                ApplyHardcodedMargins(ref margins);
                return margins;
            }
        }
        public static int notchHeight => margins.topMargin;
        static Dictionary<string, int> hardcodedNotchSize = new Dictionary<string, int> {
            // https://www.theiphonewiki.com/wiki/Models
            { "HUAWEI EML-L29", 100 },
            { "iPhone10,3", 90 }, // iPhone X  (A1865, A1902)
            { "iPhone10,6", 90 }, // iPhone X  (A1901)
            { "iPhone11,8", 90 }, // iPhone XR
            { "iPhone11,2", 90 }, // iPhone XS
            { "iPhone11,6", 90 }, // iPhone XS Max
            { "iPhone12,1", 90 }, // iPhone 11
            { "iPhone12,5", 90 }, // iPhone 11 Pro Max
            { "iPhone12,3", 90 }, // iPhone 11 Pro
            { "iPhone12,8", 90 }  // iPhone SE (2nd generation)
        };
        static void ApplyHardcodedMargins(ref ScreenMargins margins)
        {
            if (hardcodedNotchSize.TryGetValue(SystemInfo.deviceModel, out var notch))
                margins.topMargin = notch;
        }
#endif
    }
    [Serializable]
    public struct ScreenMargins
    {
        public int topMargin, bottomMargin, leftMargin, rightMargin;
    }
}