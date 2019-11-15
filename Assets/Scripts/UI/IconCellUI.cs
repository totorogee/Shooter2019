using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconCellUI : CellUI
{
    public Text Text;
    public Button Button;
    public Image Image;
    public Image Selected;

    private void OnEnable()
    {
        Button.onClick.AddListener(OnPressed);
    }

    private void OnDisable()
    {
        Button.onClick.RemoveListener(OnPressed);
    }


    public void OnPressed()
    {
        ((IconCellUIController)MyController).OnCellPressed(this);
    }
}


