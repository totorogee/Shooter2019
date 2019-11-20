using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class GroupSettingSceneUIController : MonoBehaviour
{
    public int CurrentGroupIconID = 0;

    public TouchEvent BackGround;

    public static GroupSettingSceneUIController Instance;

    public ButtonCellUIController MainMenu;
    public ButtonCellUIController TopRightMenu;
    public IconCellUIController BottonMenu;
    public Slider DensitySlider;

    public Text MainText;

    private void OnEnable()
    {
        Instance = this;
        if (BackGround != null)
        {
            BackGround.OnTouch.AddListener(OnTouch);
        }
    }

    private void OnDisable()
    {
        if (BackGround != null)
        {
            BackGround.OnTouch.RemoveListener(OnTouch);
        }
    }

    private void OnTouch(PointerEventData eventData)
    {
        Vector2 pos = Camera.main.ScreenToWorldPoint(eventData.pressPosition);
        EventManager.TriggerEvent(EventList.OnMousePressed, pos);
    }

    private void Start()
    {
        BottonMenu.Init();
    }

}
