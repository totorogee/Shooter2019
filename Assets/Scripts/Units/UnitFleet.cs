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
    //public List<PosVector> Scanner = new List<PosVector>();

    public int CurrentFormationID;

    public float Density = 2;

    public void Init(int formationID)
    {
        DidInit = true;
        //SetScanner();
        LoadSlot(formationID);
        if (Team == TeamName.Red)
        {
            AllRed.Add(this);
        }

    }

    public void ChangeDensity()
    {
        // TODO
    }

    public void ChangeFormation()
    {
        // TODO
    }

    private void SetScanner()
    {
        // TODO
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
        ChangeDensity();
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
        unitGroup.Init(this , size);
        UnitGroups.Add(unitGroup);
    }

    private void Reset()
    {

    }
}
