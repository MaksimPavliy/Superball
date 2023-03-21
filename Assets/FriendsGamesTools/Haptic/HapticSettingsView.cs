namespace FriendsGamesTools
{
    public class HapticSettingsView : ToggleView
    {
        protected override bool value
#if HAPTIC
        { get => Haptic.on; set => Haptic.on = value; }
#else
        { get => false; set { } }
#endif
    }
}
