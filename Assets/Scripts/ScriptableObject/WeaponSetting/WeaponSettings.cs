using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponSettingData", menuName = "GameSettingData/WeaponSettings", order = 51)]
public class WeaponSettings : ScriptableObject
{
    [SerializeField]
    private string weaponName = "";
    public string WeaponName => weaponName;

    [SerializeField]
    private int maxPenetrate = 2;
    public int MaxPenetrate => maxPenetrate;

    [SerializeField]
	private int range = 20;
	public int Range => range;

    [SerializeField]
    private int minRange = 8;
    public int MinRange => minRange;

    [SerializeField]
	private int angle = 90;
	public int Angle => angle;

	[SerializeField]
	private int damage = 8;
	public int Damage => damage;

    [SerializeField]
    private int stunTime = 1;
    public int StunTime => stunTime;

    public Spawnable WarnningEffect;
    public Spawnable ShootingEffect;

    private void OnEnable()
	{

	}
}
