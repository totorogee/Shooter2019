using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{

    public bool AccpetInput = true;
    public bool DetectMainTile = true;
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
        EventManager.StartListening<Vector2>(EventList.OnMousePressed , MousePressed);

    }

    private void OnDisable()
    {
        EventManager.StopListening<Vector2>(EventList.OnMousePressed, MousePressed);

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

    private void MousePressed(Vector2 mousePosition)
    {
        if (AccpetInput)
        {
            if (DetectMainTile)
            {
                EventManager.TriggerEvent(EventList.TilePressed, GetMainTileByPos(mousePosition));
            }
            else
            {
                EventManager.TriggerEvent(EventList.TilePressed, GetTileByPos(new PosVector(mousePosition)));
            }

 
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

                color = Color.Lerp(floorSetting.CenterColor, floorSetting.EdgeColor, Mathf.CeilToInt(currentTile.Distance / floorSetting.ColorRadis) * 0.2f);


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
                    color.a = 0.75f;
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

    public Tile GetMainTileByPos( Vector2 position)
    {

        if (GetTileByPos(new PosVector(position)).IsMainTile)
        {
            return GetTileByPos(new PosVector(position));
        }

        List <Tile> Nearby = NearbyTiles(GetTileByPos(new PosVector(position)));

        Tile result = Nearby[0];
        float distance = Mathf.Infinity;

        for (int i = 0; i < Nearby.Count; i++)
        {
            if (Nearby[i].IsMainTile)
            {
                float thisDistance = new Vector2(position.x - Nearby[i].TileObject.transform.position.x,
                                 position.y - Nearby[i].TileObject.transform.position.y).magnitude;

                if (thisDistance < distance)
                {
                    distance = thisDistance;
                    result = Nearby[i];
                }
            }

        }

        return result;
    }

    public List<Tile> NearbyTiles(Tile starting, int range = 1 , bool checkMainTile = false)
    {
        List<Tile> result = new List<Tile>();
        for (int i = -1 * range ; i <= 1 * range; i++)
        {
            for (int j = -1 * range; j <= 1 * range; j++)
            {
                Tile tile = GetTileByPos(starting.Position + new PosVector(i, j));
                if (tile != null && tile != starting )
                {
                    if (checkMainTile && !tile.IsMainTile)
                    {
                        continue;
                    }
                    result.Add(tile);
                }
            }
        }

        return result;
    }
}
