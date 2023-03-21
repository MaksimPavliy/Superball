#if DEBUG_PANEL
using TMPro;
using UnityEngine;

namespace FriendsGamesTools.DebugTools
{
    public class ResolutionsDropdown : MonoBehaviour
    {
        [SerializeField] TMP_Dropdown dropdown;
        Resolution[] resolutions;
        string GetResolutionString(Resolution r) => $"{r.width}x{r.height} {r.refreshRate}Hz";
        private void Awake()
        {
            resolutions = Screen.resolutions;
            dropdown.options.Clear();
            dropdown.AddOptions(resolutions.ConvertAll(GetResolutionString));
            dropdown.onValueChanged.AddListener(OnChanged);
        }

        private void OnChanged(int resolutionInd)
        {
            var r = resolutions[resolutionInd];
            Screen.SetResolution(r.width, r.height, true, r.refreshRate);
            Debug.Log($"Resolution changed to {GetResolutionString(r)}");
        }
    }
}
#endif