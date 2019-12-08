using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitBaseStatus
{
    Nil = 0,
    Stunned = 1,
    Dead = 2
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

    private Vector3 StartRotation;
    private Vector3 StartPosition;

    private float StunTime = 0;
    private Transform StunEffect;
    private Coroutine StunCoroutine;
    private float RandomSeed = 0; 

    public void Init(UnitGroup group )
    {
        DidInit = true;

        Group = group;
        Fleet = group.Fleet;
        Setting = group.Setting;

        HP = Setting.HPMax / 5;

        StartRotation = Body.localEulerAngles;
        StartPosition = Body.localPosition;
        RandomSeed = Random.Range(0f, 1f);
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

        if( HP <= 0 && UnitStatus != UnitBaseStatus.Dead) 
        {
            if (UnitStatus == UnitBaseStatus.Stunned)
            {
                OnStunEnd();
            }

            UnitStatus = UnitBaseStatus.Dead;
            transform.localPosition = transform.localPosition * 4f;
            transform.SetParent(Fleet.TheGround);
            Body.transform.localEulerAngles = new Vector3(0f ,0f, Random.Range(0f, 180f));
            OnDead();
        }

        if (HP < 0)
        {
            return;
        }

        if (StunTime > 0)
        {
            StunTime -= Time.deltaTime;
            if (UnitStatus != UnitBaseStatus.Stunned)
            {
                OnStun();
            }
            UnitStatus = UnitBaseStatus.Stunned;
        }
        else
        {
            if (UnitStatus == UnitBaseStatus.Stunned)
            {
                OnStunEnd();
            }
            UnitStatus = UnitBaseStatus.Nil;
        }


    }

    public void TakeDamage(float damage)
    {
         HP -= damage;

    }

    public void TakeStun(float time)
    {
        StunTime += time;
    }

    public void OnDead()
    {
        StartCoroutine(DelayDead(Random.Range(0.5f, 2f)));
    }

    private IEnumerator DelayDead(float delay )
    {
        yield return new WaitForSeconds(delay);
        Spawnable effect = Group.Setting.DeadEffect.Spawn();
        effect.transform.position = transform.position;
        Body.gameObject.SetActive(false);

    }

    public void OnStun()
    {
        Spawnable effect = Group.Setting.StunEffect.Spawn();
        StunEffect = effect.transform;
        StunEffect.transform.position = transform.position;
        Body.localPosition = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f);

        StunCoroutine = StartCoroutine(DelayStun());
    }

    public void OnStunEnd()
    {
        if (StunEffect != null)
        {
            StunEffect.GetComponent<Spawnable>().Kill();
            StunEffect = null;

        }

        if(StunCoroutine != null)
        {
            StopCoroutine(StunCoroutine);
            StunCoroutine = null;

        }

        Body.localEulerAngles = StartRotation;
        Body.localPosition = StartPosition;
    }

    private IEnumerator DelayStun()
    { 
        while (true)
        {
            yield return new WaitForEndOfFrame();
            var lerp = (Time.realtimeSinceStartup +RandomSeed) % 8f /8f;
            Body.localEulerAngles = new Vector3 (0, 0, Mathf.Sin(lerp * 2 * Mathf.PI) * 180f);
        }


    }


}
