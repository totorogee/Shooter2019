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
    public bool DidInit = false;

    [HideInInspector]
    public UnitFleet Fleet;
    [HideInInspector]
    public UnitGroup Group;
    [HideInInspector]
    public UnitGroupSetting Setting;

    public float HP = 100;
    public UnitBaseState UnitState = UnitBaseState.Nil;

    public PosVector GroundedPos;

    public static Color DamageColor = Color.blue;
    public static Color NormalColor = Color.red;
    public Transform Body;

    public void Init(UnitGroup group )
    {
        DidInit = true;

        Group = group;
        Fleet = group.Fleet;
        Setting = group.Setting;

        HP = Setting.HPMax / 5;
    }

    public void Update()
    {
        if (DidInit)
        {
            if (HP < Setting.HPMax && HP > 0)
            {
                HP += Setting.Recover * Time.deltaTime;
            }

            UpdateUI();
        }

    }

    public void UpdateUI()
    {
        var material = Body.GetComponent<MeshRenderer>().material;
        material.color = Color.Lerp(DamageColor, NormalColor, (float)HP / (float)Setting.HPMax);

        if( HP <= 0 && UnitState == UnitBaseState.Nil)
        {
            UnitState = UnitBaseState.Dead;
            transform.localPosition = transform.localPosition * 2f;
            Body.transform.localEulerAngles = new Vector3(0f ,0f, Random.Range(0f, 180f));
        }
    }

    public void TakeDamage(float damage)
    {
         HP -= damage;
    }
}
