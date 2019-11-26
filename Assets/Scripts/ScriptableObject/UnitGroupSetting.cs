using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitGroupSettingData", menuName = "GameSettingData/UnitGroupSettings", order = 53)]
public class UnitGroupSetting : ScriptableObject
{

    [Header ("Unit Base Setting")]

    public float HP = 100;
    public float Recover = 20;
    public float HPUnitSharing = 10;
    public float HPGroupSharing = 2;
    public float HPSharingMin = 20;

    // moving speed x 4

    [Header ("Unit Group Setting")]

    public List<WeaponSettings> Weapons = new List<WeaponSettings>();

    [SerializeField]
    private int groundWidth = 201;
    public int GroundWidth => groundWidth;

    [SerializeField]
    private int groundHeight = 201;
    public int GroundHeight => groundHeight;

    [SerializeField]
    private int colorRadis = 5;
    public int ColorRadis => colorRadis;

    [SerializeField]
    private Color centerColor = Color.blue;
    public Color CenterColor => centerColor;

    [SerializeField]
    private Color edgeColor = Color.gray;
    public Color EdgeColor => edgeColor;

    [SerializeField]
    private bool showMainTiles = true;
    public bool ShowMainTiles => showMainTiles;

    [SerializeField]
    private bool hideSmallTiles = false;
    public bool HideSmallTiles => hideSmallTiles;

    private void OnEnable()
    {
        if (groundWidth % 2 == 0)
        {
            groundWidth += 1;
        }

        if (groundHeight % 2 == 0)
        {
            groundHeight += 1;
        }
    }
}