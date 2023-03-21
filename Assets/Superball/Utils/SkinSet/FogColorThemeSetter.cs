using UnityEngine;
namespace HcUtils
{
    public class FogColorThemeSetter : ThemeSetter
    {
        public Color[] colors;

        protected override void SetTheme(int ind)
        {
            RenderSettings.fogColor = colors[ind];
        }
    }
}