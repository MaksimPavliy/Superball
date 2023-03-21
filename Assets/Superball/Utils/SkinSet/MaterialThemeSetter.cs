using UnityEngine;

namespace HcUtils
{
    public class MaterialThemeSetter : ThemeSetter
    {
        [SerializeField] private new Renderer renderer;
        public Material[] materials;

        protected override void Init()
        {
            if (!renderer)
            {
                renderer = GetComponent<Renderer>();
            }

            base.Init();
        }
        protected override void SetTheme(int ind)
        {
            if (!renderer) return;

            renderer.sharedMaterial = materials[ind];
        }
    }
}