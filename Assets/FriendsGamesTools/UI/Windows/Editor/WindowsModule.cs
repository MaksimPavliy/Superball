using System;
using System.Collections.Generic;
#if WINDOWS
using FriendsGamesTools.UI;
#endif

namespace FriendsGamesTools
{
    public class WindowsModule : ModuleManager
    {
        public const string define = "WINDOWS";
        public override string Define => define;
        public override string parentModule => UIModule.define;
        public override HowToModule HowTo() => new WindowsModule_HowTo();
        public override List<string> dependFromModules => base.dependFromModules.Adding(UIModule.define);

#if WINDOWS
        protected override void OnCompiledEnable()
        {
            base.OnCompiledEnable();
            SettingsInEditor<MessageWindowSettings>.EnsureExists();
        }
        protected override void OnCompiledGUI()
        {
            OnMessageWindowGUI();
        }

        bool customizeMessageWindowShown;
        MessageWindowSettings messageWindowSettings => SettingsInEditor<MessageWindowSettings>.instance;
        private void OnMessageWindowGUI()
        {
            EditorGUIUtils.ShowOpenClose(ref customizeMessageWindowShown, "Customize message window");
            if (!customizeMessageWindowShown)
                return;
            var changed = false;
            EditorGUIUtils.FontField("header font", ref messageWindowSettings.headerFont, ref changed);
            EditorGUIUtils.FontField("message font", ref messageWindowSettings.messageFont, ref changed);
            EditorGUIUtils.FontField("ok font", ref messageWindowSettings.okFont, ref changed);
            EditorGUIUtils.SpriteField("window pic", ref messageWindowSettings.windowPic, ref changed);
            EditorGUIUtils.SpriteField("button pic", ref messageWindowSettings.buttonPic, ref changed);
            if (changed)
                messageWindowSettings.SetChanged();
        }
#endif
    }
}