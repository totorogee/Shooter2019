using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitScanData
{
    public UnitGroup Target;
    public float Direction;
    public float BlockedAngle;
    public float SqDistant;

    public int enemyBlockCount = 0;
    public int friendlyBlockCount = 0;
    public int ownFleetBlockCount = 0;

    public static bool IsBlocked(UnitScanData target, UnitScanData other)
    {
        float angle = target.Direction - other.Direction;

        if (angle > 180)
        {
            angle -= 360;
        }

        if (Mathf.Abs(angle) > other.BlockedAngle)
        {
            return false;
        }

        return target.SqDistant >= other.SqDistant;
    }
}

public enum UnitGroupActionStatus // TODO
{
    Cleaned = 0,
    ScannedEnemy = 1,
    ScannedBlocks = 2,
    PlannedAttact = 3,
    Moved = 4
}

public class UnitGroup : MonoBehaviour
{
    public static List<UnitGroup> AllRed = new List<UnitGroup>();
    public static List<UnitGroup> AllBlue = new List<UnitGroup>();

    public UnitGroupSetting Setting;
    public List<UnitGroup> LinkedGroup; // TODO Nearest group

    public List<UnitScanData> EnemyInRange = new List<UnitScanData>();
    public List<UnitScanData> FriendlyInRange = new List<UnitScanData>();
    public List<UnitScanData> OwnFleet = new List<UnitScanData>();

    public UnitFleet Fleet;
    public PosVector StartingPos;

    public PosVector LocalPosition
    {
        get
        {
            if (localPosition == new PosVector(0, 0))
            {
                PosVector result = StartingPos * Mathf.RoundToInt(Fleet.Density) * 0.5f;
                localPosition = result.Rotate(Fleet.Angle);
            }
            return localPosition;
        }
    }
    private PosVector localPosition;

    public PosVector Position
    {
        get
        {
            if (position == new PosVector(0, 0))
            {
                position = LocalPosition + Fleet.Position;
            }
            return position;
        }
    }
    private PosVector position;

    public bool Alive
    {
        get
        {
            foreach (var item in UnitBases)
            {
                if (item.UnitStatus == UnitBaseStatus.Nil || item.UnitStatus == UnitBaseStatus.Stunned)
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

    public void SetDirty()
    {
        position = new PosVector(0, 0);
        localPosition = new PosVector(0, 0);
    }

    public void Init(UnitFleet fleet, PosVector starting, int size)
    {
        Fleet = fleet;
        StartingPos = starting;
        if (Setting == null)
        {
            Setting = UnitController.Instance.UnitGroupSetting;
        }

        didInit = true;

        if (Fleet.Team == TeamName.Red)
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

        if (!Alive)
        {
            return null;
        }

        for (int j = 0; j < Setting.Weapons.Count; j++)
        {
            WeaponSettings weapon = Setting.Weapons[j];

            for (int i = 0; i < EnemyInRange.Count; i++)
            {
                UnitGroup enemy = EnemyInRange[i].Target;
                if (!enemy.Alive)
                {
                    continue;
                }

                float SqDistant = EnemyInRange[i].SqDistant;
                if (SqDistant > weapon.Range * weapon.Range)
                {
                    break;
                }

                float Angle = EnemyInRange[i].Direction;
                int blockage = EnemyInRange[i].enemyBlockCount + EnemyInRange[i].friendlyBlockCount + EnemyInRange[i].ownFleetBlockCount;

                if ((SqDistant > weapon.MinRange * weapon.MinRange)
                    && (Angle < weapon.Angle || 360 - Angle < weapon.Angle)
                    && (blockage <= weapon.MaxPenetrate))
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

    public void ScanRange(float range)
    {
        Debug.Log("  TODO : Get all Block info ");
    }
}
