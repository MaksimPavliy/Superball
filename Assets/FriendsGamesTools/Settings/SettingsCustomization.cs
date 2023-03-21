using System.Collections.Generic;
using FriendsGamesTools.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools
{
    public class SettingsCustomization : WindowCustomization
    {
        public TextMeshProUGUI header;
        public List<TextMeshProUGUI> captions = new List<TextMeshProUGUI>();
        public TextMeshProUGUI info;
        public Image window, cross;
        public List<Image> ons, offs, stars;

        SettingsModuleSettings config => SettingsModuleSettings.instance;
        protected override void Customize()
        {
            SetFont(header, config.headerFont);
            SetFont(info, config.infoFont);
            captions.ForEach(caption => SetFont(caption, config.captionFont));
            SetPic(window, config.windowPic);
            SetPic(cross, config.crossPic);
            ons.ForEach(on => SetPic(on, config.onPic));
            offs.ForEach(off => SetPic(off, config.offPic));
            stars.ForEach(star => SetPic(star, config.starsPic));
        }
    }
}