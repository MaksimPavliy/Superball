using UnityEngine;
namespace HcUtils
{
    public class SkyBoxThemeSetter : ThemeSetter
    {
        public Material[] skyboxes;

        protected override void SetTheme(int ind)
        {
            RenderSettings.skybox = skyboxes[ind];
        }
    }
}