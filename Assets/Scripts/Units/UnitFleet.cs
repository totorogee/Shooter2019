using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitFleet : MonoBehaviour
{
    public PosVector Position
    {
        get
        {
            return new PosVector(TheGroup.localPosition);
        }
    }

    public int Angle
    {
        get
        {
            return Mathf.RoundToInt(-TheGroup.localEulerAngles.z + 360);
        }
    }

    public UnitGroup Touching;

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

    public List<UnitGroup> Enemy = new List<UnitGroup>();

    public int FleetRadis = 0;

    public bool DidInit = false;

    public static List<UnitFleet> AllRed = new List<UnitFleet>();
    public static List<UnitFleet> AllBlue = new List<UnitFleet>();

    public Transform TheGroup;
    public Transform TheGround;
    public TeamName Team = TeamName.Red;

    public List<UnitGroup> UnitGroups = new List<UnitGroup>();
    public List<UnitScanner> UnitScanners = new List<UnitScanner>();

    private const int scannerDensity = 6;

    public int CurrentFormationID;

    public float Density = 2;

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
        SetScanner();
        UpdateFleetRadis();

        //Debug.Log("Init Time : " + (Time.realtimeSinceStartup - time));
    }

    public void ChangeDensity(float density)
    {
        Density = density;

        foreach (var item in UnitGroups)
        {
            item.transform.localPosition = new Vector3(item.StartingPos.x * density / 2, item.StartingPos.y * density / 2, item.transform.localPosition.z);
        }
        UpdateFleetRadis();
    }

    public void ChangeFormation()
    {
        // TODO
    }

    private void SetScanner()
    {
        GameObject Go = Instantiate(UnitController.Instance.UnitScannerPrefab.gameObject , TheGroup);
        UnitScanner temp = Go.GetComponent<UnitScanner>();
        temp.Fleet = this;
        temp.StartingPos = new PosVector(0, 0);
        temp.UnitGroups = new List<UnitGroup>();

        UnitScanners = new List<UnitScanner> { temp };

        foreach (var item in UnitGroups)
        {
            int x = Mathf.RoundToInt ((float)item.StartingPos.x / scannerDensity );
            int y = Mathf.RoundToInt ((float)item.StartingPos.y / scannerDensity );

            PosVector pos = new PosVector(x * scannerDensity, y * scannerDensity);
            temp = null;

            foreach (var scanner in UnitScanners)
            {
                if (scanner.StartingPos == pos )
                {
                    temp = scanner;
                }
            }

            if (temp == null)
            {
                Go = Instantiate(UnitController.Instance.UnitScannerPrefab.gameObject , TheGroup);
                temp = Go.GetComponent<UnitScanner>();
                temp.Fleet = this;
                temp.StartingPos = pos;
                temp.UnitGroups = new List<UnitGroup> { item };

                UnitScanners.Add(temp);
            }
            else
            {
                temp.UnitGroups.Add(item);
            }
        }

        UnitScanners.Sort((x, y) =>
        {
            return PosVector.SqDistance(new PosVector(0, 0), x.StartingPos).CompareTo
            ( PosVector.SqDistance(new PosVector(0, 0), y.StartingPos));
        });

        foreach (var item in UnitScanners)
        {
            item.UnitGroups.Sort((x, y) =>
            {
                int result = PosVector.SqDistance(item.StartingPos, x.StartingPos).CompareTo
                (PosVector.SqDistance(item.StartingPos, y.StartingPos));

                if (result != 0)
                {
                    return result;
                }

                return PosVector.Angle(item.StartingPos, x.StartingPos).CompareTo
                (PosVector.Angle(item.StartingPos, y.StartingPos));
            });

            int distant = 0;

            for (int i = 0; i < item.UnitGroups.Count; i++)
            {
                var x = item.UnitGroups[i];
                distant = Mathf.Max(PosVector.SqDistance(item.StartingPos, x.StartingPos), distant);
            }
            item.Radius = Mathf.CeilToInt(Mathf.Sqrt(distant));

            distant = 0;
            for (int i = 0; i < item.UnitGroups.Count; i++)
            {
                var x = item.UnitGroups[i];
                for (int j = 0; j < x.Setting.Weapons.Count; j++)
                {
                    distant = Mathf.Max(distant, x.Setting.Weapons[j].Range);
                }
            }

            item.WeaponDistant = distant;
        }
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

    private void UpdateFleetRadis()
    {
        FleetRadis = 0;

        foreach (var item in UnitGroups)
        {
            FleetRadis = Mathf.Max(FleetRadis, PosVector.SqDistance(item.Position, Position));
        }

        FleetRadis = Mathf.RoundToInt(Mathf.Sqrt(FleetRadis));


    }
}
