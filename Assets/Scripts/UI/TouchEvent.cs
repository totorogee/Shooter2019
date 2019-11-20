using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[System.Serializable]
public class TouchUnityEvent : UnityEvent<PointerEventData>
{
}

public class TouchEvent : EventTrigger
{
    public TouchUnityEvent OnTouch = new TouchUnityEvent();

    public override void OnPointerClick(PointerEventData eventData)
    {
        OnTouch.Invoke(eventData);
    }
}
