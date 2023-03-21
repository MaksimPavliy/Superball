using UnityEditor;
using UnityEngine;

namespace FriendsGamesTools
{
    [InitializeOnLoad]
    public class FriendsGamesManagerEditor
    {
        public static bool inited { get; private set; }
        static FriendsGamesManagerEditor()
        {
            if (!EditorApplication.isUpdating)
                InitializeOnLoadAndAssetsUpdated();
            else
                InitAfterAssetsUpdated();
        }
        static async void InitAfterAssetsUpdated()
        {
            await Awaiters.While(() => EditorApplication.isUpdating);
            InitializeOnLoadAndAssetsUpdated();
        }
        static void InitializeOnLoadAndAssetsUpdated()
        {
            DefinesModifier.InitOnLoad();
            CompilationCallback.InitOnLoad();
            PrefabUtils.InitOnLoad();
            BuildInfoManagerEditor.InitOnLoad();
#if EXAMPLES
            ExamplesDefines.InitOnLoad();
#endif
            EditorTools.BuildModes.ProjectSettingItemManager.InitOnLoad();
            ScriptOrderSetter.InitOnLoad(); 
            EditorTools.BuildModes.SwitchBuildModeToReleaseOnReleaseBranch.InitOnLoad();
            FriendsGamesToolsWindow.instance?.InitIfNeeded();
#if ADS
            Ads.AdsModule.UpdateAutoSelectedAdsManager();
#endif
            inited = true;
        } 
    }
}
