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

    public PosVector StartingPos;

    public PosVector LocalPosition
    {
        get
        {
            PosVector result = (StartingPos * Mathf.RoundToInt(Fleet.Density)) * 0.5f;
            return PosVector.Rotate(result, Fleet.Angle);
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

    public UnitGroup CheckAttack(List<UnitGroup> enemyList )
    {
        if(!Alive)
        {
            return null;
        }

        for (int i = 0; i < enemyList.Count; i++)
        {
            UnitGroup enemy = enemyList[i];
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
                    LineSpawner line = (LineSpawner) weapon.WarnningEffect.NewSpawn();
                    line.SetStart(transform);
                    line.SetTarget(enemy.transform);
                    line.OnSpawn( PosVector.ToVector3( Position), PosVector.ToVector3(enemy.Position - Position), 0.1f, 1f);

                    enemy.TakeDamage(weapon.Damage);

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
}
