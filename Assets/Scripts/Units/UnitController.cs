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
    //public UnitScanner UnitScannerPrefab;

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

        //foreach (var scanner in fleet.UnitScanners)
        //{
        //    scanner.SetDirty();
        //}

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

    private void ScanSelf (UnitFleet fleet)
    {
        foreach (var group in fleet.UnitGroups)
        {
            if ( !group.Alive )
            {
                continue;
            }

            group.OwnFleet = new List<UnitScanData>();
            foreach (var other in fleet.UnitGroups)
            {
                if (group == other || !other.Alive)
                {
                    continue;
                }

                PosVector groupPos = group.StartingPos * Mathf.RoundToInt(fleet.Density) * 0.5f;
                PosVector otherPos = other.StartingPos * Mathf.RoundToInt(fleet.Density) * 0.5f;

                UnitScanData targetingData = new UnitScanData
                {
                    Target = other,
                    Direction = PosVector.Angle(groupPos, otherPos) - fleet.Angle,
                    BlockedAngle = PosVector.BlockedAngle(groupPos, otherPos),
                    SqDistant = PosVector.SqDistance(groupPos, otherPos)
                };
                if (targetingData.Direction < 0)
                {
                    targetingData.Direction += 360;
                }

                group.OwnFleet.Add(targetingData);


            }
            group.OwnFleet.Sort((x, y) =>
            {
                return x.SqDistant.CompareTo(y.SqDistant);
            });
        }
    }

    private void Scan (UnitFleet myFleet, UnitFleet theirFleet)
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
            return;

        foreach (var other in theirFleet.UnitGroups)
        {
            if (!other.Alive)
                continue;

            if (PosVector.SqDistance(myFleet.Position, other.Position) < Range)
            {
                if (IsEnemy)
                    myFleet.Enemy.Add(other);
                else
                    myFleet.NearBy.Add(other);
            }

        }

        // TODO : Remove Scanner ?
        //foreach (var scanner in myFleet.UnitScanners)
        //{
        //    if (!scanner.Alive)
        //        continue;

        //    Range = scanner.WeaponDistant + scanner.Radius;
        //    Range *= Range;

        //    if (IsEnemy)
        //    {
        //        scanner.Enemy = new List<UnitGroup>();
        //        foreach (var other in myFleet.Enemy)
        //        {
        //            if (PosVector.SqDistance(scanner.Position, other.Position) < Range)
        //                scanner.Enemy.Add(other);
        //        }

        //        if (scanner.Enemy.Count == 0)
        //            continue;
        //    }
        //    else
        //    {
        //        scanner.NearBy = new List<UnitGroup>();
        //        foreach (var other in myFleet.NearBy)
        //        {
        //            if (PosVector.SqDistance(scanner.Position, other.Position) < Range)
        //                scanner.NearBy.Add(other);
        //        }

        //        if (scanner.NearBy.Count == 0)
        //            continue;
        //    }
        // TODO : Remove Scanner ?

            Range = myFleet.WeaponDistant;
            Range *= Range;

            foreach (var group in myFleet.UnitGroups)
            {


                if (!group.Alive)
                    continue;

                if (IsEnemy)
                {
                    group.EnemyInRange = new List<UnitScanData>();
                    foreach (var other in myFleet.Enemy)
                    {
                        if (PosVector.SqDistance(group.Position, other.Position) < Range)
                        {
                            UnitScanData targetingData = new UnitScanData
                            {
                                Target = other,
                                Direction = PosVector.Angle(group.Position, other.Position) - myFleet.Angle,
                                BlockedAngle = PosVector.BlockedAngle(group.Position, other.Position),
                                SqDistant = PosVector.SqDistance(group.Position, other.Position)
                            };
                            if (targetingData.Direction < 0)
                            {
                                targetingData.Direction += 360;
                            }

                            group.EnemyInRange.Add(targetingData);
                        }
                    }

                    if (group.EnemyInRange.Count == 0)
                        continue;

                    group.EnemyInRange.Sort((x, y) =>
                    {
                        return x.SqDistant.CompareTo(y.SqDistant);
                    });
                }
                else  
                {
                    group.FriendlyInRange = new List<UnitScanData>();
                    foreach (var other in myFleet.NearBy)
                    {
                        if (PosVector.SqDistance(group.Position, other.Position) < Range)
                        {
                            UnitScanData targetingData = new UnitScanData
                            {
                                Target = other,
                                Direction = PosVector.Angle(group.Position, other.Position) - myFleet.Angle,
                                BlockedAngle = PosVector.BlockedAngle(group.Position, other.Position),
                                SqDistant = PosVector.SqDistance(group.Position, other.Position)
                            };

                            if (targetingData.Direction < 0)
                            {
                                targetingData.Direction += 360;
                            }

                            group.FriendlyInRange.Add(targetingData);
                        }
                    }

                    if (group.FriendlyInRange.Count == 0)
                        continue;

                    group.FriendlyInRange.Sort((x, y) =>
                    {
                        return x.SqDistant.CompareTo(y.SqDistant);
                    });
                }
            }
        //}

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
                        if ( enemy == target)
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
            //foreach (var scanner in fleet.UnitScanners)
            //{
            //    if (!scanner.Alive)
            //    {
            //        continue;
            //    }

            //    if (scanner.Enemy.Count == 0)
            //    {
            //        continue;
            //    }

                foreach (var group in fleet.UnitGroups)
                {
                    if (!group.Alive)
                    {
                        continue;
                    }

                    // TODO
                }
            //}
        }
    }
}
