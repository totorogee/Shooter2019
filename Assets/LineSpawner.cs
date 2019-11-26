using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineSpawner : Spawnable
{
    public LineRenderer LineRenderer;
    public Transform LineEndPoint;
    public Transform Target;

    public Gradient StartColor;
    public Gradient EndColor;

    public bool LoopColor = false;
    public float LoopTime = 0.5f;


    public void SetTarget(Transform target)
    {
        Target = target;
    }

    protected override void OnEnable()
    {
        Body = LineRenderer.transform;
        base.OnEnable();
    }

    protected override IEnumerator DelayMove(float startTime, float endTime)
    {
        LineRenderer.SetPosition(0,StartPos);
        yield return new WaitForSeconds(startTime);
        float startMove = Time.realtimeSinceStartup;

        while (Time.realtimeSinceStartup < startMove + endTime)
        {
            Lerp = (Time.realtimeSinceStartup - startMove) / endTime;
            LineRenderer.SetPosition(0, Vector3.Lerp(StartPos, EndPos, Lerp));
            yield return new WaitForEndOfFrame();
        }
    }



    protected void Update()
    {
        Gradient gradient = GradientUtil.Lerp(StartColor, EndColor, LoopColor ? Mathf.PingPong(Time.realtimeSinceStartup , LoopTime ) : Lerp );
        LineRenderer.colorGradient = gradient;

        if (Target != null)
        {
            LineEndPoint.position = Target.position;
        }
        LineRenderer.SetPosition(1, LineEndPoint.localPosition);

    }
}
