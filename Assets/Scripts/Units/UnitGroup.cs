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
    public List<UnitBase> Bases = new List<UnitBase>();

    public static List<UnitGroup> AllRed = new List<UnitGroup>();
    public static List<UnitGroup> AllBlue = new List<UnitGroup>();

    public LineRenderer AttackLine;


    public TeamName MyTeam = TeamName.Red;


    public float GroupDamagePool = 2;
    public float GroupShareMinHP = 10;


    private float AttackRange = 20f;
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

        Bases = new List<UnitBase>(GetComponentsInChildren<UnitBase>());
    }

    public void TakeDamage(float damage)
    {
        if (Bases.Count == 0)
        {
            Debug.Log("Error");
        }

        foreach (var item in Bases)
        {
            item.TakeDamage(damage / Bases.Count);
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
