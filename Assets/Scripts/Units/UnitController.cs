using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ClockStatus
{
    ClockInit = 0,
    Scan = 1,
    Plan = 2,
    Shoot = 3,
    //Move = 4,
    //Scyn = 5
}

public class UnitController : PrefabSingleton<UnitController>
{
    public const float TouchRange = 4;
    public UnitFleet UnitFleetPrefab;
    public UnitGroup UnitGroupPrefab;
    public UnitBase UnitBasePrefab;

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
        EventManager.StartListening("Shoot", OnShoot);
    }

    private void OnDisable()
    {
        EventManager.StopListening("ClockInit", OnClockInit);
        EventManager.StopListening("Scan", OnScan);
        EventManager.StopListening("Plan", OnPlan);
        EventManager.StopListening("Shoot", OnShoot);
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
        ScanTeam(TeamName.Red);
        ScanTeam(TeamName.Blue);
        Debug.Log("Scan Time : " + (Time.realtimeSinceStartup - time));
    }

    private void ScanTeam(TeamName team)
    {
        List<UnitFleet> Scanning = team == TeamName.Red ? UnitFleet.AllRed : UnitFleet.AllBlue;
        List<UnitFleet> Target = team == TeamName.Red ? UnitFleet.AllBlue : UnitFleet.AllRed;

        foreach (var myFleet in Scanning)
        {
            myFleet.Touching = null;
            myFleet.Enemy = new List<UnitGroup>();
            myFleet.NearBy = new List<UnitGroup>();

            foreach (var theirFleet in Target)
            {
                Scan(myFleet, theirFleet);
            }

            foreach (var theirFleet in Scanning)
            {
                Scan(myFleet, theirFleet);
            }
        }
    }

    private void Scan(UnitFleet myFleet, UnitFleet theirFleet)
    {
        bool IsEnemy = myFleet.Team != theirFleet.Team;
        if (myFleet == theirFleet)
        {
            ScanSelf(myFleet);
            return;
        }

        float Range = myFleet.FleetRadis + myFleet.WeaponDistant + theirFleet.FleetRadis;
        Range *= Range;

        if (PosVector.SqDistance(myFleet.Position, theirFleet.Position) > Range)
        {
            return;
        }

        Range = myFleet.FleetRadis + myFleet.WeaponDistant;
        Range *= Range;

        foreach (var other in theirFleet.UnitGroups)
        {
            if (!other.Alive)
            {
                continue;
            }

            if (PosVector.SqDistance(myFleet.Position, other.Position) < Range)
            {
                if (IsEnemy)
                {
                    myFleet.Enemy.Add(other);
                }
                else
                {
                    myFleet.NearBy.Add(other);
                }
            }
        }

        myFleet.Enemy.Sort((x, y) =>
        {
            return PosVector.SqDistance( myFleet.Position , x.Position ).
            CompareTo(PosVector.SqDistance (myFleet.Position, y.Position));
        });

        Range = myFleet.WeaponDistant;
        Range *= Range;

        foreach (var group in myFleet.UnitGroups)
        {
            if (!group.Alive)
            {
                continue;
            }

            if (IsEnemy)
            {
                group.EnemyInRange = ScanGroup(group, myFleet.Enemy, Range);
            }
            else
            {
                group.FriendlyInRange = ScanGroup(group, myFleet.NearBy, Range);
            }
        }

        // Check Touch
        if (myFleet.Touching != null || !IsEnemy)
        {
            return;
        }

        foreach (var group in myFleet.UnitGroups)
        {
            foreach (var enemy in group.EnemyInRange)
            {
                if (PosVector.SqDistance(group.Position, enemy.Target.Position) < TouchRange * TouchRange)
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

    private void ScanSelf(UnitFleet fleet)
    {
        foreach (var group in fleet.UnitGroups)
        {
            if (!group.Alive)
            {
                continue;
            }
            group.OwnFleet = ScanGroup(group, fleet.UnitGroups);
        }
    }

    private List<UnitScanData> ScanGroup (UnitGroup myGroup , List<UnitGroup> theirGroups , float Range = float.MaxValue)
    {
        var ResultDatas = new List<UnitScanData>();
        foreach (var other in theirGroups)
        {
            if (PosVector.SqDistance(myGroup.Position, other.Position) < Range)
            {
                UnitScanData targetingData = new UnitScanData
                {
                    Target = other,
                    Direction = PosVector.Angle(myGroup.Position, other.Position) - myGroup.Angle,
                    BlockedAngle = PosVector.BlockedAngle(myGroup.Position, other.Position),
                    SqDistant = PosVector.SqDistance(myGroup.Position, other.Position)
                };
                if (targetingData.Direction < 0)
                {
                    targetingData.Direction += 360;
                }

                ResultDatas.Add(targetingData);
            }
        }

        ResultDatas.Sort((x, y) =>
        {
            return x.SqDistant.CompareTo(y.SqDistant);
        });

        return ResultDatas;
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

        foreach (var fleet in Fleets)
        {
            foreach (var shooter in fleet.UnitGroups)
            {
                if (!shooter.Alive || shooter.EnemyInRange.Count == 0)
                {
                    continue;
                }

                foreach (var target in shooter.EnemyInRange)
                {
                    foreach (var enemy in shooter.EnemyInRange)
                    {
                        if (enemy == target)
                        {
                            continue;
                        }

                        if (UnitScanData.IsBlocked(target, enemy))
                        {
                            target.enemyBlockCount++;
                        }
                    }

                    foreach (var friend in shooter.FriendlyInRange)
                    {
                        if (UnitScanData.IsBlocked(target, friend))
                        {
                            target.friendlyBlockCount++;
                        }
                    }

                    foreach (var friend in shooter.OwnFleet)
                    {
                        if (UnitScanData.IsBlocked(target, friend))
                        {
                            target.ownFleetBlockCount++;
                        }
                    }
                }
                shooter.CheckAttack();
            }
        }
    }

    private void OnShoot()
    {
        var time = Time.realtimeSinceStartup;
        Shoot(TeamName.Red);
        Shoot(TeamName.Blue);
        Debug.Log("Shoot Time : " + (Time.realtimeSinceStartup - time));
    }

    private void Shoot(TeamName team)
    {
        List<UnitFleet> Fleets = team == TeamName.Red ? UnitFleet.AllRed : UnitFleet.AllBlue;
        foreach (var fleet in Fleets)
        {
            foreach (var group in fleet.UnitGroups)
            {
                if (!group.Alive)
                {
                    continue;
                }

                // TODO
            }
        }
    }
}
