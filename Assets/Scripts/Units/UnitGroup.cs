using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TeamName
{
    Red =0,
    Blue =1
}

[System.Serializable]
public class UnitBasePosition
{
    public List<Vector2> Position = new List<Vector2>();
}

public class UnitGroup : MonoBehaviour
{

    public static List<UnitGroup> AllRed = new List<UnitGroup>();
    public static List<UnitGroup> AllBlue = new List<UnitGroup>();

    public UnitGroupSetting Setting;

    public List<UnitGroup> LinkedGroup;

    public PosVector PositionInGroup; // TODO
    public PosVector WorldPosition; // use transform.position

    public List<UnitBase> UnitBases = new List<UnitBase>();
    [SerializeField]
    public List<UnitBasePosition> UnitBasePositions = new List<UnitBasePosition>();

    public LineRenderer AttackLine;
    public TeamName MyTeam = TeamName.Red;
    public float GroupDamagePool = 2; //
    public float GroupShareMinHP = 10; //


    private float AttackRange = 30f;
    private float AttackDamage = 10f;

    private bool didInit;

    private void OnEnable()
    {
        AttackLine.enabled = false;
    }

    private void Update()
    {
        if (!didInit)
        {
            Init();
        }

        if (didInit)
        {
            if (Time.frameCount % 40 == 0)
            {
                CheckAttack();
            }
        }


    }

    private void Init()
    {
        didInit = true;

        if(MyTeam == TeamName.Red)
        {
            AllRed.Add(this);
        }
        else
        {
            AllBlue.Add(this);
        }

        UnitBases = new List<UnitBase>(GetComponentsInChildren<UnitBase>());
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

    public void CheckAttack()
    {

        if (MyTeam == TeamName.Red)
        {
            foreach (var item in AllBlue)
            {
                if ((transform.position - item.transform.position).magnitude < AttackRange)
                {
                    DisplayLine(AttackLine, item.transform.position, 0.2f);
                    item.TakeDamage(AttackDamage);
                }
            }
        }

    }

    public void DisplayLine(LineRenderer line , Vector3 target , float time)
    {
        line.enabled = true;
        line.SetPosition(0,this.transform.position);
        line.SetPosition(1,target);
        StartCoroutine(DelayKillLine(line, time));
    }

    private IEnumerator DelayKillLine (LineRenderer line , float time)
    {
        line.enabled = true;
        yield return new WaitForSeconds(time);
        line.enabled = false;

    }

}
