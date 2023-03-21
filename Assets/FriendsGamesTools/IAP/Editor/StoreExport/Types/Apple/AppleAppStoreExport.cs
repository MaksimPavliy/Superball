#if IAP
using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FriendsGamesTools.IAP
{
    public class AppleAppStoreExport : AppStoreExport
    {
        new AppleAppStoreCredentials credentials => base.credentials.appleAppStore;
        public static AppleAppStoreExport instance { get; private set; } = new AppleAppStoreExport();
        public override string storeName => "Apple App Store";
        protected override void OnCustomGUI(ref bool changed)
        {
            var currChanged = changed;
            EditorGUIUtils.InHorizontal(() =>
            {
                EditorGUIUtils.ShowValid(AppleSKUValid());
                EditorGUIUtils.TextField("Apple SKU", ref config.appleSKU, ref currChanged, width:200, labelWidth: 70);
                FGTSettingsUtils.AppleAppIdInput();
                config.appleTeamID = ValidateAppleTeamId(config.appleTeamID);
                EditorGUIUtils.ShowValid(AppleTeamIdValid());
                EditorGUIUtils.TextField("Apple Team Id", ref config.appleTeamID, ref currChanged, width: 200, labelWidth: 90);
            });
            if (!AppleTeamIdEmpty() && config.appleTeamID != FriendsGamesConstants.AppleTeamId)
            {
                if (!config.customTeamId)
                    EditorGUIUtils.Error(teamIdNotFGT);
                EditorGUIUtils.Toggle("Allow custom team id", ref config.customTeamId, ref currChanged);
            }
            EditorGUIUtils.InHorizontal(() =>
            {
                EditorGUIUtils.ShowValid(AppleStoreLoginValid());
                EditorGUIUtils.TextField("Apple dev account login email", ref credentials.appleAppStoreLoginEmail, ref currChanged);
                EditorGUIUtils.ShowValid(AppleStorePassValid());
                EditorGUIUtils.TextField("Apple application password", ref credentials.appleAppStoreAppPassword, ref currChanged);
            });
            EditorGUIUtils.InHorizontal(() =>
            {
                EditorGUIUtils.ShowValid(UploaderPathValid());
                EditorGUIUtils.TextField("iTMSTransporter path", ref credentials.transporterPath, ref currChanged);
            });
            if (linesCount > 0)
                GUILayout.Label($"lines received = {linesCount}");
            changed = currChanged;
        }
        bool AppleTeamIdEmpty() => string.IsNullOrEmpty(config.appleTeamID);
        bool AppleTeamIdValid(StringBuilder sb = null)
        {
            if (AppleTeamIdEmpty())
            {
                sb?.AppendLine("IAP Catalog appleTeamID not set");
                return false;
            }
            if (!config.customTeamId && config.appleTeamID != FriendsGamesConstants.AppleTeamId)
            {
                sb?.AppendLine(teamIdNotFGT);
                return false;
            }
            return true;
        }
        const string teamIdNotFGT = "IAP Catalog appleTeamID does not match FriensGames team id";
        string ValidateAppleTeamId(string teamId) => AppleTeamIdEmpty() ? FriendsGamesConstants.AppleTeamId : teamId;
        bool AppleSKUValid(StringBuilder sb = null)
        {
            var valid = !string.IsNullOrEmpty(config.appleSKU);
            if (!valid)
                sb?.AppendLine("apple SKU not set");
            return valid;
        }
        bool AppleStoreLoginValid(StringBuilder sb = null)
        {
            var valid = !string.IsNullOrEmpty(credentials.appleAppStoreLoginEmail);
            if (!valid)
                sb?.AppendLine("apple app store login email not set");
            return valid;
        }
        bool AppleStorePassValid(StringBuilder sb = null)
        {
            var valid = !string.IsNullOrEmpty(credentials.appleAppStoreAppPassword);
            if (!valid)
                sb?.AppendLine("apple app store application password not set");
            return valid;
        }
        bool UploaderPathValid(StringBuilder sb = null)
        {
            var valid = !string.IsNullOrEmpty(credentials.transporterPath);
            if (!valid)
                sb?.AppendLine("iTMSTransporter path not set");
            return valid;
        }
        public override bool ExportValid(StringBuilder sb = null)
        {
            var valid1 = AppleTeamIdValid(sb);
            var valid2 = AppleSKUValid(sb);
            var valid3 = FGTSettingsUtils.AppleAppIdValid(sb);
            var valid4 = AppleStoreLoginValid(sb);
            var valid5 = AppleStorePassValid(sb);
            var valid6 = UploaderPathValid(sb);
            return valid1 && valid2 && valid3 && valid4 && valid5 && valid6;
        }

        AppleAppStoreUploaderManager uploader;
        int linesCount => uploader?.linesReceived ?? 0;
        protected override async Task<bool> Exporting()
            => await (uploader = new AppleAppStoreUploaderManager()).Upload();
    }
    [Serializable]
    public class AppleAppStoreCredentials
    {
        // If no rights to execute iTMSTransporter.cmd:
        //  sudo chown %your mac account name% iTMSTransporter.cmd
        //  sudo chmod 777 iTMSTransporter.cmd

        // xcrun altool --list-providers -u

        public string transporterPath = Application.platform == RuntimePlatform.WindowsEditor ?
            "C:/Program Files (x86)/itms/iTMSTransporter.cmd" : "/usr/local/itms/bin/iTMSTransporter";
        public string appleAppStoreLoginEmail = "";
        public string appleAppStoreAppPassword = ""; //"xxxx-xxxx-xxxx-xxxx";
    }

    //public class AppleIAPUploader
    //{
    //    IAPSettings config => SettingsInEditor<IAPSettings>.instance;
    //    ProcessLauncher manager = new ProcessLauncher();
    //    string uploadedFolderName => config.appleSKU;
    //    string metaDataFileName => "metadata.xml"; // Have to be UTF-8.

    //    // Downloaded report can refresh only once per day.
    //    // Cant do it in runtime. https://developer.apple.com/documentation/storekit/in-app_purchase/loading_in-app_product_identifiers

    //}
}
#endif