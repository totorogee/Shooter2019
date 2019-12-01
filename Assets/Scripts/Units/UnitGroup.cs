using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TeamName
{
    Red =0,
    Blue =1
}

public class UnitGroup : MonoBehaviour
{
    public static List<UnitGroup> AllRed = new List<UnitGroup>();
    public static List<UnitGroup> AllBlue = new List<UnitGroup>();

    public UnitFleet Fleet;
    public UnitGroupSetting Setting;

    public List<UnitGroup> LinkedGroup; // TODO Nearest group
    public PosVector StartingPos; // Density of 2
    public PosVector CurrentPos; // TODO function , for Density 3 or 4


    public List<UnitBase> UnitBases = new List<UnitBase>();

    private float AttackRange = 30f; //
    private float AttackDamage = 10f; //
    private bool didInit;

    public void Init(UnitFleet fleet , PosVector starting,  int size )
    {
        Fleet = fleet;
        StartingPos = starting;
        if (Setting == null)
        {
            Setting = UnitController.Instance.UnitGroupSetting;
        }

        didInit = true;

        if( Fleet.Team == TeamName.Red)
        {
            AllRed.Add(this);
        }
        else
        {
            AllBlue.Add(this);
        }

        for (int i = 0; i <= size; i++)
        {
            GameObject Go = Instantiate(UnitController.Instance.UnitBasePrefab.gameObject);
            UnitBase unitBase = Go.GetComponent<UnitBase>();
            Go.transform.SetParent(transform);
            Go.transform.localPosition = Setting.GroupPosition[size][i];
            unitBase.Init(this);
            UnitBases.Add(unitBase);
        }
    }

    public void TakeDamage(float damage)
    {
        if (UnitBases.Count == 0)
        {
            Debug.Log("Error");
        }

        foreach (var item in UnitBases)
        {
            item.TakeDamage(damage / UnitBases.Count);
        }
    }
}
