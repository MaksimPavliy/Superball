using UnityEngine;

namespace FriendsGamesTools
{
    public class HapticModule : RootModule
    {
        public const string define = "HAPTIC";
        public override string Define => define;
        public override HowToModule HowTo() => new HapticModule_HowTo();

#if HAPTIC
        static HapticSettings settings => SettingsInEditor<HapticSettings>.GetSettingsInstance();
        protected override void OnCompiledGUI()
        {
            base.OnCompiledGUI();
            bool changed = false;
            EditorGUIUtils.Toggle("availableInEditor", ref settings.availableInEditor, ref changed);
            EditorGUIUtils.Toggle("log", ref settings.log, ref changed);
            EditorGUIUtils.Toolbar("default vibration power", ref settings.defaultType, ref changed);
            if (changed) EditorUtils.SetDirty(settings);
        }
#endif
    }
}