using UnityEngine;
using UnityEngine.UI;

namespace DTL.UI
{
    public class LoadingIcon : MonoBehaviour
    {
        private Image image;
        private float fillValue = 0;
        Quaternion startRotation;
        Quaternion endRotation;
        private float lastFillValue = 0;
        private void Awake()
        {
            image = GetComponent<Image>();
        }
        private void Update()
        {
            if (!image) return;
            fillValue = Mathf.PingPong(Time.time, 1f);
            if (fillValue < lastFillValue)
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Lerp(0, 360, fillValue)));
            }
            else
            {
                transform.rotation = Quaternion.Euler(Vector3.zero);
            }
                  lastFillValue = fillValue;
            image.fillAmount = fillValue;
        }
    }
}