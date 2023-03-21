#if TUTORIAL || QUESTS
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FriendsGamesTools.ECSGame.Tutorial
{
    public abstract class TutorialButtonAbstarct : MonoBehaviour, IPointerClickHandler
    {
        public abstract void OnPointerClick(PointerEventData eventData);
        public abstract Task PressingButton(string text);
    }
}
#endif