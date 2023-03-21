using TMPro;
using UnityEngine;

namespace FriendsGamesTools
{
    public class BuildInfoView : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI buildInfo;
        private void OnEnable() {
            if (buildInfo != null)
                buildInfo.text = BuildInfoManager.buildInfo;
        }
    }
}