using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TouchEvent : EventTrigger
{
    public override void OnPointerClick(PointerEventData eventData)
    {
        Vector2 pos = Camera.main.ScreenToWorldPoint(eventData.pressPosition);
        EventManager.TriggerEvent(EventList.OnMousePressed, pos);
    }
}
