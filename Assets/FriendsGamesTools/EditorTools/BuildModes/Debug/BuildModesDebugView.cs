using FriendsGamesTools.EditorTools.BuildModes;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.DebugTools
{
    public class BuildModesDebugView : FGTModuleDebugPanel
    {
        public override string tab => CommonTab;
        public override string module => "BUILD_MODES";
        [SerializeField] Button developButton, testButton, releaseButton;
        [SerializeField] GameObject developParent, testParent, releaseParent;
        protected override void OnEnablePlaying() {
            developButton.onClick.AddListener(() => SetMode(BuildModeType.Develop));
            testButton.onClick.AddListener(() => SetMode(BuildModeType.Test));
            releaseButton.onClick.AddListener(() => SetMode(BuildModeType.Release));
            UpdateView();
        }
        void UpdateView() {
            developParent.SetActive(BuildMode.develop);
            testParent.SetActive(BuildMode.test);
            releaseParent.SetActive(BuildMode.release);
        }
        void SetMode(BuildModeType mode) {
            BuildModeSettings.instance._mode = mode;
            UpdateView();
        }
    }
}
