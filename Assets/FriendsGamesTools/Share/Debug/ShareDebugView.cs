using FriendsGamesTools.DebugTools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.Share
{
    public class ShareDebugView : FGTModuleDebugPanel
    {
        public override string module => "SHARE";
        public override string tab => CommonTab;

        [SerializeField] TMP_InputField sharedTextInput;
        [SerializeField] TMP_InputField sharedTitleInput;
        [SerializeField] Button shareButton;
#if SHARE
        protected override void AwakePlaying()
        {
            base.AwakePlaying();
            shareButton.onClick.AddListener(() => ShareManager.Share(sharedTextInput.text, sharedTitleInput.text,
                success => Debug.Log($"'share' called from debug panel, result = {success}")));
        }
#endif
    }
}