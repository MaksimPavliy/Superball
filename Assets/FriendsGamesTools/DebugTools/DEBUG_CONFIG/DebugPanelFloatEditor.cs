using System.Globalization;

namespace FriendsGamesTools.DebugTools
{
    public class DebugPanelFloatEditor : DebugPanelNumberEditor<float>
    {
#if DEBUG_CONFIG
        protected override (bool success, float value) TryParse(string text)
            => (StringUtils.TryParse(text, out float value), value);
#endif
    }
}
