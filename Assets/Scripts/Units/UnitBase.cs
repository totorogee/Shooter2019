using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitBaseStatus
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
    public UnitBaseStatus UnitStatus = UnitBaseStatus.Nil;

    public PosVector GroundedPos;

    public static Color DamageColor = Color.white;
    public static Color RedColor = Color.red;
    public static Color BlueColor = Color.blue;
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
        material.color = Color.Lerp(DamageColor, Fleet.Team== TeamName.Red ? RedColor : BlueColor , (float)HP / (float)Setting.HPMax);

        if( HP <= 0 && UnitStatus == UnitBaseStatus.Nil)
        {
            UnitStatus = UnitBaseStatus.Dead;
            transform.localPosition = transform.localPosition * 4f;
            transform.SetParent(Fleet.TheGround);
            Body.transform.localEulerAngles = new Vector3(0f ,0f, Random.Range(0f, 180f));
        }
    }

    public void TakeDamage(float damage)
    {
         HP -= damage;
    }
}
