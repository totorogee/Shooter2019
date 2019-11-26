using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fleet : MonoBehaviour
{
    public static List<Fleet> AllRed = new List<Fleet>();
    public static List<Fleet> AllBlue = new List<Fleet>();

    public Transform TheGroup;
    public Transform TheGround;
    public TeamName Team = TeamName.Red;

    public List<UnitGroup> UnitGroups = new List<UnitGroup>();
    public List<PosVector> Scanner = new List<PosVector>();



    public int CurrentFormationID;

    public float Density = 2;

    public void Init()
    {
        SetScanner();
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

    private void InitGroups()
    {
        // TODO
    }

    private void SetGroupsLinks(UnitGroup unit )
    {
        // TODO
    }

}
