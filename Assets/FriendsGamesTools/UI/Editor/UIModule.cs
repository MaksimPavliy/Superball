#if UI
using FriendsGamesTools.UI;
#endif
using UnityEditor;
using UnityEngine;

namespace FriendsGamesTools
{
    public class UIModule : RootModule
    {
        public const string define = "UI";
        public override string Define => define;
        public override HowToModule HowTo() => new UIModule_HowTo();
        public const string DefaultFontName = "Roboto-Regular-FGT SDF";

#if UI
        public static TMPro.TMP_FontAsset defaultFont => new ExampleFontAsset(DefaultFontName).asset;
        protected override void OnCompiledGUI()
        {
            base.OnCompiledGUI();

            if (SettingsInEditor<ScreenSettings>.GetSettingsInstance(false) == null
                && GUILayout.Button("emulate screen safe area"))
                Selection.activeObject = SettingsInEditor<ScreenSettings>.GetSettingsInstance();
        }
#endif
    }
}