using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitScanner : MonoBehaviour
{
    public Transform Icon;
    public Transform Aim;

    public List<UnitGroup> UnitGroups = new List<UnitGroup>();
    public List<UnitGroup> Enemy = new List<UnitGroup>();

    public int Radius = 0;
    public int WeaponDistant = 0;

    public UnitFleet Fleet;
    public PosVector StartingPos;

    public PosVector LocalPosition
    {
        get
        {
            if (localPosition == new PosVector(0, 0))
            {
                PosVector result = (StartingPos * Mathf.RoundToInt(Fleet.Density)) * 0.5f;
                localPosition = result.Rotate(Fleet.Angle);
            }
            return localPosition;
        }
    }
    private PosVector localPosition;

    public PosVector Position
    {
        get
        {
            if (position == new PosVector(0, 0))
            {
                position = LocalPosition + Fleet.Position;
            }
            return position;
        }
    }
    private PosVector position;

    public PosVector AimPosition
    {
        get
        {
            PosVector result = (StartingPos * Mathf.RoundToInt(Fleet.Density) * 0.5f) + new PosVector(0, WeaponDistant / 2);
            return result.Rotate(Fleet.Angle) + Fleet.Position;
        }
    }

    public bool Alive
    {
        get
        {
            foreach (var item in UnitGroups)
            {
                if (item.Alive)
                {
                    return true;
                }
            }
            return false;
        }
    }

    public void SetDirty()
    {
        position = new PosVector(0, 0);
        localPosition = new PosVector(0, 0);
    }

    private void Update()
    {
        Icon.position = new Vector3(Position.x, Position.y, 0);
        Aim.position = new Vector3( AimPosition.x , AimPosition.y , 0);
    }
}