using System;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.Share
{
    [RequireComponent(typeof(Button))]
    public class ShareButton : MonoBehaviour {
        [SerializeField] string title, text;
        [SerializeField] bool addGameLink = true;
#if SHARE
        private void Awake()
        {
            var button = GetComponent<Button>();
            button.onClick.AddListener(OnSharePressed);
        }
        public void OnSharePressed()
        {
            var currText = text;
            if (addGameLink)
                currText += "\n" + ShareSettings.instance.gameLink;
            ShareManager.Share(currText, title);
        }
#endif
    }
}
