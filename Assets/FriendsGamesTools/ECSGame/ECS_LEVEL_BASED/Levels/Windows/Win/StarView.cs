using UnityEngine;

namespace FriendsGamesTools.ECSGame
{
    public class StarView : MonoBehaviour
    {
        public GameObject starParent;
#if ECS_LEVEL_BASED
        public void SetState(bool collected)=> starParent.SetActive(collected);
#endif
    }
}