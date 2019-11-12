using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FormationType
{
    Circle = 0,
    Arrow = 1,
    Square = 2,
    Perset=3
}

public class GroupInfo
{
    public FormationType FormationType = FormationType.Arrow;
    public float Density = 1f;
}

public class Group : Controlable
{
    public GroupSetting GroupSetting;

    public float Density = 0.2f;
    public FormationType FormationType = FormationType.Arrow;

    public GameObject SampleUnit;
    public int GroupSize = 24;

    private List<GameObject> Units = new List<GameObject>();

    protected override void OnEnable()
    {
        base.OnEnable();

        //EventManager.StartListening("Init", GroupInit);
        //EventManager.StartListening("Update", GroupUpdate);

        //EventManager.StartListening("ButtonA", OnButtonA);
        //EventManager.StartListening("ButtonB", OnButtonB);
        //EventManager.StartListening("ButtonC", OnButtonC);
    }

    private void OnDisable()
    {
        //EventManager.StopListening("Init", GroupInit);
        //EventManager.StopListening("Update", GroupUpdate);
        //EventManager.StopListening("ButtonA", OnButtonA);
        //EventManager.StopListening("ButtonB", OnButtonB);
    }

    public void OnButtonA()
    {
        if (Selected)
        {
            Density -= 0.1f * Time.deltaTime;
        }

    }

    public void OnButtonB()
    {
        if (Selected)
        {
            Density += 0.1f * Time.deltaTime;
        }
    }

    public void OnButtonC()
    {
        if (Selected)
        {
            int result = (int)FormationType;
            result++;
            if (!System.Enum.IsDefined(typeof(FormationType), result))
            {
                result = 0;
            }

            FormationType = (FormationType)result;
        }
    }



    public void GroupInit(object param)
    {
        SampleUnit.SetActive(true);
        Units = new List<GameObject>();

        for (int i = 0; i < GroupSize; i++)
        {
            GameObject Go = Instantiate(SampleUnit, transform);
            Units.Add(Go);
        }
        SampleUnit.SetActive(false);

        GroupUpdate(null);
    }

    public void GroupUpdate(object param)
    {


        while (GroupSize > Units.Count)
        {
            SampleUnit.SetActive(true);
            GameObject Go = Instantiate(SampleUnit, transform);
            Units.Add(Go);
            SampleUnit.SetActive(false);
        }

        while (GroupSize < Units.Count)
        {
            Destroy(Units[Units.Count - 1].gameObject);
            Units.RemoveAt(Units.Count - 1);
        }

        if (Selected)
        {
            Units[0].transform.localScale = SampleUnit.transform.localScale * 1.5f;
        }
        else
        {
            Units[0].transform.localScale = SampleUnit.transform.localScale;
        }

        switch (FormationType)
        {
            case FormationType.Arrow:
                GroupUpdateArrow();
                break;
            case FormationType.Circle:
                GroupUpdateCircle();
                break;
            case FormationType.Square:
                GroupUpdateSquare();
                break;
            case FormationType.Perset:
                GroupUpdatePerSet();
                break;
            default:
                break;
        }
    }

    public void GroupUpdatePerSet()
    {
        Vector2Int pos = new Vector2Int(0, 0);


        if (GroupSetting == null)
        {
            return;
        }

        for (int i = 0; i < GroupSize && i< GroupSetting.Pos.Count; i++)
        {
            pos = GroupSetting.Pos[i];
            Units[i].transform.localPosition = (new Vector3(pos.x, pos.y, 0f) * Density);

  
        }
    }

    public void GroupUpdateSquare(float ratio = 0.1f)
    {
        Vector2Int pos = new Vector2Int(0, 0);

        int numberOfCol = 0;

        for (int j = 0; j < 200; j++)
        {
            numberOfCol = j;
            int numberOfRow = Mathf.CeilToInt(numberOfCol * ratio);

            if (numberOfRow * numberOfCol >= GroupSize)
            {
                break;
            }
        }


        for (int i = 0; i < GroupSize; i++)
        {
            Units[i].transform.localPosition = (new Vector3(pos.x - numberOfCol / 2f , -pos.y, 0f) * Density) ;

            if (pos.x < numberOfCol)
            {
                pos += new Vector2Int(1, 0);
            }
            else
            {
                pos = new Vector2Int(0, pos.y + 1);
            }

        }
    }

    public void GroupUpdateArrow()
    {
        Vector2Int pos = new Vector2Int(0, 0);

        for (int i = 0; i < GroupSize; i++)
        {
            Units[i].transform.localPosition = (new Vector3((pos.x - pos.y / 2f), -pos.y, 0f) * Density);

            if (pos.x < pos.y)
            {
                pos += new Vector2Int(1, 0);
            }
            else
            {
                pos = new Vector2Int(0, pos.y + 1);
            }

        }
    }


    public void GroupUpdateCircle()
    {
        List<int> members = memberInRows(GroupSize);

        int sortedMember = 0;

        for (int i = 0; i < members.Count; i++)
        {
            int space = 1;
            float anglarSpace = 0;
            float radius = rowToRadius(members[0]) + i;

            if (i == 0)
            {
                space = members[0];
            }
            else
            {
                space = members[i - 1] + 6;
            }

            anglarSpace = 360f / space;


            for (int j = 0; j < members[i]; j++)
            {
                if ( sortedMember < Units.Count)
                {
                    float angle = anglarSpace * j;
                    Vector3 offset = new Vector3(Mathf.Sin( Mathf.Deg2Rad * angle) , Mathf.Cos(Mathf.Deg2Rad *angle), 0f);
                    offset *= radius;
                    Units[sortedMember].transform.localPosition  = offset * Density;

                    sortedMember++;
                }
            }

        }

    }

    private float rowToRadius(int members)
    {
        return members / 6f;
    }

    private List<int> memberInRows(int members)
    {
        List<int> result = new List<int>();
        int innermostSpace = 0;
        int row = 0;

        while(row < 10)
        {
            innermostSpace++;

            // only check 1,3,4,5,6
            if (innermostSpace > 6)
            {
                row++;
                innermostSpace = 1;
            }

            if (innermostSpace == 2)
            {
                continue;
            }

            int space = 0;

            result = new List<int>();
            for (int i = 0; i < row; i++)
            {
                space += innermostSpace + i * 6;
                result.Add(innermostSpace + i * 6);
            }

            if (space < members)
            {
                continue;
            }

            result[result.Count - 1] -= space - members;
            break;
        }

        return result;
    }


    protected override void OnMovementInput(object param)
    {
        if (!Selected)
        {
            return;
        }

        if (param is MovementParam)
        {
            OnMovement(((MovementParam)param).Movement);
        }
        else
        {
            Debug.LogError("Wrong Input");
        }
    }

    protected override void OnMovement(Vector2 input)
    {
        if(input.y < 0)
        {
            input = new Vector2(input.x * 0.66f, input.y * 0.5f);
        }

        Vector3 result = Target.up * input.y;


        Target.Translate(result, Space.World);

        if (input.y > Mathf.Epsilon)
        {
            OnRotate(-input.x);
        }
        else
        {
            OnCirclarMovement(input.x);
        }
    }


    protected void OnCirclarMovement(float input)
    {
        Target.RotateAround(Target.transform.position + Target.up * Density * 20f, Target.forward , input * 10f);
    }
}
