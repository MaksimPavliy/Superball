using UnityEngine;

namespace FriendsGamesTools.Audio
{
    public class HighFPSSettingsView : ToggleView
    {
        protected override bool value
        {
            get => Application.targetFrameRate == FrameRate.HighFPS;
            set => Application.targetFrameRate = value ? FrameRate.HighFPS : FrameRate.LowFPS;
        }
    }
}