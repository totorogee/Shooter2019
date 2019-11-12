using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroupSettingSceneUIController : MonoBehaviour
{
    public static GroupSettingSceneUIController Instance;

    public ButtonCellUIController MainMenu;
    public ButtonCellUIController TopRightMenu;
    public Text MainText;

    private void OnEnable()
    {
        Instance = this;
    }
}
