using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace SandBox
{
    public class Tiles : MonoBehaviour
    {

        public TilesSetting Setting;

        public static int TransfromAccuracy = 2;

        public int GroundWidth = 0;
        public int GroundHeight = 0;

        public int ColorSize = 5;

        public bool AltColor = true;
        public bool AltSquare = true;
        public bool UseSquare = false;
        public float TileSize = 0.5f;

        public GameObject TilesSample;
        public GameObject TilesSampleSq;
        public SpriteRenderer GroundToCover;

        public static List<Tile> AllTiles = new List<Tile>();

        public static List<Dictionary<Vector2Int, Vector2Int>> TransTables = new List<Dictionary<Vector2Int, Vector2Int>>();
        public static Dictionary<Vector2Int, Tile> TileByCoordinate = new Dictionary<Vector2Int, Tile>();

        private void OnEnable()
        {
            //EventManager.StartListening("MousePressed", OnPressed);

        }

        // Start is called before the first frame update
        void Start()
        {
            if (Setting == null)
            {
                Debug.LogWarning("Tiles : No Setting");
                Setting = ScriptableObject.CreateInstance<TilesSetting>();
            }
            else
            {

            }

            if (!UseSquare)
            {
                PlaceTiles();
            }
            else
            {
                PlaceSquare();
            }


            SetTransTable();

        }



        // Update is called once per frame
        void Update()
        {

        }

        public void SetTransTable(int increasment = 0, int range = 100)
        {
            if (increasment == 0)
            {
                increasment = TransfromAccuracy;
            }

            float start = Time.realtimeSinceStartup;
            List<Tile> starting = new List<Tile>();
            Vector3 temp = new Vector3();

            for (int x = 0; x < AllTiles.Count; x++)
            {
                Vector2Int coordinate = new Vector2Int((int)AllTiles[x].Go.transform.position.x, (int)AllTiles[x].Go.transform.position.y);

                TileByCoordinate.Add(coordinate, AllTiles[x]);

                if (AllTiles[x].Distance > range)
                {
                    continue;
                }

                if (AllTiles[x].rawPos.x < 0 || AllTiles[x].rawPos.y <= 0)
                {
                    continue;
                }

                starting.Add(AllTiles[x]);
            }

            for (int i = 0; i < 360; i++)
            {
                TransTables.Add(new Dictionary<Vector2Int, Vector2Int>());
            }


            for (int i = 0; i < 90 / increasment; i++)
            {
                temp = transform.eulerAngles;
                temp.z = -i;
                transform.eulerAngles = temp;

                Dictionary<Vector2Int, Vector2Int> Current = new Dictionary<Vector2Int, Vector2Int>();


                for (int x = 1; x < starting.Count; x++)
                {
                    List<Vector2Int> oldPoints = RotatedVectors(starting[x].rawPos);
                    List<Vector2Int> newPoints = RotatedVectors(new Vector2Int((int)starting[x].Go.transform.position.x, (int)starting[x].Go.transform.position.y));

                    for (int j = 0; j < 4; j++)
                    {
                        Current.Add(oldPoints[j], newPoints[j]);
                    }
                }

                for (int n = 0; n < TransfromAccuracy; n++)
                {
                    TransTables[i * increasment + n] = Current;
                }

            }

            temp = transform.eulerAngles;
            temp.z = 0;
            transform.eulerAngles = temp;

            Debug.Log("SetTransTable() Used Time : " + (Time.realtimeSinceStartup - start));


            start = Time.realtimeSinceStartup;

            Dictionary<int, int> tempint = new Dictionary<int, int>();
            foreach (var item in TransTables[1])
            {
                tempint.Add(item.Key.ToInt(), item.Value.ToInt());
            }

            Debug.Log("Test Used Time : " + (Time.realtimeSinceStartup - start) * 90);

            start = Time.realtimeSinceStartup;
            string tempString = JsonConvert.SerializeObject(tempint);
            Debug.Log("Make file Used Time : " + (Time.realtimeSinceStartup - start));

            start = Time.realtimeSinceStartup;
            FilePath.WriteStringToFile(tempString, "Save.txt");
            Debug.Log("Save File Used Time : " + (Time.realtimeSinceStartup - start));

        }

        public void PlaceSquare()
        {
            float start = Time.realtimeSinceStartup;

            AllTiles = new List<Tile>();

            Vector2 max = GroundToCover.bounds.max;
            Vector2 min = GroundToCover.bounds.min;

            TilesSample.SetActive(false);
            TilesSample = TilesSampleSq;
            TilesSample.SetActive(true);

            Vector3 Center = new Vector3();
            int x = 0;
            int y = 0;

            for (float i = Mathf.CeilToInt(min.x / TileSize) * TileSize; i < max.x; i += TileSize)
            {
                x++;

                y = 0;
                for (float j = Mathf.CeilToInt(min.y / TileSize); j < max.y; j += TileSize)
                {


                    y++;
                    GameObject Go = Instantiate(TilesSample, transform);
                    if (AltSquare)
                    {
                        Go.transform.localPosition = new Vector3(i + (y % 2 == 0 ? 0.5f * TileSize : 0f), j, 0);
                    }
                    else
                    {
                        Go.transform.localPosition = new Vector3(i, j, 0);
                    }

                    Go.transform.localScale = TilesSample.transform.localScale * TileSize;

                    Tile currentTile = new Tile();
                    if (System.Math.Abs(i) < float.Epsilon && System.Math.Abs(j) < float.Epsilon)
                    {
                        Tile.CenterTile = currentTile;
                    }

                    currentTile.Go = Go;
                    currentTile.rawPos = new Vector2Int((int)Go.transform.position.x, (int)Go.transform.position.y);
                    AllTiles.Add(currentTile);

                }
            }
            GroundWidth = x;
            GroundHeight = y;


            Debug.Log("PlaceSquare - place: " + (Time.realtimeSinceStartup - start));
            start = Time.realtimeSinceStartup;

            foreach (var item in AllTiles)
            {
                if (item.Go.GetComponent<SpriteRenderer>() != null)
                {
                    item.Go.GetComponent<SpriteRenderer>().color = item.Go.GetComponent<SpriteRenderer>().color *
                      (Mathf.CeilToInt((item.Go.transform.localPosition - Center).magnitude / (TileSize * 2 * ColorSize)) * 0.2f);
                    if (AltColor)
                    {
                        if (item.rawPos.y % 2 == 0 && (item.rawPos.y / 2 + item.rawPos.x) % 2 == 0)
                        {
                            var color = item.Go.GetComponent<SpriteRenderer>().color;
                            color.a = 0.8f;
                            item.Go.GetComponent<SpriteRenderer>().color = color;
                            item.Go.transform.localScale = TilesSample.transform.localScale * TileSize * 2;
                            item.Big = true;
                        }
                        else
                        {
                            var color = item.Go.GetComponent<SpriteRenderer>().color;
                            color.a = 0.5f;
                            item.Go.GetComponent<SpriteRenderer>().color = color;
                            item.Big = false;
                        }
                    }

                }
            }

            TilesSample.SetActive(false);

            Debug.Log("PlaceSquare - colour : " + (Time.realtimeSinceStartup - start));

            SetIdNumber(Vector2.zero);
        }

        public void PlaceTiles()
        {


            AllTiles = new List<Tile>();

            Vector2 max = GroundToCover.bounds.max;
            Vector2 min = GroundToCover.bounds.min;



            TilesSampleSq.SetActive(false);
            TilesSample.SetActive(true);
            Vector3 Center = new Vector3();

            for (float i = min.x; i < max.x; i += 1.5f * TileSize)
            {
                int n = 0;
                for (float j = min.y; j < max.y; j += 0.4330127f * TileSize)
                {
                    n++;
                    GameObject Go = Instantiate(TilesSample, transform);
                    Go.transform.localPosition = new Vector3(i + (n % 2 == 0 ? 0.75f * TileSize : 0f), j, 0);
                    Go.transform.localScale = TilesSample.transform.localScale * TileSize;

                    Tile currentTile = new Tile();
                    currentTile.Go = Go;
                    AllTiles.Add(currentTile);
                }
            }

            Center = AllTiles[AllTiles.Count / 2].Go.transform.localPosition;

            foreach (var item in AllTiles)
            {
                if (item.Go.GetComponent<SpriteRenderer>() != null)
                {
                    item.Go.GetComponent<SpriteRenderer>().color = item.Go.GetComponent<SpriteRenderer>().color *
                      (Mathf.CeilToInt((item.Go.transform.localPosition - Center).magnitude / (5f * TileSize)) * 0.2f);
                }
            }

            TilesSample.SetActive(false);
            Debug.Log((AllTiles.Count));
        }

        public void OnPressed(object param)
        {
            Vector2 pos = new Vector2();
            Tile selected = new Tile();

            if (param is Vector3)
            {
                pos = (Vector2)(Vector3)param;

                GameObject Nearest = AllTiles[0].Go;
                float distant = Mathf.Infinity;
                for (int i = 0; i < AllTiles.Count; i++)
                {
                    if (((Vector2)(AllTiles[i].Go.transform.position) - pos).sqrMagnitude < distant)
                    {
                        selected = AllTiles[i];
                        Nearest = AllTiles[i].Go;
                        distant = ((Vector2)(AllTiles[i].Go.transform.position) - pos).sqrMagnitude;
                    }


                }

                if (Nearest != null)
                {
                    if (Nearest.activeSelf)
                    {
                        EventManager.TriggerEvent("PlaceMark", (Vector2)Nearest.transform.position);
                    }
                    else
                    {
                        EventManager.TriggerEvent("RemoveMark", (Vector2)Nearest.transform.position);
                    }

                    Nearest.SetActive(!Nearest.activeSelf);


                }
                selected.OnPressed();

                Vector2Int selectedPoint = new Vector2Int((int)selected.Go.transform.position.x, (int)selected.Go.transform.position.y);
                TransTables[30].TryGetValue(selectedPoint, out Vector2Int newPoint);
                TileByCoordinate.TryGetValue(newPoint, out Tile newTile);

                newTile.OnPressed();
            }
            else
            {
                Debug.Log("Worng Type");
            }
        }

        public void SetIdNumber(Vector2 center)
        {
            float start = Time.realtimeSinceStartup;


            foreach (var item in AllTiles)
            {
                item.Distance = (((Vector2)item.Go.transform.position) - center).magnitude;
                item.Angle = Mathf.Atan2(center.x - item.Go.transform.position.x, center.y - item.Go.transform.position.y) * 180 / Mathf.PI + 180;
                if (Mathf.Abs(item.Angle - 360) <= float.Epsilon)
                {
                    item.Angle = 0f;
                }

            }

            AllTiles.Sort((x, y) =>
            {

                int result = x.Distance.CompareTo(y.Distance);

                if (result != 0)
                {
                    return result;
                }
                else
                {

                    result = x.Angle.CompareTo(y.Angle);
                }

                return result;
            });

            for (int i = 0; i < AllTiles.Count; i++)
            {
                AllTiles[i].ID = i;
            }

            Debug.Log("SetID(Used Time) : " + (Time.realtimeSinceStartup - start));

        }

        public List<Vector2Int> RotatedVectors(Vector2Int input)
        {
            return new List<Vector2Int>
        {
            input,
            new Vector2Int(input.y, -input.x),
            input * -1,
            new Vector2Int(-input.y, input.x),
        };
        }
    }

}

