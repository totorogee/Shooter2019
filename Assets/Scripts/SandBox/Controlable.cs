using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controlable : MonoBehaviour
{
    public bool Selected = false;
    public Transform Target;
    public int PlayerNumber = 0;

    protected virtual void OnEnable()
    {
        //EventManager.StartListening("MovementInput" , OnMovementInput);
    }


    // Start is called before the first frame update
    protected virtual void Start()
    {
        if (Target == null)
        {
            Target = transform;
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    protected virtual void OnMovementInput(object param)
    {
        if (!Selected)
        {
            return;
        }

        if ( param is MovementParam)
        {
            OnMovement(((MovementParam)param).Movement);
            OnRotate(((MovementParam)param).Rotation);
        }
        else
        {
            Debug.LogError("Wrong Input");
        }
    }

    protected virtual void OnMovement(Vector2 input)
    {
        Vector3 result = Target.right * input.x + Target.up * input.y;
        Target.Translate(result,Space.World);
    }

    protected virtual void OnRotate(float input)
    {
        Target.eulerAngles = new Vector3(Target.eulerAngles.x, Target.eulerAngles.y, Target.eulerAngles.z + input * 60f);
    }

    public struct MovementParam
    {
        public Vector2 Movement;
        public float Rotation;
    }
}
