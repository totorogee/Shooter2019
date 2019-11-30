using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class GroupTestingController : MonoBehaviour
{
    public Transform DummyEnemy;

    public CinemachineTargetGroup TargetGroup;

    private bool didInit = false;
    private int allowedFleet = 3;

    public int CurrentSelection = 0;

    private Text MainText;
    private ButtonCellUI ResetButton;
    private ButtonCellUI SendEnemyButton;

    private List<ButtonCellUI> SavedFormationButtons = new List<ButtonCellUI>();
    private List<ButtonCellUI> ChangeFleetButtons = new List<ButtonCellUI>();


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



        var UIController = GroupTestingSceneUIController.Instance;

        MainText = UIController.MainText;
        ResetButton = UIController.MainMenu.AddCell();
        ResetButton.Text.text = "Reset";
        ResetButton.Button.onClick.AddListener(OnResetPressed);

        SendEnemyButton = UIController.MainMenu.AddCell();
        SendEnemyButton.Text.text = "Send Enemy";
        SendEnemyButton.Button.onClick.AddListener(OnSendEnemyPressed);

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
            GameObject Go = Instantiate(UnitController.Instance.UnitFleetPrefab.gameObject);
            UnitFleet unitFleet = Go.GetComponent<UnitFleet>();
            unitFleet.Init(0);

            Go.name = "Fleet_" + i;
            unitFleet.TheGroup.transform.position = new Vector3((i - 1f) * 30f, -20f, 0f);


            TargetGroup.m_Targets[i].target = Go.transform;
            TargetGroup.m_Targets[i].weight = 1f;
            TargetGroup.m_Targets[i].radius = 15f;

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
            var temp = UnitFleet.AllRed[CurrentSelection].TheGroup.localPosition;
            temp += (Vector3)(fullJoystick.Direction * 5f * Time.fixedDeltaTime);
            UnitFleet.AllRed[CurrentSelection].TheGroup.localPosition = temp;

            temp = UnitFleet.AllRed[CurrentSelection].TheGroup.localEulerAngles;
            temp.z -= hJoystick.Horizontal * 90f * Time.fixedDeltaTime;
            UnitFleet.AllRed[CurrentSelection].TheGroup.localEulerAngles = temp;

        }
    }

    private void UpdateUI()
    {
        for (int i = 0; i < UnitFleet.AllRed.Count; i++)
        {
            var button = SavedFormationButtons[i];
            var data = SavedFormation.SavedList[i];
            button.Text.text = data.Used ? data.Name : data.Name + " (Empty Slot)";
        }

        for (int i = 0; i < allowedFleet; i++)
        {
            var button = ChangeFleetButtons[i];
            button.Text.text = CurrentSelection == i ? "Fleet " + (i + 1) + " (Selected)" : "Fleet " + (i + 1);
        }
    }

    private void OnSlotPressed(int ID)
    {
        //LoadSlot(ID, CurrentSelection);
    }

    private void OnFleetSelection(int ID)
    {
        CurrentSelection = ID;
    }

    private void OnResetPressed()
    {

    }

    private void OnSendEnemyPressed()
    {
        Instantiate(DummyEnemy);
    }

    private void ChangeDensity()
    {
        //ChangeDensity(CurrentDensity[CurrentSelection]);
    }

    private void ChangeDensity(float density)
    {
        //int fleetID = CurrentSelection;

        //CurrentDensity[fleetID] = density;

        //foreach (var item in PlacedUnits[fleetID])
        //{
        //    item.Value.transform.localPosition = new Vector3(item.Key.x * density / 2, item.Key.y * density / 2, item.Value.transform.localPosition.z);
        //}
    }
}
