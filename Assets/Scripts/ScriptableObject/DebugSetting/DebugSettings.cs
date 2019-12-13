using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DebugSettingData", menuName = "GameSettingData/DebugSettings", order = 54)]
public class DebugSettings : ScriptableObject
{
    public bool ShowPhaseTime = true;
    public int ShowPhaseTimeInterval = 10;

    public static DebugSettings Current
    {
        get
        {
            return MainController.Instance.DebugSettings;
        }
    }
}
