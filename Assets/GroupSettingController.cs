using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public enum GroupSettingState
{
    Nil = 0,
    Setting =1,
    Done =2,
    Rejected =3,
    Ready = 4
}

public class GroupSettingController : MonoBehaviour
{
    [SerializeField]
    private Floor CurrentFloor;

    public GroupSettingState GroupSettingState = GroupSettingState.Nil;

    public int GroupSizeSelected = 2;
    public int TotalGroup = 72;

    [SerializeField]
    private List<Tile> SeletedTiles = new List<Tile>();

    private ButtonCellUI StartButton;
    private ButtonCellUI DoneButton;
    private ButtonCellUI ResetButton;
    private ButtonCellUI LoadButton;

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

        StartButton = UIController.MainMenu.AddCell();
        DoneButton = UIController.MainMenu.AddCell();
        ResetButton = UIController.MainMenu.AddCell();
        LoadButton = UIController.MainMenu.AddCell();

        StartButton.Text.text = "Press to Start";
        LoadButton.Text.text = "Load Saved Formation";

        StartButton.Button.onClick.AddListener(OnStartPressed);
        DoneButton.Button.onClick.AddListener(OnDonePressed);
        ResetButton.Button.onClick.AddListener(OnResetPressed);
        LoadButton.Button.onClick.AddListener(OnLoadPressed);

        SaveDataController.TryLoad(DataName.SavedFormation, out List<List<PosVector>> Data );
        SavedFormation.Update(Data);
        SavedFormationList = SavedFormation.SavedList;

        for (int i = 0; i < SavedFormation.AllowedCount; i++)
        {
            var button = UIController.TopRightMenu.AddCell();
            int ID = i;
            button.Button.onClick.AddListener( delegate { OnSlotPressed(ID); });
            SavedFormationButtons.Add(button);
        }
    }

    private void Update()
    {
        for (int i = 0; i < SavedFormationList.Count; i++)
        {
            var button = SavedFormationButtons[i];
            var data = SavedFormationList[i];
            button.Text.text = data.Used ? data.Name : data.Name + " (Empty Slot)";
        }

        switch (GroupSettingState)
        {
            case GroupSettingState.Setting:
                StartButton.Text.text = (TotalGroup / GroupSizeSelected - SeletedTiles.Count) +  " Positions Left";
                DoneButton.Text.text = SeletedTiles.Count == TotalGroup / GroupSizeSelected ? "Done" : "";
                ResetButton.Text.text = SeletedTiles.Count > 0 ? " Reset" : "";

                //for (int i = 0; i < SavedFormationList.Count; i++)
                //{
                //    var button = SavedFormationButtons[i];
                //    var data = SavedFormationList[i];
                //    button.Text.text = data.Used ? "Empty Slot" : data.Name;
                //}

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
        Debug.Log("Happy ? " + ID);
    }

    private void OnTilePressed(Tile tile)
    {
        if (GroupSettingState != GroupSettingState.Setting)
        {
            return;
        }

        if (tile.IsMainTile )
        {
            if (tile.TileObject.activeSelf && SeletedTiles.Count < TotalGroup / GroupSizeSelected)
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

    private void OnStartPressed()
    {
        if (GroupSettingState == GroupSettingState.Nil)
        {
            GroupSettingState = GroupSettingState.Setting;
        }
    }

    private void OnDonePressed()
    {
        if (GroupSettingState == GroupSettingState.Setting)
        {
            GroupSettingState = GroupSettingState.Done;
            Save();
        }

        OnResetPressed();
    }

    private void OnResetPressed()
    {
        foreach (var item in SeletedTiles)
        {
            item.TileObject.SetActive(true);
        }
        SeletedTiles = new List<Tile>();
    }

    private void OnLoadPressed()
    {
        OnResetPressed();
        Load();
    }

    private void Load()
    {
        if (SaveDataController.TryLoad(DataName.DefaultFormation, out List<PosVector> tempPos))
        {
            foreach (var item in tempPos)
            {
                SeletedTiles.Add(CurrentFloor.GetTileByPos(item));
            }

            foreach (var item in SeletedTiles)
            {
                item.TileObject.SetActive(false);
            }
        }
        else
        {
            Debug.Log("No Data");
        }
    }

    private void Save()
    {
        List<PosVector> tempPos = new List<PosVector>();
        foreach (var item in SeletedTiles)
        {
            tempPos.Add(item.Position);
        }

        SaveDataController.Save(DataName.DefaultFormation , tempPos);
    }
}
