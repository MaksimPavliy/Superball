namespace FriendsGamesTools.Audio
{
    public class MusicSettingsView : ToggleView
    {
        protected override bool value
#if AUDIO
        { get => MusicManager.on; set => MusicManager.on = value; }
#else
        { get => false; set { } }
#endif
    }
}