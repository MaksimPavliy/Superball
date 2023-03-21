using FriendsGamesTools.DebugTools;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.ECSGame
{
    public class GameRootDebugView : ECSModuleDebugPanel
    {
        public override string tab => "ECS";
        public override string module => "ECS_GAMEROOT";
        [SerializeField] Button resetButton, emailSaveButton;

#if ECS_GAMEROOT
        protected override void AwakePlaying()
        {
            base.AwakePlaying();
            resetButton.onClick.AddListener(OnResetPressed);
            emailSaveButton.onClick.AddListener(OnEmailSavePressed);
        }
        protected override void OnEnablePlaying()
        {
            resetButton.interactable = true;
#if TUTORIAL
            resetButton.interactable = !(Tutorial.TutorialManager.instance?.chapterShown??false);
#endif
        }
        void OnResetPressed()
        {
#if WINDOWS
            UI.Windows.CloseAll();
#endif
            GameRoot.instance.ResetWorld();
        }
        void OnEmailSavePressed() => Serialization.SendSaveToDeveloper("from debug panel");
#endif
    }
}
