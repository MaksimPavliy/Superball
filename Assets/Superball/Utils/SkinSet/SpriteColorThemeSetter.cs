using UnityEngine;

namespace HcUtils
{
    public class SpriteColorThemeSetter : ThemeSetter
    {
        [SerializeField] private SpriteRenderer sprite;

        [SerializeField] public Color[] _colors;
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

            sprite.color = _colors[ind];
        }
    }
}