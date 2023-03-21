#if MAX_SDK
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
using System.IO;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

namespace FriendsGamesTools.Integrations.MaxSDK
{
    public class BuildPostProcessorForMAXMediations : MonoBehaviour
    {
        [PostProcessBuild]
        public static void DoPostProcess(BuildTarget buildTarget, string path)
        {
#if UNITY_IOS
            if (buildTarget == BuildTarget.iOS)
            {
                var max = MaxSDKSettings.instance;

                // Disable App Transport Security. UnityAds and Adcolony needs that
                if ((
                    max.enabledMediations.Contains(Mediations.UNITY_NETWORK)||
                    max.enabledMediations.Contains(Mediations.ADCOLONY_NETWORK))
                    && max.ios.enabled)
                {
                    // ChangeXcodePlist, add allowing http.
                    string plistPath = path + "/Info.plist";
                    PlistDocument plist = new PlistDocument();
                    plist.ReadFromFile(plistPath);
                    PlistElementDict rootDict = plist.root;

                    // Add actual exception keys for allowing http.
                    if (rootDict["NSAppTransportSecurity"] == null)
                        rootDict.CreateDict("NSAppTransportSecurity");
                    rootDict["NSAppTransportSecurity"].AsDict().SetBoolean("NSAllowsArbitraryLoads", true);

                    //var exceptionDomains = rootDict["NSAppTransportSecurity"].AsDict().CreateDict("NSExceptionDomains");
                    //var domain = exceptionDomains.CreateDict("YOURDOMAIN.com");

                    //domain.SetBoolean("NSExceptionAllowsInsecureHTTPLoads", true);
                    //domain.SetBoolean("NSIncludesSubdomains", true);

                    File.WriteAllText(plistPath, plist.WriteToString());
                }
            }
#endif
        }
    }
}
#endif