using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GroupSettingData", menuName = "GameSettingData/Group", order = 51)]
public class GroupSetting : ScriptableObject
{
    public List<Vector2Int> Pos = new List<Vector2Int>();
}

