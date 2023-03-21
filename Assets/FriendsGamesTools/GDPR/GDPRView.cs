using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools
{
    public class GDPRView : MonoBehaviour
    {
        [SerializeField] Button termsOfUse, privacyPolicy;
        static GDPRSettings settings => GDPRSettings.instance;
        private void Awake()
        {
            termsOfUse.onClick.AddListener(() => Application.OpenURL(settings.iOSTermsOfUseURL));
            privacyPolicy.onClick.AddListener(() => Application.OpenURL(settings.iOSPrivacyPolicyURL));
        }
    }
}