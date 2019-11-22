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
    private Transform UnitMarksContainer;

    private Dictionary<PosVector, GameObject> PlacedMarks = new Dictionary<PosVector, GameObject>();
    private Dictionary<PosVector, int> CurrentSelection = new Dictionary<PosVector, int>();

    public List<Transform> GroupIcons = new List<Transform>();

    [SerializeField]
    private Floor CurrentFloor;

    public GroupSettingState GroupSettingState = GroupSettingState.Setting;

    public const int TotalGroup = 48;

    public float CurrentDensity = 2;

    private Text MainText;
    private ButtonCellUI ResetButton;

    private List<ButtonCellUI> SavedFormationButtons = new List<ButtonCellUI>();
    private List<SavedFormation> SavedFormationList;

    private Slider densitySlider;


    private void OnEnable()
    {
        EventManager.StartListening<Tile>(EventList.TilePressed, OnTilePressed);
    }

    private void OnDisable()
    {
        EventManager.StopListening<Tile>(EventList.TilePressed, OnTilePressed);
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

        densitySlider = UIController.DensitySlider;
        densitySlider.onValueChanged.AddListener(ChangeDensity);

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

                densitySlider.interactable = true;

                ResetButton.Text.text = "Start Edit ? ";
                MainText.text = "Start Edit / Preview with different density";

                break;

            case GroupSettingState.Setting:

                densitySlider.interactable = false;
                ChangeDensity(2);

                if (TotalGroup - UnitPlaced() > 0)
                {
                    MainText.text = "Place Unit on Screen. Units Reminded : " + (TotalGroup - UnitPlaced());
                }
                else
                {
                    MainText.text = "Choose a Slot to save ";
                }

                ResetButton.Text.text = UnitPlaced() > 0 ? " Reset" : "";

                break;
            case GroupSettingState.Done:

                densitySlider.interactable = false;
                ChangeDensity(2);
                break;
            default:
                break;
        }
    }



    private void OnSlotPressed( int ID)
    {
        if (TotalGroup - UnitPlaced() > 0 || GroupSettingState == GroupSettingState.View)
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

    private void PlaceMark(PosVector pos , int size )
    {
        GameObject Go = Instantiate(GroupIcons[size], UnitMarksContainer).gameObject;
        Go.transform.localPosition = Floor.Instance.GetTileByPos(pos).TileObject.transform.localPosition;

        PlacedMarks.Add(pos , Go);
        CurrentSelection.Add(pos , size);
    }

    private void PlaceMarks(Dictionary<PosVector, int> data)
    {
        foreach (var item in data)
        {
            PlaceMark(item.Key, item.Value);
        }
    }

    private void LoadSlot(int ID)
    {

        Reset();

        Dictionary<PosVector, int> pos = SavedFormationList[ID].PositionGroupSizePair;

        PlaceMarks(pos);
        GroupSettingState = GroupSettingState.View;
    }

    private void SaveSlot(int ID)
    {
        if (GroupSettingState == GroupSettingState.View)
        {
            return;
        }

        SavedFormationList[ID].PositionGroupSizePair = CurrentSelection;
        SavedFormationList[ID].Used = true;

        SavedFormation.Update();
        Reset();

        GroupSettingState = GroupSettingState.View;
    }

    private void OnTilePressed(Tile tile)
    {
        PosVector pos = tile.Position;
        int size = GroupSettingSceneUIController.Instance.CurrentGroupIconID;

        if (GroupSettingState != GroupSettingState.Setting)
        {
            return;
        }

        if (tile.IsMainTile )
        {

            if ( !CurrentSelection.ContainsKey(pos) && UnitPlaced() + size +1 <= TotalGroup)
            {
                PlaceMark(pos, size);
            }
            else if (CurrentSelection.ContainsKey(pos))
            {
                CurrentSelection.Remove(pos);
                Destroy(PlacedMarks[pos]);
                PlacedMarks.Remove(pos);
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
                Reset();
                break;
        }
    }

    private void Reset()
    {
        foreach (var item in PlacedMarks)
        {
            Destroy(item.Value);
        }

        PlacedMarks = new Dictionary<PosVector, GameObject>();
        CurrentSelection = new Dictionary<PosVector, int>();

        GroupSettingState = GroupSettingState.Setting;
    }

    private int UnitPlaced()
    {
        int result = 0;

        foreach (var item in CurrentSelection)
        {
            result += item.Value+1;
        }

        return result;
    }

    private void ChangeDensity(float density)
    {
        if (density == CurrentDensity)
        {
            return;
        }

        CurrentDensity = density;

        foreach (var item in PlacedMarks)
        {
            item.Value.transform.localPosition = new Vector3(item.Key.x * density / 2, item.Key.y * density / 2, item.Value.transform.localPosition.z);
        }
    }
}
