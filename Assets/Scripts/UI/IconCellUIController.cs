using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconCellUIController : CellUIController<IconCellUI>
{
    public void Select(int ID)
    {
        foreach (var item in CellsList)
        {
            item.Selected.gameObject.SetActive(false);
        }

        CellsList[ID].Selected.gameObject.SetActive(true);
        GroupSettingSceneUIController.Instance.CurrentGroupIconID = ID;

    }

    public override void Init()
    {
        base.Init();

        Select(GroupSettingSceneUIController.Instance.CurrentGroupIconID);

    }

    public void OnCellPressed(IconCellUI cellUI)
    {
        for (int i = 0; i < CellsList.Count; i++)
        {
            if (CellsList[i] == cellUI)
            {
                Select(i);
                return;
            }
        }
    }

}
