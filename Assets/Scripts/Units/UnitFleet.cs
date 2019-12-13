using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPlaningData // TODO
{
    UnitGroup Target;

    float currentHP;
    float totalDamage;
}

public class UnitFleet : MonoBehaviour
{
    public int FleetRadis
    {
        get
        {
            if (fleetRadis == 0)
            {
                foreach (var item in UnitGroups)
                {
                    if (item.Alive)
                    {
                        fleetRadis = Mathf.Max(fleetRadis, PosVector.SqDistance(item.Position, Position));
                    }
                }
                fleetRadis = Mathf.RoundToInt(Mathf.Sqrt(fleetRadis));
            }
            return fleetRadis;
        }
    }
    private int fleetRadis = 0;

    public PosVector Position
    {
        get
        {
            if (position == new PosVector(0, 0))
            {
                position = new PosVector(TheGroup.localPosition);
            }

            return position;
        }
    }
    private PosVector position = new PosVector (0,0);

    public int Angle
    {
        get
        {
            if (angle == 0)
            {
                angle = Mathf.RoundToInt(-TheGroup.localEulerAngles.z + 360);
            }

            return angle;
        }
    }
    private int angle = 0;

    public List<UnitGroup> EnemyTouching = new List<UnitGroup>();
    public List<UnitGroup> FriendlyTouching = new List<UnitGroup>();

    public bool Touching
    {
        get
        {
            return EnemyTouching.Count > 0;
        }
    }

    public float ForwardSpeed
    {
        get
        {
            if (forwardSpeed <= 0)
            {
                forwardSpeed = 1;

                foreach (var item in UnitGroups)
                {
                    forwardSpeed = Mathf.Min(forwardSpeed, item.Setting.ForwardSpeed);
                }
            }
            return forwardSpeed * (Touching ? 0.25f : 1f);
        }
    }
    private float forwardSpeed = 0;

    public float BackwardSpeed
    {
        get
        {
            if (backwardSpeed <= 0)
            {
                backwardSpeed = 1;

                foreach (var item in UnitGroups)
                {
                    backwardSpeed = Mathf.Min(backwardSpeed, item.Setting.BackwardSpeed);
                }
            }
            return backwardSpeed * (Touching ? 0.25f : 1f);
        }
    }
    private float backwardSpeed = 0;

    public float SidewaySpeed
    {
        get
        {
            if (sidewaySpeed <= 0)
            {
                sidewaySpeed = 1;

                foreach (var item in UnitGroups)
                {
                    sidewaySpeed = Mathf.Min(sidewaySpeed, item.Setting.SidewaySpeed);
                }
            }
            return sidewaySpeed * (Touching ? 0.25f : 1f);
        }
    }
    private float sidewaySpeed = 0;

    public float RotationSpeed
    {
        get
        {
            return (forwardSpeed / FleetRadis) * (Touching ? 0.25f : 1f); 
        }
    }

    public float WeaponDistant = 20f;
    public List<UnitGroup> Enemy = new List<UnitGroup>();
    public List<UnitGroup> NearBy = new List<UnitGroup>();

    public bool DidInit = false;

    public static List<UnitFleet> AllRed = new List<UnitFleet>();
    public static List<UnitFleet> AllBlue = new List<UnitFleet>();

    public Transform TheGroup;
    public Transform TheGround;
    public TeamName Team = TeamName.Red;

    public List<UnitGroup> UnitGroups = new List<UnitGroup>();
    public int CurrentFormationID;

    public float Density = 2;

    public void SetDirty()
    {
        position = new PosVector(0, 0);
        angle = 0;
        fleetRadis = 0;
    }

    public void Init(int formationID)
    {
        var time = Time.realtimeSinceStartup;

        DidInit = true;

        LoadSlot(formationID);
        if (Team == TeamName.Red)
        {
            AllRed.Add(this);
        }
        else
        {
            AllBlue.Add(this);
        }

        float distant = 0;
        for (int i = 0; i < UnitGroups.Count; i++)
        {
            var x = UnitGroups[i];
            for (int j = 0; j < x.Setting.Weapons.Count; j++)
            {
                distant = Mathf.Max(distant, x.Setting.Weapons[j].Range);
            }
        }

        WeaponDistant = distant;
        //Debug.Log("Init Time : " + (Time.realtimeSinceStartup - time));
    }

    public void ChangeDensity(float density)
    {
        Density = density;

        foreach (var item in UnitGroups)
        {
            item.transform.localPosition = new Vector3(item.StartingPos.x * density / 2, item.StartingPos.y * density / 2, item.transform.localPosition.z);
        }
    }

    public void ChangeFormation()
    {
        // TODO
    }

    private void SetGroupsLinks(UnitGroup unit )
    {
        // TODO
    }

    public void CheckAttack()
    {

    }

    private void LoadSlot(int ID)
    {
        Reset();
        Dictionary<PosVector, int> pos = SavedFormation.SavedList[ID].PositionGroupSizePair;
        PlaceUnits(pos);
        ChangeDensity(Density);
    }

    private void PlaceUnits(Dictionary<PosVector, int> data )
    {
        foreach (var item in data)
        {
            PlaceUnit(item.Key, item.Value);
        }
    }

    private void PlaceUnit(PosVector pos, int size)
    {
        GameObject Go = Instantiate(UnitController.Instance.UnitGroupPrefab , TheGroup).gameObject;
        UnitGroup unitGroup = Go.GetComponent<UnitGroup>();
        Go.transform.localPosition = Floor.Instance.GetTileByPos(pos).TileObject.transform.localPosition;
        unitGroup.Init(this , pos , size);
        UnitGroups.Add(unitGroup);
    }

    private void Reset()
    {

    }


}
