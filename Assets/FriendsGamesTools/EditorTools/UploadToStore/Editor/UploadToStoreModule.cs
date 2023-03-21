using FriendsGamesTools.ModulesUpdates;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace FriendsGamesTools.EditorTools.Upload
{
    public class UploadToStoreModule : ModuleManager
    {
        public const string define = "STORE_UPLOAD";
        public override string Define => define;
        public override string parentModule => EditorToolsModule.define;
        public override HowToModule HowTo() => new UploadToStoreModule_HowTo();
        public static UploadToStoreSettings settings => SettingsInEditor<UploadToStoreSettings>.instance;

#if STORE_UPLOAD
#endif
    }
}