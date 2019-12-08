using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Spawnable : MonoBehaviour
{
    public Transform Body;
    public Transform StartTarget;
    public Transform EndTarget;
    public Vector3 StartPos = Vector3.zero;
    public Vector3 EndPos = Vector3.zero;
    public float appearTime = 1f;
    public float disappearTime = 1f;

    protected float Lerp = 0;

    protected Coroutine disappearCoroutine;
    protected Coroutine appearCoroutine;
    protected Coroutine moveCoroutine;

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
    }

    protected virtual void OnDisable()
    {

    }

    public virtual Spawnable Spawn(Vector3 start, Vector3 end, float startTime = 0f, float endTime = 1f)
    {
        StartPos = new Vector3(start.x, start.y, Body.localPosition.z);
        EndPos = new Vector3(end.x, end.y, Body.localPosition.z);
        return Spawn(startTime , endTime);
    }

    public virtual Spawnable Spawn(Transform follow , float startTime = 0f, float endTime = 1f)
    {
        StartTarget = follow;
        EndTarget = follow;
        return Spawn(startTime, endTime);
    }

    public virtual Spawnable Spawn(Transform start, Transform end, float startTime = 0f, float endTime = 1f)
    {
        StartTarget = start;
        EndTarget = end;
        return Spawn(startTime, endTime);
    }

    public virtual Spawnable Spawn(float startTime = 0, float endTime = 1f)
    {
        GameObject go = Instantiate(gameObject);
        Spawnable spawnable = go.GetComponent<Spawnable>();
        spawnable.OnSpawn(startTime, endTime);
        return spawnable;
    }

    protected virtual void OnSpawn(float startTime = 0, float endTime = 1f)
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
        Kill();
    }

    protected virtual IEnumerator DelayMove(float startTime , float endTime)
    {
        Body.localPosition = StartPos;
        yield return new WaitForSeconds(startTime);
        float startMove = Time.realtimeSinceStartup;

        while ( Time.realtimeSinceStartup < startMove + endTime)
        {
            if (StartTarget != null && EndTarget != null)
            {
                StartPos = StartTarget.position;
                EndPos = EndTarget.position;
            }

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
