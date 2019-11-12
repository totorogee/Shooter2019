using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TilesSettingData", menuName = "GameSettingData/Tiles", order = 50)]
public class TilesSetting : ScriptableObject
{
    public int GroundWidth = 200;
    public int GroundHeight = 200;
}

