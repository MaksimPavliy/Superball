using System.Text;

namespace FriendsGamesTools.EditorTools.BuildModes
{
    public class EnableAnyPlatform : ProjectSettingItem
    {
        public override string name => "ENABLE_ANY_PLATFORM";
        public override string description => "You have to enable at least one platform - android, ios";
        public override void GetReleaseCheckError(StringBuilder sb)
        {
            if (!settings.AndroidEnabled && !settings.IOSEnabled)
                sb.AppendLine($"What platform are you building for?\n{description}");
        }
    }
}
