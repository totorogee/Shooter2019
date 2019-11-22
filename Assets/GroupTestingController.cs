using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class GroupTestingController : MonoBehaviour
{
    public CinemachineTargetGroup TargetGroup;

    private bool didInit = false;
    private int allowedFleet = 3;
    private List<Transform> FleetContainer = new List<Transform>();

    private List<Dictionary<PosVector, GameObject>> PlacedUnits = new List<Dictionary<PosVector, GameObject>>();

    public int CurrentSelection = 0;

    public List<Transform> GroupIcons = new List<Transform>();

    public const int TotalGroup = 48;

    public List<float> CurrentDensity = new List<float>();

    private Text MainText;
    private ButtonCellUI ResetButton;

    private List<ButtonCellUI> SavedFormationButtons = new List<ButtonCellUI>();
    private List<ButtonCellUI> ChangeFleetButtons = new List<ButtonCellUI>();


    private List<SavedFormation> SavedFormationList;

    private Slider densitySlider;

    private VariableJoystick fullJoystick;
    private VariableJoystick hJoystick;


    private void OnEnable()
    {
        EventManager.StartListening(EventList.FloorReady, Init);
    }

    private void Init()
    {
        didInit = true;


        TargetGroup.m_Targets = new CinemachineTargetGroup.Target[allowedFleet];

        for (int i = 0; i < allowedFleet; i++)
        {
            GameObject Go = new GameObject();
            Go.name = "Fleet_" + i;
            Go.transform.SetParent(transform);
            Go.transform.localPosition = new Vector3((i - 1f) *30f , -20f, 0f);
            FleetContainer.Add(Go.transform);

            TargetGroup.m_Targets[i].target = Go.transform;
            TargetGroup.m_Targets[i].weight = 1f;
            TargetGroup.m_Targets[i].radius = 15f;

            CurrentDensity.Add(2f);
        }

        var UIController = GroupTestingSceneUIController.Instance;

        MainText = UIController.MainText;
        ResetButton = UIController.MainMenu.AddCell();
        ResetButton.Text.text = "Reset";
        ResetButton.Button.onClick.AddListener(OnResetPressed);

        SavedFormation.Init();
        SavedFormationList = SavedFormation.SavedList;

        for (int i = 0; i < SavedFormation.AllowedCount; i++)
        {
            var button = UIController.TopRightMenu.AddCell();
            int ID = i;
            button.Button.onClick.AddListener(delegate { OnSlotPressed(ID); });
            SavedFormationButtons.Add(button);
        }

        for (int i = 0; i < allowedFleet; i++)
        {
            var button = UIController.MainMenu.AddCell();
            int ID = i;
            button.Button.onClick.AddListener(delegate { OnFleetSelection(ID); });
            button.Text.text = "Fleet " + (i + 1);
            ChangeFleetButtons.Add(button);
        }

        densitySlider = UIController.DensitySlider;
        densitySlider.onValueChanged.AddListener(ChangeDensity);

        for (int i = 0; i < allowedFleet; i++)
        {
            LoadSlot(0, i);

        }

        fullJoystick = UIController.FullJoystick;
        hJoystick = UIController.HJoystick;
    }

    private void Start()
    {




    }

    private void Update()
    {

        if (didInit)
        {
            UpdateUI();
        }

    }

    public void FixedUpdate()
    {

        if (didInit)
        {

            var temp = FleetContainer[CurrentSelection].localPosition;
            temp += (Vector3)(fullJoystick.Direction *5f* Time.fixedDeltaTime);
            FleetContainer[CurrentSelection].localPosition = temp;

            temp = FleetContainer[CurrentSelection].localEulerAngles;
            temp.z -= hJoystick.Horizontal * 90f * Time.fixedDeltaTime;
            FleetContainer[CurrentSelection].localEulerAngles = temp;

        }
    }

        private void UpdateUI()
    {
        for (int i = 0; i < SavedFormationList.Count; i++)
        {
            var button = SavedFormationButtons[i];
            var data = SavedFormationList[i];
            button.Text.text = data.Used ? data.Name : data.Name + " (Empty Slot)";
        }

        for (int i = 0; i < allowedFleet; i++)
        {
            var button = ChangeFleetButtons[i];
            button.Text.text = CurrentSelection == i ? "Fleet " + (i + 1) + " (Selected)" : "Fleet " + (i + 1);
        }
    }

    private void LoadSlot(int ID , int fleetID)
    {
        Reset(fleetID);
        Dictionary<PosVector, int> pos = SavedFormationList[ID].PositionGroupSizePair;
        PlaceUnits(pos , fleetID);

        ChangeDensity();
    }

    private void OnSlotPressed(int ID)
    {
        LoadSlot(ID, CurrentSelection);
    }

    private void OnFleetSelection(int ID)
    {
        CurrentSelection = ID;
    }

    private void PlaceUnit(PosVector pos, int size , int fleetID)
    {
        if (fleetID >= allowedFleet)
        {
            fleetID %= allowedFleet;
        }

        GameObject Go = Instantiate(GroupIcons[size], FleetContainer[fleetID] ).gameObject;
        Go.transform.localPosition = Floor.Instance.GetTileByPos(pos).TileObject.transform.localPosition;

        PlacedUnits[fleetID].Add(pos, Go);
    }

    private void PlaceUnits(Dictionary<PosVector, int> data , int fleetID)
    {
        if (fleetID >= allowedFleet)
        {
            fleetID %= allowedFleet;
        }

        foreach (var item in data)
        {
            PlaceUnit(item.Key, item.Value , fleetID);
        }
    }


    private void OnResetPressed()
    {

    }

    private void Reset(int fleetID)
    {
        for (int i = 0; i < allowedFleet; i++)
        {
            if (PlacedUnits.Count <= i)
            {
                PlacedUnits.Add(new Dictionary<PosVector, GameObject>());
            }
        }

        foreach (var item in PlacedUnits[fleetID])
        {
            Destroy(item.Value);
        }

        PlacedUnits[fleetID] = new Dictionary<PosVector, GameObject>();

    }

    private void ChangeDensity()
    {
        ChangeDensity(CurrentDensity[CurrentSelection]);
    }

    private void ChangeDensity(float density)
    {
        int fleetID = CurrentSelection;

        CurrentDensity[fleetID] = density;

        foreach (var item in PlacedUnits[fleetID])
        {
            item.Value.transform.localPosition = new Vector3(item.Key.x * density / 2, item.Key.y * density / 2, item.Value.transform.localPosition.z);
        }
    }
}
