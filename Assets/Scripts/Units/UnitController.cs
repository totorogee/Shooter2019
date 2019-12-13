using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Diagnostics;

using Debug = UnityEngine.Debug;

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

    public bool UseTeamPhase = true;
    public TeamName Phase = TeamName.Red;

    public float FieldFrameLenght = 0.5f;

    private float lastUpdateTime = 0f;
    public  int Turn
    {
        get
        {
            int result = turn / EnumUtil.GetValuesList<ClockStatus>().Count;
            return UseTeamPhase ? result / 2 : result;
        }
    }
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


    private string PhaseName()
    {
        return (UseTeamPhase ? Phase.ToString() : "") + " " + Turn + " : " + Status.ToString() + " ";
    }


    void Update()
    {

        if (MainController.Instance.CurrentScene != SceneName.GroupSettingScene)
        UpdateClock();
    }

    private void UpdateClock()
    {
        if (lastUpdateTime + FieldFrameLenght < Time.realtimeSinceStartup)
        {
            turn++;
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            lastUpdateTime = Time.realtimeSinceStartup;
            status = (ClockStatus)(turn % EnumUtil.GetValuesList<ClockStatus>().Count);
            EventManager.TriggerEvent(status.ToString());

            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;

            if (DebugSettings.Current.ShowPhaseTime
                && Turn % DebugSettings.Current.ShowPhaseTimeInterval == 0)
            {
                Debug.Log(PhaseName() + ts.TotalMilliseconds + "ms");
            }

        }
    }

    private void OnClockInit()
    {
        if (UseTeamPhase)
        {
            Phase = Phase == TeamName.Red ? TeamName.Blue : TeamName.Red;
        }

        if (Phase == TeamName.Red || !UseTeamPhase)
        {
            foreach (var fleet in UnitFleet.AllRed)
            {
                OnClockInit(fleet);
            }
        }

        if (Phase == TeamName.Blue || !UseTeamPhase)
        {
            foreach (var fleet in UnitFleet.AllBlue)
            {
                OnClockInit(fleet);
            }
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


    private void OnScan()
    {
        if (Phase == TeamName.Red || !UseTeamPhase)
        {
            ScanTeam(TeamName.Red);
        }

        if (Phase == TeamName.Blue || !UseTeamPhase)
        {
            ScanTeam(TeamName.Blue);
        }
    }

    private void ScanTeam(TeamName team)
    {
        List<UnitFleet> Scanning = team == TeamName.Red ? UnitFleet.AllRed : UnitFleet.AllBlue;
        List<UnitFleet> Target = team == TeamName.Red ? UnitFleet.AllBlue : UnitFleet.AllRed;

        foreach (var myFleet in Scanning)
        {
            myFleet.Enemy = new List<UnitGroup>();
            myFleet.NearBy = new List<UnitGroup>();

            myFleet.EnemyTouching = new List<UnitGroup>();
            myFleet.FriendlyTouching = new List<UnitGroup>();

            foreach (var theirFleet in Target)
            {
                ScanFleet(myFleet, theirFleet);
            }

            foreach (var theirFleet in Scanning)
            {
                ScanFleet(myFleet, theirFleet);
            }
        }
    }

    private void ScanFleet(UnitFleet myFleet, UnitFleet theirFleet)
    {
        if (myFleet == theirFleet)
        {
            foreach (var group in myFleet.UnitGroups)
            {
                if (!group.Alive)
                {
                    continue;
                }
                group.OwnFleet = GetScanData(group, myFleet.UnitGroups);
            }
            return;
        }

        bool IsEnemy = myFleet.Team != theirFleet.Team;

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
                group.EnemyInRange = GetScanData(group, myFleet.Enemy, Range);
            }
            else
            {
                group.FriendlyInRange = GetScanData(group, myFleet.NearBy, Range);
            }
        }

        Range = TouchRange;
        Range *= Range;

        if (IsEnemy)
        {
            myFleet.EnemyTouching = GetTouchList(myFleet, myFleet.Enemy, Range);
        }
        else
        {
            myFleet.FriendlyTouching = GetTouchList(myFleet, myFleet.NearBy, Range);
        }
    }

    private List<UnitGroup> GetTouchList( UnitFleet fleet , List<UnitGroup> theirGroups , float sqRange = float.MaxValue)
    {
        List<UnitGroup> result = new List<UnitGroup>();

        foreach (var group in fleet.UnitGroups)
        {
            foreach (var other in theirGroups)
            {
                if (PosVector.SqDistance(group.Position, other.Position) < sqRange)
                {
                    result.Add(group);
                    // TODO MOVE to onShoot
                    //UnitGroupSetting.TouchEffect.Spawn(group.transform, 0, 1);
                    break;
                }
            }
        }
        return result;
    }

    private List<UnitScanData> GetScanData (UnitGroup myGroup , List<UnitGroup> theirGroups , float sqRange = float.MaxValue)
    {
        var ResultDatas = new List<UnitScanData>();
        foreach (var other in theirGroups)
        {
            if (PosVector.SqDistance(myGroup.Position, other.Position) < sqRange)
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
        if (Phase == TeamName.Red || !UseTeamPhase)
        {
            Plan(TeamName.Red);
        }

        if (Phase == TeamName.Blue || !UseTeamPhase)
        {
            Plan(TeamName.Blue);
        }
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
        if (Phase == TeamName.Red || !UseTeamPhase) 
        {
            Shoot(TeamName.Red);
        }

        if (Phase == TeamName.Blue || !UseTeamPhase)
        {
            Shoot(TeamName.Blue);
        }
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
