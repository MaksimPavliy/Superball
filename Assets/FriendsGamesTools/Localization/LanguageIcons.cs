#if LOCALIZATION
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace FriendsGamesTools
{
    public static class LanguageIcons
    {
        static Dictionary<Language, Sprite> icons;
        public static Sprite Get(Language lang)
        {
            if (icons == null) icons = new Dictionary<Language, Sprite>();
            if (!icons.TryGetValue(lang, out var sprite))
            {
                var flagName = lang.IsChinese() ? "CH" : lang.ToString();
                // Flags got from https://www.flaticon.com/packs/countrys-flags and then scaled down to 128x128.
                sprite = Resources.Load<Sprite>($"FlagIcons/{flagName}");
                icons.Add(lang, sprite);
            }
            return sprite;
        }
    }
}
#endif