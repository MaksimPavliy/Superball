using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.UI
{
    public abstract class WindowCustomization : MonoBehaviour
    {
        protected virtual void Awake() => Customize();
        protected abstract void Customize();
        protected void SetFont(TextMeshProUGUI text, TMP_FontAsset font)
        {
            if (text != null && font != null)
                text.font = font;
        }
        protected void SetPic(Image img, Sprite pic)
        {
            if (pic != null && img != null)
                img.sprite = pic;
        }
    }
}
