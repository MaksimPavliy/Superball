using System;
using FriendsGamesTools.DebugTools;
using TMPro;
using UnityEngine;

namespace FriendsGamesTools
{
    public class InternetDebugView : DebugPanelItemView
    {
        public override (string tab, string name) whereToShow => (FGTModuleDebugPanel.CommonTab, "internet");

        [SerializeField] TMP_InputField input;
        protected override void OnEnablePlaying()
        {
            base.OnEnablePlaying();
            input.text = FGTSettings.instance.simulatedInternetDelay.ToString();
            input.onValueChanged.AddListener(e => OnChanged());
        }

        private void OnChanged()
        {
            if (!float.TryParse(input.text, out var value))
                FGTSettings.instance.simulatedInternetDelay = value;
        }
    }
}