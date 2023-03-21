
namespace FriendsGamesTools.DebugTools
{
    public class DebugPanelIntEditor : DebugPanelNumberEditor<int>
    {
#if DEBUG_CONFIG
        protected override (bool success, int value) TryParse(string text)
            => (int.TryParse(text, out var val), val);
#endif
    }
}
