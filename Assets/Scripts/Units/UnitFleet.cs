using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitScanner
{
    public PosVector StartingPos;
    public List<UnitGroup> UnitGroups = new List<UnitGroup>();
    public int maxDistance = 0;
}

public class UnitFleet : MonoBehaviour
{
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

    private void SetScanner()
    {
        UnitScanners = new List<UnitScanner>();

        foreach (var item in UnitGroups)
        {
            int x = Mathf.RoundToInt ((float)item.StartingPos.x / scannerDensity );
            int y = Mathf.RoundToInt ((float)item.StartingPos.y / scannerDensity );

            PosVector pos = new PosVector(x * scannerDensity, y * scannerDensity);
            UnitScanner temp = null;

            foreach (var scanner in UnitScanners)
            {
                if (scanner.StartingPos == pos )
                {
                    temp = scanner;
                }
            }

            if (temp == null)
            {
                temp = new UnitScanner
                {
                    StartingPos = pos,
                    UnitGroups = new List<UnitGroup> { item }
                };

                UnitScanners.Add(temp);

                Transform t = Instantiate(UnitController.Instance.UnitGroupSetting.TestIcon, TheGroup.position + new Vector3(x * scannerDensity, y * scannerDensity, 0), TheGroup.rotation);
                t.SetParent(TheGroup);
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

            int distance = 0;
            for (int i = 0; i < item.UnitGroups.Count; i++)
            {
                var x = item.UnitGroups[i];
                distance = Mathf.Max(PosVector.SqDistance(item.StartingPos, x.StartingPos), distance);
            }
            item.maxDistance = Mathf.CeilToInt(Mathf.Sqrt(distance));
        }
    }

    private void SetGroupsLinks(UnitGroup unit )
    {
        // TODO
    }

    public void CheckAttack()
    {
        //if (Fleet.Team == TeamName.Red)
        //{
        //    foreach (var item in AllBlue)
        //    {
        //        if ((transform.position - item.transform.position).magnitude < AttackRange)
        //        {
        //            Setting.Weapons[0].ShootingEffect.OnSpawn(transform.position, item.transform.position, 0, 0.2f);
        //            item.TakeDamage(AttackDamage);
        //        }
        //    }
        //}
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
