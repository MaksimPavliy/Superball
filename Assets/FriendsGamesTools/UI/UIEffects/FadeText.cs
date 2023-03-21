#if UI
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace FriendsGamesTools.UI
{
    public class FadeText : MonoBehaviour
    {
        public TextMeshProUGUI label;
        public float duration = 1;
        public Vector3 move = new Vector3(10, 30, 0);

        public async Task PlayInstance(string text, Vector3 worldCoo, Transform parent)
        {
            gameObject.SetActive(false);
            var instance = Instantiate(this, parent);
            instance.transform.position = worldCoo;
            instance.label.text = text;
            instance.gameObject.SetActive(true);

            float elapsed = 0;
            while (elapsed < duration)
            {
                var progress = elapsed / duration;
                instance.label.alpha = 1 - progress;
                instance.transform.position = Vector3.Lerp(worldCoo, worldCoo + move, progress);
                await Awaiters.EndOfFrame;
                elapsed += Time.deltaTime;
            }

            Destroy(instance.gameObject);
        }
    }
}
#endif