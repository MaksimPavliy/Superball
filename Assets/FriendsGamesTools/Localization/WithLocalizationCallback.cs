using System.Collections.Generic;
using UnityEngine;

namespace FriendsGamesTools
{
    public abstract class WithLocalizationCallbackInRuntime : MonoBehaviour
    {
#if LOCALIZATION
        private static List<WithLocalizationCallbackInRuntime> _instances = new List<WithLocalizationCallbackInRuntime>();
        public static IReadOnlyList<WithLocalizationCallbackInRuntime> instances => _instances;
        protected virtual void Awake()
        {
            _instances.Add(this);
            if (Application.isPlaying)
                LocalizationManager.EnsureExists();
        }
        protected virtual void OnDestroy()
        {
            _instances.Remove(this);
        }
        public abstract void SetLocalizationInRuntime(Language lang, LocalizationSettings settings);
#endif
    }
}
