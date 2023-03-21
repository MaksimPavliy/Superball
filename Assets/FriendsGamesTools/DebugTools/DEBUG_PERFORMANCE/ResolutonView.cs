using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.DebugTools
{
    public class ResolutonView: MonoBehaviour
    {
        [SerializeField] TMP_Dropdown view;
#if DEBUG_PERFORMANCE
        List<Resolution> resolutions; 
        private void Awake()
        {
            view.ClearOptions();
            resolutions = new List<Resolution>();
#if UNITY_IOS
            resolutions = Screen.resolutions.ToList();
#else
            var res = Screen.currentResolution;
            for (int i = 0; i < 8; i++)
            {
                resolutions.Add(res);
                res.width = res.width * 2 / 3;
                res.height = res.height * 2 / 3;
            }
#endif
            var value = resolutions.FindIndex(r => r.width == Screen.currentResolution.width 
                && r.height == Screen.currentResolution.height 
                && r.refreshRate == Screen.currentResolution.refreshRate);

            view.AddOptions(resolutions.ConvertAll(r => r.ToString()));
            view.value = value;
            view.onValueChanged.AddListener(OnChanged);
        }
        private void OnChanged(int val)
        {
            var res = resolutions[val];
            Debug.Log($"change resolution from {Screen.currentResolution.ToString()} to {res}");
            Screen.SetResolution(res.width, res.height, true, res.refreshRate);
        }
#endif
    }
}
