using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ClockStatus
{
    ClockInit = 0,
    Scan = 1,
    Plan = 2,
    //Shoot = 3,
    //Move = 4,
    //Scyn = 5
}

public class UnitController : PrefabSingleton<UnitController>
{
    public const float TouchRange = 4;

    public int MaxWeaponRange = 20;

    public UnitFleet UnitFleetPrefab;
    public UnitGroup UnitGroupPrefab;
    public UnitBase UnitBasePrefab;
    public UnitScanner UnitScannerPrefab;

    public UnitGroupSetting UnitGroupSetting;

    [SerializeField]
    private ClockStatus status = ClockStatus.Scan;
    public ClockStatus Status => status;
    public float FieldFrameLenght = 0.5f;

    private float lastUpdateTime = 0f;
    private int turn = 0;

    private void OnEnable()
    {
        EventManager.StartListening("ClockInit", OnClockInit);
        EventManager.StartListening("Scan", OnScan);
        EventManager.StartListening("Plan", OnPlan);
    }

    private void OnDisable()
    {
        EventManager.StopListening("ClockInit", OnClockInit);
        EventManager.StopListening("Scan", OnScan);
        EventManager.StopListening("Plan", OnPlan);
    }

    void Update()
    {
        UpdateClock();
    }

    private void UpdateClock()
    {
        if (lastUpdateTime + FieldFrameLenght < Time.realtimeSinceStartup)
        {
            turn++;
            lastUpdateTime = Time.realtimeSinceStartup;
            status = (ClockStatus)(turn % EnumUtil.GetValuesList<ClockStatus>().Count);
            EventManager.TriggerEvent(status.ToString());
        }
    }

    private void OnClockInit(UnitFleet fleet)
    {
        fleet.SetDirty();

        foreach (var scanner in fleet.UnitScanners)
        {
            scanner.SetDirty();
        }

        foreach (var group in fleet.UnitGroups)
        {
            group.SetDirty();
        }
    }

    private void OnClockInit()
    {
        var time = Time.realtimeSinceStartup;

        foreach (var fleet in UnitFleet.AllRed)
        {
            OnClockInit(fleet);
        }

        foreach (var fleet in UnitFleet.AllBlue)
        {
            OnClockInit(fleet);
        }

        Debug.Log("ClockInit Time : " + (Time.realtimeSinceStartup - time));
    }

    private void OnScan()
    {
        var time = Time.realtimeSinceStartup;
        Scan(TeamName.Red);
        Scan(TeamName.Blue);
        Debug.Log("Scan Time : " + (Time.realtimeSinceStartup - time));
    }

    private void Scan(TeamName team)
    {
        List<UnitFleet> Scanning = team == TeamName.Red ? UnitFleet.AllRed : UnitFleet.AllBlue;
        List<UnitFleet> Target = team == TeamName.Red ? UnitFleet.AllBlue : UnitFleet.AllRed;

        foreach (var myFleet in Scanning)
        {
            myFleet.Touching = null;
            myFleet.Enemy = new List<UnitGroup>();

            foreach (var theirFleet in Target)
            {
                Scan(myFleet, theirFleet);
            }
        }
    }

    private void Scan (UnitFleet myFleet, UnitFleet theirFleet)
    {
        float Range = myFleet.FleetRadis + MaxWeaponRange + theirFleet.FleetRadis;
        Range *= Range;

        if (PosVector.SqDistance(myFleet.Position, theirFleet.Position) > Range)
        {
            return;
        }

        foreach (var enemy in theirFleet.UnitGroups)
        {
            if (!enemy.Alive)
            {
                continue;
            }

            if (PosVector.SqDistance(myFleet.Position, enemy.Position) < Range)
            {
                myFleet.Enemy.Add(enemy);
            }
        }

        foreach (var scanner in myFleet.UnitScanners)
        {
            if (!scanner.Alive)
            {
                continue;
            }

            Range = scanner.WeaponDistant + scanner.Radius;
            Range *= Range;

            scanner.Enemy = new List<UnitGroup>();
            for (int i = 0; i < myFleet.Enemy.Count; i++)
            {
                if (PosVector.SqDistance(scanner.Position, myFleet.Enemy[i].Position) < Range)
                    scanner.Enemy.Add(myFleet.Enemy[i]);
            }

            if (scanner.Enemy.Count == 0)
            {
                continue;
            }

            Range = scanner.WeaponDistant;
            Range *= Range;

            foreach (var group in scanner.UnitGroups)
            {
                group.Enemy = new List<UnitGroup>();
                for (int i = 0; i < scanner.Enemy.Count; i++)
                {
                    if (PosVector.SqDistance(group.Position, scanner.Enemy[i].Position) < Range)
                        group.Enemy.Add(scanner.Enemy[i]);
                }

                if (group.Enemy.Count == 0)
                {
                    continue;
                }

                group.Enemy.Sort((x, y) =>
                {
                    return PosVector.SqDistance(group.Position, x.Position).CompareTo
                    (PosVector.SqDistance(group.Position, y.Position));
                });

                group.ScanRange(Range);
            }
        }

        // Check Touch
        if (myFleet.Touching != null)
        {
            return;
        }

        foreach (var group in myFleet.UnitGroups)
        {
            foreach (var enemy in group.Enemy)
            {
                if (PosVector.SqDistance(group.Position, enemy.Position) < TouchRange * TouchRange)
                {
                    myFleet.Touching = group;
                    UnitGroupSetting.TouchEffect.Spawn(group.transform, 0, 1);
                    break;
                }
            }

            if (myFleet.Touching != null)
            {
                break;
            }
        }
    }

    private void OnPlan()
    {
        var time = Time.realtimeSinceStartup;
        Plan(TeamName.Red);
        Plan(TeamName.Blue);
        Debug.Log("Plan Time : " + (Time.realtimeSinceStartup - time));
    }

    private void Plan(TeamName team)
    {
        List<UnitFleet> Fleets = team == TeamName.Red ? UnitFleet.AllRed : UnitFleet.AllBlue;
        foreach (var myFleet in Fleets)
        {
            foreach (var scanner in myFleet.UnitScanners)
            {
                if (!scanner.Alive)
                {
                    continue;
                }

                if (scanner.Enemy.Count == 0)
                {
                    continue;
                }

                foreach (var group in scanner.UnitGroups)
                {
                    if (!group.Alive)
                    {
                        continue;
                    }

                    UnitGroup plan = group.CheckAttack();

                    if (plan == null)
                    {
                        Debug.Log("No");
                    }
                    else
                    {
                        Debug.Log("Yes");
                    }
                }
            }
        }
    }
}
