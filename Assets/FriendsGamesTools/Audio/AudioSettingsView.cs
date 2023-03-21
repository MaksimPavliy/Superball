namespace FriendsGamesTools.Audio
{
    public class AudioSettingsView : ToggleView
    {
        protected override bool value
#if AUDIO
        {
            get => SoundManager.on;
            set
            {
                SoundManager.on = value;
                MusicManager.on = value;
            }
        }
#else
        { get => false; set { } }
#endif
    }
}