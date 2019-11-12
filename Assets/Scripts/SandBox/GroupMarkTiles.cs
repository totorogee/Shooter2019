using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SandBox
{
    public class GroupMarkTiles : MonoBehaviour
    {
        public float MinDistane = 3;
        public float MaxDistane = 5;
        public float MinAngle = 5f;
        public float MaxAngle = 10f;

        public List<int> MarkedID = new List<int>();

        private void OnEnable()
        {
            //EventManager.StartListening("GroupMark", OnMark);
            //EventManager.StartListening("GroupMarkRemove", OnRemove);
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnRemove(object parma)
        {

            for (int i = 0; i < MarkedID.Count; i++)
            {
                EventManager.TriggerEvent("RemoveMark", (Vector2)Tiles.AllTiles[MarkedID[i]].Go.transform.position);

            }
            MarkedID = new List<int>();
        }

        public void OnMark(object parma)
        {
            float timeStart = Time.realtimeSinceStartup;

            int n = 0;

            for (int i = 0; i < Tiles.AllTiles.Count; i++)
            {

                var item = Tiles.AllTiles[i];
                if (item.Distance < MinDistane)
                {
                    continue;
                }

                if (item.Distance > MaxDistane)
                {
                    break;
                }

                if (item.Angle > MaxAngle || item.Angle < MinAngle)
                {
                    continue;
                }

                StartCoroutine(DelayMark(i, n));
                n++;

            }

            Debug.Log("GroupMark : " + n + " point marked / in " + (Time.realtimeSinceStartup - timeStart));
        }

        public IEnumerator DelayMark(int ID, int n, float delayTime = 0.0000f)
        {
            MarkedID.Add(ID);

            yield return new WaitForSeconds(n * delayTime);
            EventManager.TriggerEvent("PlaceMark", (Vector2)Tiles.AllTiles[ID].Go.transform.position);
        }
    }
}