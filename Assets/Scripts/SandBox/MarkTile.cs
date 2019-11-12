using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SandBox
{
    public class MarksInfo
    {
        public GameObject Mark;
        public Vector2 Position;
    }

    public class MarkTile : MonoBehaviour
    {
        public int AngleOffset = 45;

        //public static List<string> TileMarks = new List<string>()
        //{
        //    "1","2","3","4"
        //};
        public float MarkSize = 1f;

        public Text CurrentIDText;
        public int CurrentMarkID = 0;

        public List<GameObject> MarksSample = new List<GameObject>();

        public List<MarksInfo> MarksUsed = new List<MarksInfo>();

        private void OnEnable()
        {
            //EventManager.StartListening("PlaceMark", OnPlaceMark);
            //EventManager.StartListening("RemoveMark", OnRemoveMark);
        }

        // Start is called before the first frame update
        void Start()
        {
            foreach (var item in MarksSample)
            {
                item.transform.localScale = item.transform.localScale * MarkSize;
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void ChangeMark()
        {
            CurrentMarkID++;
            CurrentMarkID = Mathf.Clamp(CurrentMarkID, 0, MarksSample.Count - 1);
            CurrentIDText.text = "Mark: " + (CurrentMarkID + 1).ToString();
        }

        public void OnPlaceMark(object parma)
        {
            Vector2 pos = new Vector2();
            if (parma is Vector2)
            {
                pos = (Vector2)parma;
            }
            else
            {
                return;
            }

            if (AngleOffset != 0)
            {
                Tiles.TransTables[AngleOffset].TryGetValue(new Vector2Int((int)pos.x, (int)pos.y), out Vector2Int newpoint);
                pos = newpoint;
            }


            MarksInfo CurrentInfo = new MarksInfo();

            GameObject GO = Instantiate(MarksSample[CurrentMarkID], transform);
            GO.SetActive(true);
            GO.transform.position = pos;

            CurrentInfo.Position = pos;
            CurrentInfo.Mark = GO;

            MarksUsed.Add(CurrentInfo);
        }

        public void OnRemoveMark(object parma)
        {
            Vector2 pos = new Vector2();
            if (parma is Vector2)
            {
                pos = (Vector2)parma;
            }
            else
            {
                return;
            }

            if (AngleOffset != 0)
            {
                Tiles.TransTables[AngleOffset].TryGetValue(new Vector2Int((int)pos.x, (int)pos.y), out Vector2Int newpoint);
                pos = newpoint;
            }

            List<MarksInfo> Removing = new List<MarksInfo>();
            foreach (var item in MarksUsed)
            {
                if (item.Position == pos)
                {
                    Destroy(item.Mark);
                    Removing.Add(item);
                }
            }

            MarksUsed = MarksUsed.Except(Removing).ToList();

        }
    }
}