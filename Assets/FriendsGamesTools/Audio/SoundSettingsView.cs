namespace FriendsGamesTools.Audio
{
    public class SoundSettingsView : ToggleView
    {
        protected override bool value
#if AUDIO
        { get => SoundManager.on; set => SoundManager.on = value; }
#else
        { get => false; set { } }
#endif
    }
}