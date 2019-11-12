using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool TeamRed = true;

    public List<Group> ControlGroups = new List<Group>();
    public int CurrectSelection = -1;


    private void OnEnable()
    {
        //EventManager.StartListening("SelectionInput" , OnSelection);
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSelection(object param)
    {
        if (ControlGroups == null || ControlGroups.Count == 0)
        {
            return;
        }

        CurrectSelection++;
        if (CurrectSelection >= ControlGroups.Count)
        {
            CurrectSelection = -1;
        }

        for (int i = 0; i < ControlGroups.Count; i++)
        {
            if (i == CurrectSelection)
            {
                ControlGroups[i].Selected = true;
            }
            else
            {
                ControlGroups[i].Selected = false;
            }
        }
    }
}
