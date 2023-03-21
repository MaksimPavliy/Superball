using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Superball
{
    public class RateStar : MonoBehaviour, IPointerClickHandler
    {
        public class IntEvent : UnityEvent<int> { }
        [SerializeField] private GameObject ActiveStar;

        [HideInInspector]
        public IntEvent eSelected;

        [HideInInspector]
        public int index = -1;
        public void SetActive(bool active) => ActiveStar.gameObject.SetActive(active);

        private void Awake()
        {
            eSelected = new IntEvent();
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            eSelected.Invoke(index);
        }
    }
}