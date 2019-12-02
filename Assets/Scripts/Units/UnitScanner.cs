using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitScanner : MonoBehaviour
{
    public Transform Icon;
    public Transform Aim;

    public UnitFleet Fleet;
    public PosVector StartingPos;


    public List<UnitGroup> UnitGroups = new List<UnitGroup>();
    public int MaxDistance = 0;
    public int WeaponDistant = 0;

    public List<UnitGroup> Enemy = new List<UnitGroup>();

    public PosVector LocalPosition
    {
        get
        {
            PosVector result = (StartingPos * Mathf.RoundToInt(Fleet.Density)) * 0.5f;
            return PosVector.Rotate(result, Fleet.Angle);
        }
    }

    public PosVector Position
    {
        get
        {
            return LocalPosition + Fleet.Position;
        }
    }

    public PosVector AimPosition
    {
        get
        {
            PosVector result = (StartingPos + new PosVector(0, 10)) * Mathf.RoundToInt(Fleet.Density) * 0.5f;
            return PosVector.Rotate(result, Fleet.Angle) + Fleet.Position;
        }
    }


    private void Update()
    {
        Icon.position = new Vector3(Position.x, Position.y, 0);
        Aim.position = new Vector3( AimPosition.x , AimPosition.y , 0);
    }
}