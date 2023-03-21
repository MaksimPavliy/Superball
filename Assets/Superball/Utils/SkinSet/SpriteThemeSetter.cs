using UnityEngine;

namespace HcUtils
{
    public class SpriteThemeSetter : ThemeSetter
    {
        [SerializeField] private SpriteRenderer sprite;

        [SerializeField] public Sprite[] sprites;
        protected override void Init()
        {
            if (!sprite)
            {
                sprite = GetComponent<SpriteRenderer>();
            }

            base.Init();
        }
        protected override void SetTheme(int ind)
        {
            if (!sprite) return;

            sprite.sprite = sprites[ind];
        }
    }
}