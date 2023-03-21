using FriendsGamesTools.UI;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools
{
    [ExecuteAlways]
    public class InternetProgressView : MonoBehaviour
    {
        [SerializeField] Image pic;
        [SerializeField] float speed1 = 0.1f, speed2 = 0.2f;
        private void Update()
        {
            if (!Application.isPlaying) return;
            pic.transform.localEulerAngles += Vector3.forward * Time.unscaledDeltaTime * speed1;
            pic.fillAmount = (pic.fillAmount + Time.unscaledDeltaTime * speed2 / 360f) % 1;
        }
        private void OnEnable()
        {
            if (Application.isPlaying) return;
            var parentPic = transform.parent.GetComponent<Image>();
            var currPic = transform.GetComponent<Image>();
            if (parentPic != null && currPic != null)
            {
                currPic.sprite = parentPic.sprite;
                currPic.type = parentPic.type;
            }
            FillParentRect.Fill(gameObject.GetComponent<RectTransform>());
        }
    }
}