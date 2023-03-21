using UnityEngine;
namespace HcUtils
{
    public class CameraThemeSetter : ThemeSetter
    {
        private new Camera camera;
        public Color[] colors;

        protected override void Init()
        {
            camera = GetComponent<Camera>();
            base.Init();
        }


        protected override void SetTheme(int ind)
        {
            if (!camera) return;
            camera.backgroundColor = colors[ind];
        }
    }
}