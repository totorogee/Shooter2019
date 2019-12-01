using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitFleet : MonoBehaviour
{
    public bool DidInit = false;

    public static List<UnitFleet> AllRed = new List<UnitFleet>();
    public static List<UnitFleet> AllBlue = new List<UnitFleet>();

    public Transform TheGroup;
    public Transform TheGround;
    public TeamName Team = TeamName.Red;

    public List<UnitGroup> UnitGroups = new List<UnitGroup>();
    public Dictionary<PosVector, List<UnitGroup>> Scanners = new Dictionary<PosVector, List<UnitGroup>>();

    private const int scannerDensity = 6;
    public Transform TestIcon;

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

        Scanners = new Dictionary<PosVector, List<UnitGroup>>();

        foreach (var item in UnitGroups)
        {
            int x = Mathf.RoundToInt ((float)item.StartingPos.x / scannerDensity );
            int y = Mathf.RoundToInt ((float)item.StartingPos.y / scannerDensity );

            if (!Scanners.ContainsKey (new PosVector(x * scannerDensity, y * scannerDensity)))
            {
                //Debug.Log(item.x + " " + x * scannerDensity);
                Scanners.Add(new PosVector(x * scannerDensity, y * scannerDensity), new List<UnitGroup> { item } );
                Transform t = Instantiate(UnitController.Instance.UnitGroupSetting.TestIcon, new Vector3(x * scannerDensity, y* scannerDensity, 0), Quaternion.identity);
                t.SetParent(TheGroup);
            }
            else
            {
                Scanners[new PosVector(x * scannerDensity, y * scannerDensity)].Add(item);
            }
        }

        foreach (var item in Scanners)
        {
            //Debug.Log(item.Key + " " + item.Value.Count);
            foreach (var x in item.Value)
            {
                Debug.Log(PosVector.SqDistance(item.Key, x.StartingPos));
            }

        }
    }

    private void SetGroupsLinks(UnitGroup unit )
    {
        // TODO
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
