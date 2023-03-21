using System.Text;

namespace FriendsGamesTools.EditorTools.BuildModes
{
    public class NotificationsPackageShouldNotBeOld : ProjectSettingItem
    {
        public override string name => "NOTIFICATIONS_VERSION";
        const string minVersion = "1.0.3";
        const string package = "com.unity.mobile.notifications";
        public override string description => $"notifications package should be newer than {minVersion}";
        public override void GetReleaseCheckError(StringBuilder sb)
        {
            if (!PackagesManager.IsInProject(package, minVersion) && PackagesManager.IsInProject(package))
                sb.AppendLine(description);
        }
    }
}