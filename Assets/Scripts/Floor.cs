using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{

    public bool AccpetInput = true;
    public Tile CenterTile;

    [SerializeField]
    private FloorSetting floorSetting;

    [SerializeField]
    private SpriteRenderer tileSprite;

    public List<Tile> TileList = new List<Tile>();
    public List<Tile> CircularTileList = new List<Tile>();

    public static Dictionary<int, Dictionary<PosVector, PosVector>> RotatedPosition = new Dictionary<int, Dictionary<PosVector, PosVector>>();

    private void OnEnable()
    {
        EventManager.StartListening<PosVector>(EventList.OnMousePressed , MousePressed);

    }

    private void OnDisable()
    {
        EventManager.StopListening<PosVector>(EventList.OnMousePressed, MousePressed);

    }

    private void Start()
    {
        float startingTime = Time.realtimeSinceStartup;
        PlaceTiles();
        Debug.Log("Place Tile Time : " + (Time.realtimeSinceStartup - startingTime));
        startingTime = Time.realtimeSinceStartup;


        SetCircularTileList();
        Debug.Log("Set Circular Tile Time : " + (Time.realtimeSinceStartup - startingTime));

    }

    private void Update()
    {

    }

    private void MousePressed(PosVector position)
    {
        if (AccpetInput)
        {
            EventManager.TriggerEvent(EventList.TilePressed, GetTileByPos(position));
        }
    }


    private void PlaceTiles()
    {
        TileList = new List<Tile>();
        GameObject TilesSample = tileSprite.gameObject;

        TilesSample.SetActive(true);
        int currentID = 0;
        for (int y = -(floorSetting.GroundHeight-1)/2 ; y <= (floorSetting.GroundHeight-1)/2; y++)
        {
            for (int x = -(floorSetting.GroundWidth-1)/2; x <= (floorSetting.GroundWidth-1)/2; x++)
            { 
                GameObject Go = Instantiate(TilesSample, transform);
                Go.transform.localPosition = new Vector3(x, y, 0);

                bool isMainTile = y % 2 == 0 && (y / 2 + x) % 2 == 0;
                float angle = Mathf.Atan2(-x, -y) * 180 / Mathf.PI + 180;
                if (Mathf.Abs(angle - 360) <= float.Epsilon)
                {
                    angle = 0f;
                }

                Tile currentTile = new Tile
                {
                    TileObject = Go,
                    ID = currentID,
                    Position = new PosVector(x, y),
                    Distance = new Vector2Int(x, y).magnitude,
                    Angle = angle,
                    IsMainTile = isMainTile
                };



                var color = Go.GetComponent<SpriteRenderer>().color;

                color *= Mathf.CeilToInt(currentTile.Distance / floorSetting.ColorRadis) * 0.2f;

                if (isMainTile && floorSetting.ShowMainTiles)
                {
                    color.a = 0.7f;
                    Go.GetComponent<SpriteRenderer>().color = color;
                    Go.GetComponent<SpriteRenderer>().sortingOrder += 1;
                    Go.transform.localScale = TilesSample.transform.localScale * 2;
                }
                else
                {
                    color.a = floorSetting.HideSmallTiles ? 0 : 0.4f;
                    Go.GetComponent<SpriteRenderer>().color = color;
                }

                if (x == 0 && y == 0)
                {
                    color.a = 1f;
                    Go.GetComponent<SpriteRenderer>().color = color;
                    Go.GetComponent<SpriteRenderer>().sortingOrder += 1;
                    Go.transform.localScale = TilesSample.transform.localScale * 2;
                    CenterTile = currentTile;
                }

                TileList.Add(currentTile);
                currentID++;
            }
        }
        TilesSample.SetActive(false);
    }

    public void SetCircularTileList()
    {
        CircularTileList = new List<Tile>();
        for (int i = 0; i < TileList.Count; i++)
        {
            CircularTileList.Add(TileList[i]);
        }

        CircularTileList.Sort((x, y) =>
        {
            int result = x.Distance.CompareTo(y.Distance);

            if (result != 0)
            {
                return result;
            }

            return x.Angle.CompareTo(y.Angle);
        });

        for (int i = 0; i < CircularTileList.Count; i++)
        {
            CircularTileList[i].CircularID = i;
        }
    }

    private Tile GetTileByPos(Tile starting , PosVector position)
    {
        if (Mathf.Abs(starting.Position.x + position.x) > (floorSetting.GroundWidth - 1) / 2)
        {
            return null;
        }

        if (Mathf.Abs(starting.Position.y + position.y) > (floorSetting.GroundHeight - 1) / 2)
        {
            return null;
        }

        return TileList[starting.ID + position.y * floorSetting.GroundWidth + position.x];
    }

    public Tile GetTileByPos(PosVector position)
    {
        return GetTileByPos(CenterTile, position);
    }

    //public void OnPressed(object param)
    //{
    //    Vector2 pos = new Vector2();
    //    Tile selected = new Tile();

    //    if (param is Vector3)
    //    {
    //        pos = (Vector2)(Vector3)param;

    //        GameObject Nearest = AllTiles[0].Go;
    //        float distant = Mathf.Infinity;
    //        for (int i = 0; i < AllTiles.Count; i++)
    //        {
    //            if (((Vector2)(AllTiles[i].Go.transform.position) - pos).sqrMagnitude < distant)
    //            {
    //                selected = AllTiles[i];
    //                Nearest = AllTiles[i].Go;
    //                distant = ((Vector2)(AllTiles[i].Go.transform.position) - pos).sqrMagnitude;
    //            }


    //        }

    //        if (Nearest != null)
    //        {
    //            if (Nearest.activeSelf)
    //            {
    //                EventManager.TriggerEvent("PlaceMark", (Vector2)Nearest.transform.position);
    //            }
    //            else
    //            {
    //                EventManager.TriggerEvent("RemoveMark", (Vector2)Nearest.transform.position);
    //            }

    //            Nearest.SetActive(!Nearest.activeSelf);


    //        }
    //        selected.OnPressed();

    //        Vector2Int selectedPoint = new Vector2Int((int)selected.Go.transform.position.x, (int)selected.Go.transform.position.y);
    //        TransTables[30].TryGetValue(selectedPoint, out Vector2Int newPoint);
    //        TileByCoordinate.TryGetValue(newPoint, out Tile newTile);

    //        newTile.OnPressed();
    //    }
    //    else
    //    {
    //        Debug.Log("Worng Type");
    //    }
    //}
}
