using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.UI
{
    public class InfoNotificationView : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI label;
        [SerializeField] float duration = 1;
        [SerializeField] List<MaskableGraphic> fadeOutItems = new List<MaskableGraphic>();
#if INFO_NOTIFICATION
        static InfoNotificationSettings config => InfoNotificationSettings.instance;
        static InfoNotificationView instance;
        static object shown;
        public static async void Show(string text)
        {
            if (instance == null)
                instance = Instantiate(config.prefab, Windows.instance.transform);
            var curr = shown = new object();

            instance.gameObject.SetActive(true);
            instance.label.text = text;
            await AsyncUtils.SecondsWithProgress(instance.duration, progress => {
                if (curr != shown) return;
                instance.UpdateView(progress);
            }, true);
            if (curr != shown) return;
            instance.gameObject.SetActive(false);
        }
        protected virtual void UpdateView(float progress)
        {
            var alpha = 1 - Mathf.Pow(progress, 2);
            fadeOutItems.ForEach(pic => pic.SetAlpha(alpha));
        }
#endif
    }
}