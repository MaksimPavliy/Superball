using UnityEngine;

namespace FriendsGamesTools
{
    public class TouchesModule : RootModule
    {
        public const string define = "TOUCHES";
        public override string Define => define;
        public override HowToModule HowTo() => new TouchesModule_HowTo();

#if TOUCHES
        TouchSettings config => SettingsInEditor<TouchSettings>.instance;
        protected override void OnCompiledGUI()
        {
            base.OnCompiledGUI();
            var changed = false;
            EditorGUIUtils.ShowValid("pics set", picsValid);
            EditorGUIUtils.ObjectField("touch sprite normal", ref config.picNormal, ref changed);
            EditorGUIUtils.ObjectField("touch sprite pressed", ref config.picPressed, ref changed);
            if (!picsValid)
                EditorGUIUtils.Toggle("its ok when no pics, module is used only for tracking touches in code", ref config.okIfNoPics, ref changed, labelWidth: 400);
            if (changed)
                config.SetChanged();
        }
        bool picsValid => config.picNormal != null && config.picPressed != null;
        public override string DoReleaseChecks()
        {
            if (!picsValid && !config.okIfNoPics)
                return $"pics for touches not set";
            else
                return null;
        }
        protected override string debugViewPath => "Touches/TouchesDebugView";
#endif
    }
}