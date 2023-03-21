using System.Globalization;

namespace FriendsGamesTools.DebugTools
{
    public class DebugPanelDoubleEditor : DebugPanelNumberEditor<double>
    {
#if DEBUG_CONFIG
        protected override (bool success, double value) TryParse(string text)
            => (StringUtils.TryParse(text, out double value), value);
#endif
    }
}
