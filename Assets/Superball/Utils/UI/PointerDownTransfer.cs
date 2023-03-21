using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PointerDownTransfer : MonoBehaviour, IPointerDownHandler
{
    [SerializeField]
    private Button playButton;
    private void Awake()
    {
        if (playButton.targetGraphic)
        {
            playButton.targetGraphic.raycastTarget = false;
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        ExecuteEvents.Execute(playButton.gameObject, eventData, ExecuteEvents.pointerDownHandler);
    }
}
