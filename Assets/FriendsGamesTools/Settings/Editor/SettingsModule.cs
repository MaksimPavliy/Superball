using System.Collections.Generic;
using System.Text;
using FriendsGamesTools.UI;

namespace FriendsGamesTools
{
    public class SettingsModule : RootModule
    {
        public const string define = "SETTINGS";
        public override string Define => define;
        public override HowToModule HowTo() => new SettingsModule_HowTo();
        public override List<string> dependFromModules => base.dependFromModules.Adding(WindowsModule.define);

#if SETTINGS
        protected override void OnCompiledEnable()
        {
            base.OnCompiledEnable();
            SettingsInEditor<SettingsModuleSettings>.EnsureExists();
        }
        SettingsModuleSettings config => SettingsInEditor<SettingsModuleSettings>.instance;
        protected override void OnCompiledGUI()
        {
            base.OnCompiledGUI();
            var changed = false;
            config.window.ShowWindow<SettingsWindow, SettingsWindowPrefabSettings>(ref changed, true);
            OnWindowCustomizationGUI(ref changed);
            ShowErrors();
            if (changed)
                config.SetChanged();
        }

        StringBuilder sb = new StringBuilder();
        void ShowErrors()
        {
            var errors = DoReleaseChecks();
            if (!string.IsNullOrEmpty(errors))
                EditorGUIUtils.Error(errors);
        }
        public override string DoReleaseChecks() {
            sb.Clear();
           // WindowValid(sb);
            return sb.ToString();
        }
        //bool WindowValid(StringBuilder sb = null)
        //{
        //    var valid = true;
        //    if (!WindowEditorUtils.WindowValid(config.window, sb))
        //        valid = false;
        //    return valid;
        //}
        bool customizeWindowShown;
        private void OnWindowCustomizationGUI(ref bool changed)
        {
            if (!config.window.IsDefaultWindow())
                return;

            EditorGUIUtils.ShowOpenClose(ref customizeWindowShown, "Customize settings window");
            if (!customizeWindowShown)
                return;
            EditorGUIUtils.FontField("header font", ref config.headerFont, ref changed);
            EditorGUIUtils.FontField("caption font", ref config.captionFont, ref changed);
            EditorGUIUtils.FontField("info font", ref config.infoFont, ref changed);
            EditorGUIUtils.SpriteField("window pic", ref config.windowPic, ref changed);
            EditorGUIUtils.SpriteField("cross pic", ref config.crossPic, ref changed);
            EditorGUIUtils.SpriteField("ON pic", ref config.onPic, ref changed);
            EditorGUIUtils.SpriteField("OFF pic", ref config.offPic, ref changed);
            EditorGUIUtils.SpriteField("stars pic", ref config.starsPic, ref changed);
        }
#endif
    }
}