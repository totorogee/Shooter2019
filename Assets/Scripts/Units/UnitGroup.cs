using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingData
{
    UnitGroup Target;

    float Direction;
    float Angle;
    float SqDistant;
}

public enum UnitGroupActionStatus
{
    Cleaned =0,
    ScannedEnemy =1,
    ScannedBlocks = 2,
    PlannedAttact =3,
    Moved =4
}

public class UnitGroup : MonoBehaviour
{
    public static List<UnitGroup> AllRed = new List<UnitGroup>();
    public static List<UnitGroup> AllBlue = new List<UnitGroup>();

    public UnitGroupSetting Setting;
    public List<UnitGroup> LinkedGroup; // TODO Nearest group

    public List<UnitGroup> Enemy = new List<UnitGroup>();

    public List<TargetingData> EnemyBlocks = new List<TargetingData>();
    public List<TargetingData> FriendlyBlocks = new List<TargetingData>();

    public UnitFleet Fleet;
    public PosVector StartingPos;

    public PosVector LocalPosition
    {
        get
        {
            PosVector result = (StartingPos * Mathf.RoundToInt(Fleet.Density)) * 0.5f;
            return result.Rotate(Fleet.Angle);
        }
    }
    public PosVector Position
    {
        get
        {
            return LocalPosition + Fleet.Position;
        }
    }

    public bool Alive
    {
        get
        {
            foreach (var item in UnitBases)
            {
                if(item.UnitStatus == UnitBaseStatus.Nil || item.UnitStatus == UnitBaseStatus.Stunned)
                {
                    return true;
                }
            }
            return false;
        }
    }

    public bool IsStun
    {
        get
        {
            foreach (var item in UnitBases)
            {
                if (item.UnitStatus == UnitBaseStatus.Stunned)
                {
                    return true;
                }
            }
            return false;
        }
    }

    public List<UnitBase> UnitBases = new List<UnitBase>();

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

    public UnitGroup CheckAttack()
    {
        Debug.Log("TODO : Attack with block info");

        if(!Alive)
        {
            return null;
        }

        if (Enemy.Count != 0)
        {
            Debug.Log(Enemy.Count + " " + Fleet.Team);
        }

        for (int i = 0; i < Enemy.Count; i++)
        {
            UnitGroup enemy = Enemy[i];
            float Angle =  PosVector.Angle(Position, enemy.Position) - Fleet.Angle;
            if (Angle < 0)
            {
                Angle += 360;
            }
            float SqDistant = PosVector.SqDistance(Position, enemy.Position);

            for (int j = 0; j < Setting.Weapons.Count; j++)
            {
                WeaponSettings weapon = Setting.Weapons[j];
                if  ( ( SqDistant < weapon.Range * weapon.Range)
                    && (SqDistant > weapon.MinRange * weapon.MinRange)
                    && (Angle < weapon.Angle || 360 - Angle < weapon.Angle)
                    && enemy.Alive )
                {

                    if (Fleet.Team == TeamName.Red)
                    {
                        weapon.WarnningEffect.Spawn(transform, enemy.transform, 0f, 0.2f);
                        weapon.ShootingEffect.Spawn(transform, enemy.transform, 0.2f, 0.1f);
                    }
                    else
                    {
                        weapon.WarnningEffect.Spawn(transform, enemy.transform, 0.4f, 0.2f);
                        weapon.ShootingEffect.Spawn(transform, enemy.transform, 0.6f, 0.1f);
                    }

                    enemy.TakeDamage(weapon.Damage);
                    enemy.TakeStun(weapon.StunTime);

                    return enemy;
                }
            }
        }
        return null;
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

    public void TakeStun(float time)
    {
        if (UnitBases.Count == 0)
        {
            Debug.Log("Error");
        }

        foreach (var item in UnitBases)
        {
            item.TakeStun(time);
        }
    }

    public void ScanRange(float range )
    {
        Debug.Log("  TODO : Get all Block info ");
    }
}
