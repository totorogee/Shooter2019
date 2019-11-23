using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Tile
{
    public GameObject TileObject;
    public int CircularID = 0;
    public int ID = 0;
    public PosVector Position = new PosVector();
    public float Distance = 0;
    public float Angle = 0;

    public bool IsMainTile = false;
    
    // nearest tiles


}

