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


namespace SandBox
{
    public class Tile
    {

        public static Tile CenterTile;

        public bool Big = false;

        public float Distance = 0;
        public float Angle = 0;

        public List<Tile> Nearest = new List<Tile>();

        public int ID = 0;
        public int RawID = 0;

        public GameObject Go;

        public Vector2Int rawPos = new Vector2Int();

        public void OnPressed()
        {
            Debug.Log("Pressed : " + ID + " / " + Distance + " / " + Angle + " / " + rawPos);
            Go.GetComponent<SpriteRenderer>().color = Color.blue;

        }

    }
}
