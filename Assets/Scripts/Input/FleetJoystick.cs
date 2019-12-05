using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FleetJoystick : VariableJoystick
{
    public float ForwardLimit = 1f;
    public float BackwardLimit = 0.6f;
    public float SidewayLimit = 0.3f;
    private int direction = 30;
    private float inputDirection = 0;


    private void OnEnable()
    {
        EventManager.StartListening(EventList.RotationInput, UpdateUI);
    }

    private void OnDisable()
    {
        EventManager.StopListening(EventList.RotationInput, UpdateUI);
    }

    public void UpdateUI()
    {
        direction = UnitFleet.AllRed[GroupTestingController.Instance.CurrentSelection].Angle;
        background.eulerAngles = new Vector3(background.eulerAngles.x, background.eulerAngles.y, -direction);
    }


    public override void OnDrag(PointerEventData eventData)
    {
        UpdateUI();



        cam = null;
        if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
            cam = canvas.worldCamera;

        Vector2 position = RectTransformUtility.WorldToScreenPoint(cam, background.position);
        Vector2 radius = background.sizeDelta / 2;
        input = (eventData.position - position) / (radius * canvas.scaleFactor);

        inputDirection = Mathf.Atan2(input.x, input.y) * (180/Mathf.PI);
        if (inputDirection < 0)
        {
            inputDirection += 360;
        }

        FormatInput();
        HandleInput(input.magnitude, input.normalized, radius , cam);

        handle.anchoredPosition = input * radius * handleRange;
    }

    protected override void HandleInput(float magnitude, Vector2 normalised, Vector2 radius, Camera cam)
    {
        if (magnitude > DeadZone)
        {
            input = normalised * LenghtLimit(inputDirection - direction) * Mathf.Clamp01(magnitude);
        }
        else
            input = Vector2.zero;
    }

    private float LenghtLimit (float angel)
    {
        if (angel < 0)
        {
            angel += 360;
        }

        float sin = Mathf.Sin(angel * Mathf.Deg2Rad);
        float cos = Mathf.Cos(angel * Mathf.Deg2Rad);

        if (angel < 90 || angel > 270)
        {
            return Mathf.Min(Mathf.Abs(ForwardLimit / cos), Mathf.Abs(SidewayLimit / sin));
        }
        else
        { 
            return Mathf.Min(Mathf.Abs(BackwardLimit /cos) , Mathf.Abs(SidewayLimit / sin));
        }
    }
}
