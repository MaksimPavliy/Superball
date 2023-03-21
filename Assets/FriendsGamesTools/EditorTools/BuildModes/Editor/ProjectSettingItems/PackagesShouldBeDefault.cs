using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace FriendsGamesTools.EditorTools.BuildModes
{
    public class PackagesShouldBeDefault : ProjectSettingItem
    {
        List<string> defaultPackages = new List<string> {
            "com.google.external-dependency-manager",
            "com.unity.burst@1.2.3",
            "com.unity.entities@0.1.1-preview",
            "com.unity.ide.vscode",
            "com.unity.textmeshpro",
            "com.unity.modules.animation",
            "com.unity.modules.particlesystem",
            "com.unity.modules.uielements",
            "com.unity.modules.unitywebrequest",
            "com.unity.modules.audio",
            "com.unity.modules.imageconversion",
            "com.unity.modules.imgui",
            "com.unity.modules.jsonserialize",
            "com.unity.modules.physics",
            "com.unity.modules.physics2d",
            "com.unity.modules.screencapture",
            "com.unity.modules.ui",
            "com.unity.modules.uielements",
            "com.unity.modules.umbra",
            "com.unity.modules.unityanalytics",
            "com.unity.modules.unitywebrequest",
            "com.unity.modules.video",
            "com.unity.mobile.notifications"
        };
        List<string> packagesToRemove = new List<string> {
            "com.unity.collab-proxy",
            "com.unity.ide.rider",
            "com.unity.test-framework",
            "com.unity.timeline",
            "com.unity.modules.ai",
            "com.unity.modules.assetbundle",
            "com.unity.modules.cloth",
            "com.unity.modules.director",
            "com.unity.modules.terrain",
            "com.unity.modules.terrainphysics",
            "com.unity.modules.tilemap",
            "com.unity.modules.unitywebrequestassetbundle",
            "com.unity.modules.unitywebrequestaudio",
            "com.unity.modules.unitywebrequesttexture",
            "com.unity.modules.unitywebrequestwww",
            "com.unity.modules.vehicles",
            "com.unity.modules.vr",
            "com.unity.modules.wind",
            "com.unity.modules.xr"
        };
        public override bool ignoreByDefault => true;
        public override bool setupByDefault => false;
        public override string name => "DEFAULT_PACKAGES";
        public override string description => "packages should be default";
        public override void GetReleaseCheckError(StringBuilder sb)
        {
            PackagesManager.Update();
            if (!PackagesManager.googleRegisterExists)
                sb.AppendLine("Google packages resiter does not exist");
            foreach (var package in defaultPackages)
            {
                if (!PackageExists(package))
                    sb.AppendLine($"package {package} should be installed");
            }
            foreach (var package in packagesToRemove)
            {
                if (PackageExists(package))
                    sb.AppendLine($"package {package} should NOT be installed");
            }
        }
        bool PackageExists(string package)
        {
            if (!package.Contains("@"))
                return PackagesManager.IsInProject(package);
            else
            {
                var nameVersion = package.Split('@');
                return PackagesManager.IsInProject(nameVersion[0], nameVersion[1], true);
            }
        }
        public override bool canSetup => true;
        protected override async void Setup()
        {
            if (!PackagesManager.googleRegisterExists)
                PackagesManager.AddGooglePackagesRegister();
            foreach (var package in defaultPackages)
            {
                if (PackageExists(package)) continue;
                Debug.Log($"installing {package}");
                var request = UnityEditor.PackageManager.Client.Add(package);
                await EditorAsync.Until(() => request.IsCompleted);
                Debug.Log($"{package} installed {request.Status}");
            }
            foreach (var package in packagesToRemove)
            {
                if (!PackageExists(package)) continue;
                Debug.Log($"uninstalling {package}");
                var request = UnityEditor.PackageManager.Client.Remove(package);
                await EditorAsync.Until(() => request.IsCompleted);
                Debug.Log($"{package} uninstalled {request.Status}");
            }
        }
    }
}
