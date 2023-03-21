using UnityEngine;

namespace FriendsGamesTools.DebugTools
{
    public class DebugSettings : SettingsScriptable<DebugSettings>
    {
        protected override bool inRepository => false;
        [SerializeField] float _timeSpeed = 1;
        public float timeSpeed
        {
            get => _timeSpeed;
            set
            {
                if (_timeSpeed == value) return;
                _timeSpeed = value;
                GameTime.UpdateUnityTimeSpeed();
            }
        }
    }
}
