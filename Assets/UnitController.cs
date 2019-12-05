using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ClockStatus
{
    Scan = 0,
    Plan = 1,
    Shoot = 2, 
    Damage = 3,
    Move = 4,
    Scyn = 5 
}

public class UnitController : PrefabSingleton<UnitController>
{
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
        EventManager.StartListening("Scan", OnScan);
        EventManager.StartListening("Plan", OnPlan);
    }

    private void OnDisable()
    {
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

    private void OnScan()
    {
        var time = Time.realtimeSinceStartup;
        Scan(TeamName.Red);
        Scan(TeamName.Blue);
        //Debug.Log("Scan Time : " + (Time.realtimeSinceStartup - time));
    }

    private void Scan(TeamName team)
    {
        List<UnitFleet> Scanning = team == TeamName.Red ? UnitFleet.AllRed : UnitFleet.AllBlue;
        List<UnitFleet> Target = team == TeamName.Red ? UnitFleet.AllBlue : UnitFleet.AllRed;


        foreach (var redFleet in Scanning)
        {
            redFleet.Enemy = new List<UnitGroup>();

            foreach (var blueFleet in Target)
            {
                float Range = redFleet.FleetRadis + MaxWeaponRange + blueFleet.FleetRadis;

                if (PosVector.SqDistance(redFleet.Position, blueFleet.Position) < Range * Range)
                {
                    foreach (var enemy in blueFleet.UnitGroups)
                    {
                        if (!enemy.Alive)
                        {
                            continue;
                        }

                        if (PosVector.SqDistance(redFleet.Position, enemy.Position) < Range * Range)
                        { 
                            redFleet.Enemy.Add(enemy);
                        }

                    }

                    foreach (var scanner in redFleet.UnitScanners)
                    {
                        if (!scanner.Alive)
                        {
                            continue;
                        }

                        Range = scanner.WeaponDistant + scanner.Radius;
                        scanner.Enemy = new List<UnitGroup>();

                        for (int i = 0; i < redFleet.Enemy.Count; i++)
                        {
                            if (PosVector.SqDistance(scanner.Position, redFleet.Enemy[i].Position) < Range * Range)
                                scanner.Enemy.Add(redFleet.Enemy[i]);
                        }

                        scanner.Enemy.Sort((x, y) =>
                        {
                            return PosVector.SqDistance(scanner.AimPosition, x.Position).CompareTo
                            (PosVector.SqDistance(scanner.AimPosition, y.Position));
                        });
                    }
                }
            }
        }

    }

    private void OnPlan()
    {
        var time = Time.realtimeSinceStartup;
        Plan(TeamName.Red);
        Plan(TeamName.Blue);
        //Debug.Log("Plan Time : " + (Time.realtimeSinceStartup - time));
    }

    private void Plan(TeamName team)
    {
        List<UnitFleet> Fleets = team == TeamName.Red ? UnitFleet.AllRed : UnitFleet.AllBlue;
        foreach (var redFleet in Fleets )
        {
            foreach (var scanner in redFleet.UnitScanners)
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

                    UnitGroup plan = group.CheckAttack(scanner.Enemy);

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
