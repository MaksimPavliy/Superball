using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HcUtils
{
 //Disables fog for a current render.
 //It's handy to use it to disable for UI camera
    public class FogDisabler : MonoBehaviour
    {
        bool previousFogState;
        void OnPreRender()
        {
            previousFogState = RenderSettings.fog;
            RenderSettings.fog = false;
        }
        void OnPostRender()
        {
            RenderSettings.fog = previousFogState;
        }

    }
}