using UnityEngine;
using UnityEngine.UI;

namespace HcUtils
{
    public class ImageThemeSetter : ThemeSetter
    {
        [SerializeField] private Image image;

        [SerializeField] public Sprite[] sprites;
        protected override void Init()
        {
            if (!image)
            {
                image = GetComponent<Image>();
            }

            base.Init();
        }
        protected override void SetTheme(int ind)
        {
            if (!image) return;

            image.sprite = sprites[ind];
        }
    }
}