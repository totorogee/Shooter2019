using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Spawnable : MonoBehaviour
{
    public Transform Body;
    public Vector3 StartPos = Vector3.zero;
    public Vector3 EndPos = Vector3.zero;
    public bool StartOnAwake = false;
    public float appearTime = 1f;
    public float disappearTime = 1f;

    protected float Lerp = 0;

    protected Coroutine disappearCoroutine;
    protected Coroutine appearCoroutine;
    protected Coroutine moveCoroutine;

    protected string IsAppearOnTrigger = "";
    protected string IsDisappearOnTrigger = "";

    protected virtual void OnEnable()
    {
        if (Body == null)
        {
            Body = this.transform;
        }

        if (StartPos == Vector3.zero)
        {
            StartPos = Body.localPosition;
        }

        if ( EndPos == Vector3.zero)
        {
            EndPos = Body.localPosition;
        }

        if (StartOnAwake)
        {

            OnSpawn(appearTime , disappearTime);
        }
    }

    protected virtual void OnDisable()
    {
        if ( !string.IsNullOrWhiteSpace( IsAppearOnTrigger))
        {
            EventManager.StopListening(IsAppearOnTrigger, Appear);
        }

        if (!string.IsNullOrWhiteSpace(IsDisappearOnTrigger))
        {
            EventManager.StopListening(IsDisappearOnTrigger, Disappear);
        }
    }

    public virtual void SetStartTrigger(string eventName)
    {
        IsAppearOnTrigger = eventName;
        EventManager.StartListening(eventName, Appear);
    }

    public virtual void SetEndTrigger(string eventName)
    {
        IsDisappearOnTrigger = eventName;
        EventManager.StartListening(eventName, Disappear);
    }

    public virtual void OnSpawn(Vector3 Start , Vector3 End , float startTime = 0f , float endTime = 1f)
    {
        StartPos = new Vector3(Start.x, Start.y, Body.localPosition.z);
        EndPos = new Vector3(End.x, End.y, Body.localPosition.z);
        OnSpawn(startTime, endTime);
    }

    public virtual Spawnable NewSpawn()
    {
        GameObject go = Instantiate(gameObject);
        return go.GetComponent<Spawnable>();
    }

    public virtual void OnSpawn(float startTime = 0, float endTime = 1f)
    {
        Disappear();

        Body.localPosition = new Vector3( StartPos.x , StartPos.y , Body.localPosition.z);

        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        if (appearCoroutine != null)
        {
            StopCoroutine(appearCoroutine);
        }

        if (disappearCoroutine != null)
        {
            StopCoroutine(disappearCoroutine);
        }

        if (startTime == 0)
        {
            Appear();
        }
        else
        {
            appearCoroutine = StartCoroutine(DelayAppear(startTime));
        }

        disappearCoroutine = StartCoroutine(DelayDisappear(startTime + endTime));
        moveCoroutine = StartCoroutine(DelayMove(startTime, endTime));
    }

    protected virtual IEnumerator DelayAppear(float time)
    {
        yield return new WaitForSeconds(time);
        Appear();
    }

    protected virtual IEnumerator DelayDisappear(float time)
    {
        yield return new WaitForSeconds(time);
        Disappear();
        Kill();
    }

    protected virtual IEnumerator DelayMove(float startTime , float endTime)
    {
        Body.localPosition = StartPos;
        yield return new WaitForSeconds(startTime);
        float startMove = Time.realtimeSinceStartup;

        while ( Time.realtimeSinceStartup < startMove + endTime)
        {
            Lerp = (Time.realtimeSinceStartup - startMove) / endTime;
            Body.localPosition = Vector3.Lerp(StartPos, EndPos , Lerp);
            yield return new WaitForEndOfFrame();
        }
    }

    protected virtual void Appear()
    {
        Body.gameObject.SetActive(true);
    }

    protected virtual void Disappear()
    {
        Body.gameObject.SetActive(false);
    }

    public virtual void Kill()
    {
        StopAllCoroutines();
        Destroy(gameObject);
    }
}
