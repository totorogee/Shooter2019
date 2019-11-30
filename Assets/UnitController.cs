using UnityEngine;

public enum ClockStatus
{
    Scan = 0,
    Plan = 1,
    Shoot = 2, 
    Damage = 3,
    Move = 4,
    Scyn = 5 
}

public class UnitController : PrefabSingleton<UnitController>
{
    public UnitFleet UnitFleetPrefab;
    public UnitGroup UnitGroupPrefab;
    public UnitBase UnitBasePrefab;

    public UnitGroupSetting UnitGroupSetting;

    [SerializeField]
    private ClockStatus status = ClockStatus.Scan;
    public ClockStatus Status => status;
    public float FieldFrameLenght = 0.5f;

    private float lastUpdateTime = 0f;
    private int turn = 0;

    void Update()
    {
        UpdateClock();
    }

    private void UpdateClock()
    {
        if (lastUpdateTime + FieldFrameLenght < Time.realtimeSinceStartup)
        {
            turn++;
            lastUpdateTime = Time.realtimeSinceStartup;
            status = (ClockStatus)(turn % EnumUtil.GetValuesList<ClockStatus>().Count);
            EventManager.TriggerEvent(status.ToString());
        }
    }
}
