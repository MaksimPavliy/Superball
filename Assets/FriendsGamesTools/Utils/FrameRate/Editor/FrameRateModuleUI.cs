namespace FriendsGamesTools
{
    public static class FrameRateModuleUI
    {
        static FrameRateSettings config => SettingsInEditor<FrameRateSettings>.instance;
        public static void Show() {
            var changed = false;

            EditorGUIUtils.Toggle("set default FPS?", ref config.defaultFPSEnabled, ref changed);
            if (config.defaultFPSEnabled) {
                EditorGUIUtils.IntField("default FPS", ref config.defaultFPS, ref changed);
            }

            if (changed)
                config.SetChanged();
        }
    }
}