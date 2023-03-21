#if TUTORIAL
using System.Threading.Tasks;
using UnityEngine.EventSystems;

namespace FriendsGamesTools.ECSGame.Tutorial
{
    public class TutorialButtonReference : TutorialButtonAbstarct
    {
        public TutorialButtonAbstarct tutorialButton;
        public override void OnPointerClick(PointerEventData eventData) => tutorialButton.OnPointerClick(eventData);
        public override async Task PressingButton(string text) => await tutorialButton.PressingButton(text);
    }
}
#endif