using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(UnitFleet))]
public class DummyEnemy : MonoBehaviour
{
    public UnitFleet UnitFleet;
    public Vector3 StartingPos;
    public float Direction;

    public bool DidInit = false;

    private void Start()
    {
        UnitFleet = GetComponent<UnitFleet>();
    }

    private void FixedUpdate()
    {
        if (UnitFleet.DidInit)
        {
            if (!DidInit)
            {
                Init();
            }

            UnitFleet.TheGroup.Translate(new Vector3(0, 1f, 0) * Time.fixedDeltaTime);
        }
    }

    private void Init()
    {
        DidInit = true;
        UnitFleet.TheGroup.localPosition = StartingPos;
        UnitFleet.TheGroup.eulerAngles = new Vector3(0, 0, Direction);
    }
}
