using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitGroupSettingData", menuName = "GameSettingData/UnitGroupSettings", order = 53)]
public class UnitGroupSetting : ScriptableObject
{

    [Header("Movement Setting")]
    public float ForwardSpeed = 1f;
    public float SidewaySpeed = 0.5f;
    public float BackwardSpeed = 0.7f;

    public float displaySize = 0.7f;

    [Header("Unit Base Setting")]

    public float HPMax = 100;
    public float Recover = 20;
    public float HPUnitSharing = 10;
    public float HPGroupSharing = 2;
    public float HPSharingMin = 20;

    [Header ("Unit Group Setting")]
    public List<WeaponSettings> Weapons = new List<WeaponSettings>();

    [Header("Unit In Group Position Setting")]
    public Transform GroupOfOne;
    public Transform GroupOfTwo;
    public Transform GroupOfThree;
    public Transform GroupOfFour;

    public List<List<Vector3>> GroupPosition;

    public Transform TestIcon;

    private void OnEnable()
    {
        if (GroupOfOne.childCount != 1 || GroupOfTwo.childCount !=2 || GroupOfThree.childCount !=3 || GroupOfFour.childCount != 4)
        {
            Debug.Log("Error on setup Group Postiton");
        }
        else
        {
            GroupPosition = new List<List<Vector3>>
            {
                GetChildPositions(GroupOfOne),
                GetChildPositions(GroupOfTwo),
                GetChildPositions(GroupOfThree),
                GetChildPositions(GroupOfFour)
            };
        }
    }

    private List<Vector3> GetChildPositions(Transform transform)
    {
        List<Vector3> result = new List<Vector3>();
        //var children = transform.GetComponentsInChildren<Transform>();

        //Debug.Log(children.Length);
        for (int i = 0; i < transform.childCount ; i++)
        {
            result.Add(transform.GetChild(i).localPosition);
        }
        return result;
    }
}