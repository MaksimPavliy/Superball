using UnityEngine;

namespace FriendsGamesTools
{
    public class UIModule_HowTo : HowToModule
    {
        public override string forWhat => "utilities for creating UI";
        protected override void OnHowToGUI()
        {
            EditorGUIUtils.RichMultilineLabel("<b>Defaults for FriendsGamesIncubator games</b>");
#if UI
            Roboto.ShowOnGUI("Use this as default font");
#endif
            Shadow.ShowOnGUI("Use this as default material for shadow sprites");
            GUILayout.Space(10);
            EditorGUIUtils.RichMultilineLabel("<b>Tween effects</b>");
            TweenColor.ShowOnGUI("Basic color blinking for UI");
            TweenScale.ShowOnGUI("Basic scale pulsing animation for UI");
            ButtonScale.ShowOnGUI("Scaling button up when presses");
            GUILayout.Space(10);
            EditorGUIUtils.RichMultilineLabel("<b>Basic sprite effects</b>");
            AnimatedEffect.ShowOnGUI("Script for making simple animated effects - when you have a list of sprites for it and want to play it once",
                "Set <b>pic</b> where sprite animation will be shown, and <b>sprites</b> for that animation.\n" +
                "Call <b>PlayInstance(pos, parent)</b> from prefab - it will create effect instance, play animation and then destroy instance.");
            FadeText.ShowOnGUI("Script for making text fly up and fade out - like money earned etc.",
                "Give it <b>label</b> to use, <b>duration</b> to live and <b>move</b> direction to fly");
            GUILayout.Space(10);
            EditorGUIUtils.RichMultilineLabel("<b>Screen size and safe zones</b>");
            ScreenSettings.ShowOnGUI("Want to react to mobile SafeZone? Get it tested in editor? Use this script",
                "Use ScreenSettings.<b>margins</b> in code to react to safe zone size\n" +
                "Or use TopShiftForNotch, NotchRect - they react to safe zone automatically\n" + 
                "Set simulated safe zone margins with ScreenSettings.instance.<b>marginsInEditor</b>"
                );
            NotchRect.ShowOnGUI("Want to react to devices having Notch?",
            "Put this script to RectTransform that is a direct child of a canvas. It will fill notch rect on device,\n" +
            "so you can be sure making all interface in safe zone.");
            TopShiftForNotch.ShowOnGUI("This is like NotchRect, but it will just add notch size to its height.");
            TopPanelTemplate.ShowOnGUI("You can use this prefab to make your top panel for the game, reacting properly to notches");
            IHasScreenSizeChangeCallback.ShowOnGUI("If you need MonoBehaviour to react to screen parameters change, use this interface",
                "The only reason to change screen now is for emulating game on different devices for screenshots.");
            GUILayout.Space(10);
            EditorGUIUtils.RichMultilineLabel("<b>Other utils</b>");
            TabView.ShowOnGUI("Tabs in UI", "Set tab buttons and tab parents to use it");
        }
        protected override string docsURL => "https://docs.google.com/document/d/16jFu8EqgAb1wGKL4u7Aqz8x2wIJqaGwZan5my4PGBRY/edit?usp=sharing";
#if UI
        ExampleFontAsset Roboto = new ExampleFontAsset(UIModule.DefaultFontName);
#endif
        ExampleMaterial Shadow = new ExampleMaterial("Shadow");
        ExampleScript TweenColor = new ExampleScript("TweenColor");
        ExampleScript TweenScale = new ExampleScript("TweenScale");
        ExampleScript ButtonScale = new ExampleScript("ButtonScale");
        ExampleScript AnimatedEffect = new ExampleScript("AnimatedEffect");
        ExampleScript FadeText = new ExampleScript("FadeText");
        ExampleScript IHasScreenSizeChangeCallback = new ExampleScript("IHasScreenSizeChangeCallback");
        ExampleScript ScreenSettings = new ExampleScript("ScreenSettings");
        ExampleScript NotchRect = new ExampleScript("NotchRect");
        ExampleScript TopShiftForNotch = new ExampleScript("TopShiftForNotch");
        ExamplePrefab TopPanelTemplate = new ExamplePrefab("TopPanelTemplate");
        ExampleScript TabView = new ExampleScript("TabView");
    }
}


