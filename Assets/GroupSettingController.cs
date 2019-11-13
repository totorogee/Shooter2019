using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.UI;

public enum GroupSettingState
{
    View =0,
    Setting =1,
    Done =2,
    Rejected =3
}

public class GroupSettingController : MonoBehaviour
{
    [SerializeField]
    private Floor CurrentFloor;

    public GroupSettingState GroupSettingState = GroupSettingState.Setting;

    public int GroupSizeSelected = 2;
    public const int TotalGroup = 12;

    [SerializeField]
    private List<Tile> SeletedTiles = new List<Tile>();

    private Text MainText;
    private ButtonCellUI ResetButton;

    //private List<ButtonCellUI> DefaultFormationSlots;

    private List<ButtonCellUI> SavedFormationButtons = new List<ButtonCellUI>();
    private List<SavedFormation> SavedFormationList;


    private void OnEnable()
    {
        EventManager.StartListening<Tile>(EventList.TilePressed, OnTilePressed);
    }

    private void Start()
    {
        var UIController = GroupSettingSceneUIController.Instance;

        MainText = UIController.MainText;
        ResetButton = UIController.MainMenu.AddCell();
        ResetButton.Button.onClick.AddListener(OnResetPressed);

        SavedFormation.Init();
        SavedFormationList = SavedFormation.SavedList;

        for (int i = 0; i < SavedFormation.AllowedCount; i++)
        {
            var button = UIController.TopRightMenu.AddCell();
            int ID = i;
            button.Button.onClick.AddListener( delegate { OnSlotPressed(ID); });
            SavedFormationButtons.Add(button);
        }

        GroupSettingState = GroupSettingState.Setting;
    }

    private void Update()
    {
        for (int i = 0; i < SavedFormationList.Count; i++)
        {
            var button = SavedFormationButtons[i];
            var data = SavedFormationList[i];
            button.Text.text = data.Used ? data.Name : data.Name + " (Empty Slot)";
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        switch (GroupSettingState)
        {
            case GroupSettingState.View:

                ResetButton.Text.text = "Start Edit";
                MainText.text = "Start Edit ?";

                break;

            case GroupSettingState.Setting:

                if (TotalGroup - SeletedTiles.Count > 0)
                {
                    MainText.text = "Place Unit on Screen. Units Reminded : " + (TotalGroup - SeletedTiles.Count);
                }
                else
                {
                    MainText.text = "Choose a Slot to save ";
                }

                ResetButton.Text.text = SeletedTiles.Count > 0 ? " Reset" : "";

                break;
            case GroupSettingState.Done:
                break;
            default:
                break;
        }
    }

    private void OnDisable()
    {
        EventManager.StopListening<Tile>(EventList.TilePressed, OnTilePressed);
    }

    private void OnSlotPressed( int ID)
    {
        if (TotalGroup - SeletedTiles.Count > 0)
        {
            if (SavedFormationList[ID].Used)
            {
                LoadSlot(ID);
            }
            else
            {
                Debug.Log("No Data");
            }
        }
        else
        {
            SaveSlot(ID);
        }
    }

    private void LoadSlot(int ID)
    {
        List<PosVector> pos = SavedFormationList[ID].Posistions;
        SeletedTiles = new List<Tile>();

        foreach (var item in pos)
        {
            SeletedTiles.Add(CurrentFloor.GetTileByPos(item));
        }

        foreach (var item in SeletedTiles)
        {
            item.TileObject.SetActive(false);
        }

        GroupSettingState = GroupSettingState.View;
    }

    private void SaveSlot(int ID)
    {
        if (GroupSettingState == GroupSettingState.View)
        {
            return;
        }

        List<PosVector> pos = new List<PosVector>();
        foreach (var item in SeletedTiles)
        {
            pos.Add(item.Position);
        }

        SavedFormationList[ID].Posistions = pos;
        SavedFormationList[ID].Used = true;

        SavedFormation.Update();
        OnResetPressed();

        GroupSettingState = GroupSettingState.View;
    }

    private void OnTilePressed(Tile tile)
    {
        if (GroupSettingState != GroupSettingState.Setting)
        {
            return;
        }

        if (tile.IsMainTile )
        {
            if (tile.TileObject.activeSelf && SeletedTiles.Count < TotalGroup)
            {
                SeletedTiles.Add(tile);
                tile.TileObject.SetActive(!tile.TileObject.activeSelf);
            }
            else if (SeletedTiles.Contains(tile))
            {
                SeletedTiles.Remove(tile);
                tile.TileObject.SetActive(!tile.TileObject.activeSelf);
            }
        }
    }

    private void OnResetPressed()
    {
        switch (GroupSettingState)
        {
            case GroupSettingState.View:

                GroupSettingState = GroupSettingState.Setting;
                break;

            case GroupSettingState.Setting:
            default:
                foreach (var item in SeletedTiles)
                {
                    item.TileObject.SetActive(true);
                }
                SeletedTiles = new List<Tile>();

                GroupSettingState = GroupSettingState.Setting;
                break;
        }
    }
}
