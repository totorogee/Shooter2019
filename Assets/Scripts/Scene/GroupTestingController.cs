using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class GroupTestingController : MonoBehaviour
{
    public static GroupTestingController Instance;

    public Transform DummyEnemy;

    public CinemachineTargetGroup TargetGroup;

    private bool didInit = false;
    private int allowedFleet = 3;

    public int CurrentSelection = 0;

    private Text MainText;
    private ButtonCellUI SettingSceneButton;
    private ButtonCellUI ResetButton;
    private ButtonCellUI SendEnemyButton;

    private List<ButtonCellUI> SavedFormationButtons = new List<ButtonCellUI>();
    private List<ButtonCellUI> ChangeFleetButtons = new List<ButtonCellUI>();

    private Slider densitySlider;

    private FleetJoystick fullJoystick;
    private VariableJoystick hJoystick;


    private void OnEnable()
    {
        Instance = this;
        didInit = false;
        EventManager.StartListening(EventList.FloorReady, Init);
    }

    private void OnDisable()
    {
        EventManager.StopListening(EventList.FloorReady, Init);
    }

    private void Init()
    {
        didInit = true;

        TargetGroup.m_Targets = new CinemachineTargetGroup.Target[allowedFleet];

        var UIController = GroupTestingSceneUIController.Instance;

        MainText = UIController.MainText;

        SettingSceneButton = UIController.MainMenu.AddCell();
        SettingSceneButton.Text.text = "Setting Scene";
        SettingSceneButton.Button.onClick.AddListener(OnSettingScenePressed);

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

        UnitFleet.AllRed = new List<UnitFleet>();
        UnitFleet.AllBlue = new List<UnitFleet>();

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
            unitFleet.Init(i);

            Go.name = "Fleet_" + i;
            unitFleet.TheGroup.transform.position = new Vector3((i - 1f) * 50f, -20f, 0f);


            TargetGroup.m_Targets[i].target = unitFleet.TheGroup;
            TargetGroup.m_Targets[i].weight = 1f;
            TargetGroup.m_Targets[i].radius = 20f;

        }

        fullJoystick = UIController.FullJoystick;
        hJoystick = UIController.HJoystick;
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
            temp.z -= hJoystick.Horizontal * 360f * Time.fixedDeltaTime * UnitFleet.AllRed[CurrentSelection].RotationSpeed;
            UnitFleet.AllRed[CurrentSelection].TheGroup.localEulerAngles = temp;
            if (hJoystick.Horizontal != 0)
            {
                EventManager.TriggerEvent(EventList.FleetRotation);
            }
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
        MainController.Instance.LoadScene(SceneName.GroupTestingScene);
    }

    private void OnSettingScenePressed()
    {
        MainController.Instance.LoadScene(SceneName.GroupSettingScene);
    }

    private void OnSendEnemyPressed()
    {
        GameObject Go = Instantiate(DummyEnemy.gameObject);
        Go.GetComponent<UnitFleet>().Init(Random.Range(0, 3));
        Go.GetComponent<DummyEnemy>().Direction = Random.Range(165, 195);
        Go.GetComponent<DummyEnemy>().StartingPos = new Vector3(Random.Range(-30, 30), 60, 0);

    }

    private void ChangeDensity()
    {
        //ChangeDensity(CurrentDensity[CurrentSelection]);
    }

    private void ChangeDensity(float density)
    {

        UnitFleet.AllRed[CurrentSelection].ChangeDensity(density);
    }
}
