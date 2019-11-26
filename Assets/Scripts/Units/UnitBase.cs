using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitBaseState
{
    Nil =0,
    Stunned =1,
    Dead =2
}

public class UnitBase : MonoBehaviour
{
    public float HP = 100; //
    public float HPMax = 100;
    public float Recover = 20; //
    public UnitBaseState UnitState = UnitBaseState.Nil;


    public static Color DamageColor = Color.blue;
    public static Color NormalColor = Color.red;
    public Transform Body;

    public void Update()
    {
        if (HP < HPMax && HP > 0)
        {
            HP += Recover * Time.deltaTime;
        }

        UpdateUI();
    }

    public void UpdateUI()
    {
        var material = Body.GetComponent<MeshRenderer>().material;
        material.color = Color.Lerp(DamageColor, NormalColor, (float)HP / (float)HPMax);

        if(HP <= 0 && UnitState == UnitBaseState.Nil)
        {
            UnitState = UnitBaseState.Dead;
            transform.localPosition = transform.localPosition * 2f;
            Body.transform.localEulerAngles = new Vector3(0f ,0f, Random.Range(0f, 180f));
        }
    }

    private void OnEnable()
    {
        HP = HPMax/5;
    }

    public void TakeDamage(float damage)
    {
        HP -= damage;
    }
}
