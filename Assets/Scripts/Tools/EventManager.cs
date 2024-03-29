using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System;

public class EventManager : PrefabSingleton<EventManager>
{
    public struct EventArg
    {
        public string EventName;
        public Type ParamType;
    }

    protected Dictionary<EventArg, List<object>> eventDictionary = new Dictionary<EventArg, List<object>>();

    #region Generic argument

    public static void StartListening<T>(string eventName, UnityAction<T> listener) 
    {

        EventArg thisInfo = new EventArg
        {
            EventName = eventName,
            ParamType = typeof(T)
        };

        if (Instance.eventDictionary.TryGetValue(thisInfo, out List<object> thisEvent))
        {
            thisEvent.Add(listener);
        }
        else
        {
            thisEvent = new List<object> { listener };
            Instance.eventDictionary.Add(thisInfo, thisEvent);
        }
    }

    public static void StopListening<T>(string eventName, UnityAction<T> listener)
    {

        EventArg thisInfo = new EventArg
        {
            EventName = eventName,
            ParamType = typeof(T)
        };

        if (Instance.eventDictionary.TryGetValue(thisInfo, out List<object> thisEvent))
        {
            thisEvent.Remove(listener);
        }
        else
        {
            Debug.LogWarning ("Event not registered : " + eventName + " / Param : " + typeof(T));
        }
    }

    public static void TriggerEvent<T>(string eventName, T message)
    {

        EventArg thisInfo = new EventArg
        {
            EventName = eventName,
            ParamType = typeof(T)
        };

        if (Instance.eventDictionary.TryGetValue(thisInfo, out List<object> thisEvent))
        {
            foreach (var item in thisEvent)
            {
                ((UnityAction<T>)item).Invoke(message);
            }
        }
        else
        {
            Debug.LogWarning ("Event not registered : " + eventName + " / Param : " + typeof(T));
        }
    }

    #endregion

    #region No argument

    public static void StartListening(string eventName, UnityAction listener)
    {
        EventArg thisInfo = new EventArg
        {
            EventName = eventName,
            ParamType = null
        };

        if (Instance.eventDictionary.TryGetValue(thisInfo, out List<object> thisEvent))
        {
            thisEvent.Add(listener);
        }
        else
        {
            thisEvent = new List<object> { listener };
            Instance.eventDictionary.Add(thisInfo, thisEvent);
        }
    }

    public static void StopListening(string eventName, UnityAction listener)
    {
        EventArg thisInfo = new EventArg
        {
            EventName = eventName,
            ParamType = null
        };

        if (Instance.eventDictionary.TryGetValue(thisInfo, out List<object> thisEvent))
        {
            thisEvent.Remove(listener);
        }
        else
        {
            Debug.LogWarning("Event not registered : " + eventName);
        }
    }

    public static void TriggerEvent(string eventName)
    {
        EventArg thisInfo = new EventArg
        {
            EventName = eventName,
            ParamType = null
        };

        if (Instance.eventDictionary.TryGetValue(thisInfo, out List<object> thisEvent))
        {
            foreach (var item in thisEvent)
            {
                ((UnityAction)item).Invoke();
            }
        }
        else
        {
            Debug.LogWarning("Event not registered : " + eventName);
        }
    }

    #endregion




    //public static void StopListening(string eventName, UnityAction<object> listener)
    //{
    //    if (eventManager == null)
    //        return;

    //    EventInfo thisInfo = new EventInfo
    //    {
    //        EventName = eventName,
    //        ParamType = typeof(object)
    //    };

    //    if (instance.eventDictionary.TryGetValue(thisInfo, out ListenEvent thisEvent))
    //    {
    //        thisEvent.RemoveListener(listener);
    //    }
    //    else
    //    {
    //        Debug.Log("Event not registered : " + eventName + " With Param : " + typeof(object));
    //    }
    //}

    //public static void TriggerEvent(string eventName)
    //{
    //    TriggerEvent(eventName, null);
    //}

    //public static void TriggerEvent(string eventName, object message)
    //{
    //    EventInfo thisInfo = new EventInfo
    //    {
    //        EventName = eventName,
    //        Param = typeof(object)
    //    };

    //    if (instance.eventDictionary.TryGetValue(thisInfo, out ListenEvent thisEvent))
    //    {
    //        thisEvent.Invoke(message);
    //    }
    //    else
    //    {
    //        Debug.Log("Event not registered : " + eventName + " With Param : " + typeof(object));
    //    }
    //}

    //public static void StartListening(string eventName, UnityAction<object> listener)
    //{
    //    EventInfo thisInfo = new EventInfo
    //    {
    //        EventName = eventName,
    //        Param = typeof(object)

    //    };

    //    Debug.Log(eventName + " " + thisInfo.Param);

    //    if (instance.eventDictionary.TryGetValue(thisInfo, out ListenEvent thisEvent))
    //    {
    //        thisEvent.AddListener(listener);
    //    }
    //    else
    //    {
    //        thisEvent = new ListenEvent();
    //        thisEvent.AddListener(listener);
    //        instance.eventDictionary.Add(thisInfo, thisEvent);
    //    }
    //}

}
