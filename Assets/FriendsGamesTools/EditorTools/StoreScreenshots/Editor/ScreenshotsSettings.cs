#if SCREENSHOTS
using FriendsGamesTools.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FriendsGamesTools.EditorTools.Screenshots
{
    public class ScreenshotsSettings : SettingsScriptable<ScreenshotsSettings>
    {
        protected override string SubFolder => "EditorTools";
        protected override bool inResources => false;
        public float autoTakeScreenshotDelay = 10;
        public bool autoScreenshotsEnabled;
        public TargetPlatform selected = TargetPlatform.IOS;
        [NonSerialized]
        public List<ScreenDesc> iosScreens = new List<ScreenDesc>
        {
            new ScreenDesc { name = "6.5-inch iPhones", desc = "highest resolution - iPhone 12 Pro MAX ", resolution = new Vector2Int { x = 1284, y = 2778 }, margins = new ScreenMargins { bottomMargin = 120, leftMargin = 120, rightMargin = 120, topMargin = 120 } },
            new ScreenDesc { name = "5.5-inch iPhones (rendered)", desc = "6S+, 7+, 8+ rendered resolution, not screen", resolution = new Vector2Int { x= 1242, y = 2208 }, margins = new ScreenMargins { } },
            new ScreenDesc { name = "12.9-inch iPad (2-gen)", desc = "iPad Pro", resolution = new Vector2Int { x = 2048, y = 2732 }, margins = new ScreenMargins { } }
        };
        [NonSerialized]
        public List<ScreenDesc> androidScreens = new List<ScreenDesc>
        {
            new ScreenDesc { name = "common android screen", desc = "", resolution = new Vector2Int { x = 1080, y = 1920 }, margins = new ScreenMargins { } }
        };
        public List<ScreenDesc> screens => selected == TargetPlatform.IOS ? iosScreens : androidScreens;
    }
}
#endif