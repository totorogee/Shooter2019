using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
namespace SandBox
{
    public class SandBoxController : PrefabSingleton<SandBoxController>
    {



        public TextAsset TilesSettingText;
        public int UpdateTime = 10;


        // Start is called before the first frame update
        void Start()
        {
            //EventManager.TriggerEvent("Init");

            //Save();
            //Load();

            //Vector2Int temp = new Vector2Int(3000, 4500);
            //int temp2 = temp.ToInt();

            //Debug.Log(temp2.ToString());
            //Debug.Log(temp2.ToVector2Int());
        }

        // Update is called once per frame
        void Update()
        {
            if (Time.frameCount % UpdateTime == 0)
            {
                //EventManager.TriggerEvent("Update");
            }
        }

        public void Save()
        {
            float start = Time.realtimeSinceStartup;
            string tempString = JsonConvert.SerializeObject(Tiles.TransTables[1]);
            Debug.Log(tempString.Length);
            Debug.Log("Create File Used Time : " + (Time.realtimeSinceStartup - start));

            start = Time.realtimeSinceStartup;
            FilePath.WriteStringToFile(tempString, "Save.txt");
            Debug.Log("Save File Used Time : " + (Time.realtimeSinceStartup - start));

        }

        public void Load()
        {
            float start = Time.realtimeSinceStartup;
            Debug.Log(FilePath.ReadStringFromFile("Save.txt").Length);
            Debug.Log("Load File Used Time : " + (Time.realtimeSinceStartup - start));
        }
    }
}
