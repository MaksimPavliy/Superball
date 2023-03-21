using FriendsGamesTools.Integrations;
using System.Linq;
using System.Text;
using UnityEditor;

namespace FriendsGamesTools.EditorTools.BuildModes
{
    public class ApplicationIdShouldBeValid : ProjectSettingItem
    {
        public override string name => "VALIDATE_APP_ID";
        public override string description => "Check application id";
        public override void GetReleaseCheckError(StringBuilder sb)
        {
            if (manifestPackageIdError)
                sb.AppendLine($"android package id not set in main AndroidManifest");
            if (androidIdError)
                sb.AppendLine($"android package id not set properly: {ApplicationIdValidator.idTip}");
            if (iosIdError)
                sb.AppendLine($"ios bundle id not set properly: {ApplicationIdValidator.idTip}");
            if (idsDifferentError)
                sb.AppendLine($"{ApplicationIdValidator.idsDifferentTip}");
        }
        public override bool canSetup => (androidIdError || iosIdError || idsDifferentError) ? string.IsNullOrEmpty(FindIdName()) : true;
        public static string androidId => PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android);
        public static bool androidIdError => settings.AndroidEnabled && !ApplicationIdValidator.IdValid(androidId);
        public static string iosId => PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.iOS);
        public static bool iosIdError => settings.IOSEnabled && !ApplicationIdValidator.IdValid(iosId);
        public static bool idsDifferentError => settings.IOSEnabled && settings.AndroidEnabled && iosId != androidId;
        public static bool anyError => androidIdError || iosIdError || idsDifferentError;
        public static bool allOk => !anyError;
        public static string GetStringAfterLastDot(string id)
        {
            if (string.IsNullOrEmpty(id))
                return string.Empty;
            var parts = id.Split('.');
            return parts.Length > 0 ? parts.Last() : string.Empty;
        }
        string FindIdName()
        {
            var name = string.Empty;
            TryMakeIdValid(androidId);
            TryMakeIdValid(iosId);
            return name;
            void TryMakeIdValid(string id)
            {
                var lastPart = TryMakeNameValid(GetStringAfterLastDot(id));
                if (!string.IsNullOrEmpty(lastPart))
                    name = lastPart;
            }
            string TryMakeNameValid(string currName)
            {
                currName = StringUtils.RemoveDigits(currName);
                currName = currName.Replace(" ", "");
                return currName;
            }
        }
        protected override void Setup()
        {
            var name = FindIdName();
            var appId = FriendsGamesConstants.ApplicationIdPrefix + name;
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, appId);
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, appId);
            if (manifestPackageIdError)
                SetupPackageIdToManifest();
        }

        AndroidManifestManager manifest;
        bool manifestPackageIdError => settings.AndroidEnabled && !manifestPackageIdSet;
        bool manifestPackageIdSet
        {
            get
            {
                if (manifest == null)
                    manifest = new AndroidManifestManager();
                var packageIdFromManifest = manifest.GetPackageName();
                return packageIdFromManifest == androidId;
            }
        }
        void SetupPackageIdToManifest()
        {
            if (manifest == null)
                manifest = new AndroidManifestManager();
            manifest.SetPackageName(androidId);
            manifest.Save();
            manifest = new AndroidManifestManager();
        }
    }
}