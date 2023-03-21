using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    [ExecuteAlways]
    [RequireComponent(typeof(Image))]
    public class ScrollDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IInitializePotentialDragHandler
    {
        RectTransform rect;
        void Awake() => rect = GetComponent<RectTransform>();
        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData) { }
        void IDragHandler.OnDrag(PointerEventData eventData)
            => rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y + eventData.delta.y);
        void IInitializePotentialDragHandler.OnInitializePotentialDrag(PointerEventData eventData)
            => eventData.useDragThreshold = false;
    }
}
