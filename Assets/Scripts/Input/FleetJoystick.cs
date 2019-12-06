using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FleetJoystick : VariableJoystick
{
    private float ForwardLimit;
    private float BackwardLimit;
    private float SidewayLimit;
    private int direction;
    private float inputDirection;


    private void OnEnable()
    {
        EventManager.StartListening(EventList.FleetRotation, UpdateSpeed);
    }

    private void OnDisable()
    {
        EventManager.StopListening(EventList.FleetRotation, UpdateSpeed);
    }

    public void UpdateSpeed()
    {
        var fleet = UnitFleet.AllRed[GroupTestingController.Instance.CurrentSelection];

        direction = fleet.Angle;
        background.eulerAngles = new Vector3(background.eulerAngles.x, background.eulerAngles.y, -direction);
        ForwardLimit = fleet.ForwardSpeed;
        BackwardLimit = fleet.BackwardSpeed;
        SidewayLimit = fleet.SidewaySpeed;
    }

    private void Update()
    {
        if (Direction != Vector2.zero)
        {
            UpdateSpeed();
        }
    }

    public override void OnDrag(PointerEventData eventData)
    {
        UpdateSpeed();

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
        HandleInput(input.magnitude, input.normalized, radius, cam);
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
