using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InputController : MonoBehaviour
{
    public Camera MainCamera;

    private void Start()
    {
        if (MainCamera == null)
        {
            MainCamera = Camera.main;
        }
    }

    private void Update()
    {
        CheckMousePressed();
    }

    private void CheckMousePressed()
    {
        if (Input.GetMouseButtonDown(0))
        {
            EventManager.TriggerEvent(EventList.OnMousePressed, new PosVector ( MainCamera.ScreenToWorldPoint( Input.mousePosition)));
        }
    }
}

//namespace SandBox
//{
//    public enum InputType
//    {
//        GetKey = 0,
//        GetKeyDown = 1,
//        GetKeyUp = 2
//    }

//    public class InputCode
//    {
//        public KeyCode KeyCode = KeyCode.None;
//        public InputType InputType = InputType.GetKey;
//    }

//    public class InputSetting
//    {
//        public static KeyCode Up = KeyCode.W;
//        public static KeyCode Down = KeyCode.S;

//        public static KeyCode Left = KeyCode.A;
//        public static KeyCode Right = KeyCode.D;

//        public static KeyCode RightTurn = KeyCode.None;
//        public static KeyCode LeftTurn = KeyCode.None;

//        public static KeyCode ButtonA = KeyCode.Q;
//        public static KeyCode ButtonB = KeyCode.E;
//        public static KeyCode ButtonC = KeyCode.X;
//        public static KeyCode SelectionNext = KeyCode.Space;

//        public static KeyCode ButtonMark = KeyCode.M;
//        public static KeyCode ButtonMarkRemove = KeyCode.N;
//    }

    //public class InputController : MonoBehaviour
    //{
    //    public Camera MainCamera;

    //    // Start is called before the first frame update
    //    void Start()
    //    {
    //        if (MainCamera == null)
    //        {
    //            MainCamera = Camera.main;
    //        }
    //    }

    //    Update is called once per frame
    //    void Update()
    //    {
    //        CheckMovementInput();
    //        //CheckSelection();
    //        //CheckGeneralInput();
    //        CheckMousePressed();
    //        CheckMarkInput();
    //    }

    //    private void CheckMousePressed()
    //    {
    //        if (Input.GetMouseButtonDown(0))
    //        {
    //            EventManager.TriggerEvent("MousePressed", MainCamera.ScreenToWorldPoint(Input.mousePosition));
    //        }
    //    }

    //    private void CheckMarkInput()
    //    {
    //        if (Input.GetKeyDown(InputSetting.ButtonMark))
    //        {
    //            EventManager.TriggerEvent("GroupMark", MainCamera.ScreenToWorldPoint(Input.mousePosition));
    //        }
    //        if (Input.GetKeyDown(InputSetting.ButtonMarkRemove))
    //        {
    //            EventManager.TriggerEvent("GroupMarkRemove", MainCamera.ScreenToWorldPoint(Input.mousePosition));
    //        }
    //    }

    //    //private void CheckGeneralInput()
    //    //{
    //    //    if (Input.GetKey(InputSetting.ButtonA))
    //    //    {
    //    //        EventManager.TriggerEvent("ButtonA", null);
    //    //    }

    //    //    if (Input.GetKey(InputSetting.ButtonB))
    //    //    {
    //    //        EventManager.TriggerEvent("ButtonB", null);
    //    //    }

    //    //    if (Input.GetKeyDown(InputSetting.ButtonC))
    //    //    {
    //    //        EventManager.TriggerEvent("ButtonC", null);
    //    //    }
    //    //}
    //    //private void CheckSelection()
    //    //{
    //    //    if (Input.GetKeyDown(InputSetting.SelectionNext))
    //    //    {
    //    //        EventManager.TriggerEvent("SelectionInput", null);
    //    //    }
    //    //}

    //    private void CheckMovementInput()
    //    {
    //        Controlable.MovementParam param = new Controlable.MovementParam();
    //        bool moved = false;

    //        if (Input.GetKey(InputSetting.Left))
    //        {
    //            param.Movement.x -= Time.deltaTime;
    //            moved = true;
    //        }

    //        if (Input.GetKey(InputSetting.Right))
    //        {
    //            param.Movement.x += Time.deltaTime;
    //            moved = true;
    //        }

    //        if (Input.GetKey(InputSetting.Up))
    //        {
    //            param.Movement.y += Time.deltaTime;
    //            moved = true;
    //        }

    //        if (Input.GetKey(InputSetting.Down))
    //        {
    //            param.Movement.y -= Time.deltaTime;
    //            moved = true;
    //        }

    //        if (Input.GetKey(InputSetting.LeftTurn))
    //        {
    //            param.Rotation += Time.deltaTime;
    //            moved = true;
    //        }

    //        if (Input.GetKey(InputSetting.RightTurn))
    //        {
    //            param.Rotation -= Time.deltaTime;
    //            moved = true;
    //        }

    //        if (moved)
    //        {
    //            EventManager.TriggerEvent("MovementInput", param);
    //        }
    //    }
    //}
