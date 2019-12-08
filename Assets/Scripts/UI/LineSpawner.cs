﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineSpawner : Spawnable
{
    public float EndLenght = 1f;
    public float StartLenght = 1f;

    public LineRenderer LineRenderer;

    public Gradient StartColor;
    public Gradient EndColor;

    public bool LoopColor = false;
    public float LoopTime = 0.2f;

    protected override void OnEnable()
    {
        Body = LineRenderer.transform;
        base.OnEnable();
    }

    protected override IEnumerator DelayMove(float startTime, float endTime)
    {
        yield return new WaitForEndOfFrame();
    }

    protected void Update()
    {
        Gradient gradient = GradientUtil.Lerp(StartColor, EndColor, LoopColor ? Mathf.PingPong(Time.realtimeSinceStartup , LoopTime ) : Lerp );
        LineRenderer.colorGradient = gradient;

        LineRenderer.SetPosition(0, Vector3.Lerp(EndTarget.position, StartTarget.position ,  StartLenght));
        LineRenderer.SetPosition(1, Vector3.Lerp(StartTarget.position , EndTarget.position , EndLenght));

    }
}