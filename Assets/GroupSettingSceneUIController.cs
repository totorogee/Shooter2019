using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupSettingSceneUIController : MonoBehaviour
{
    public static GroupSettingSceneUIController Instance;


    public ButtonCellUIController MainMenu;
    public ButtonCellUIController TopRightMenu;

    private void OnEnable()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
