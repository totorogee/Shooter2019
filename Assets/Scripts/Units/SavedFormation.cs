using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavedFormation
{
	public static int AllowedCount = 3;
	public static List<SavedFormation> SavedList
    {
        get
        {
            if (savedList.Count == 0)
            {
                Init();
            }
            return savedList;
        }
    }

    private static List<SavedFormation> savedList = new List<SavedFormation>();
	public static bool DidInit = false;

	public int ID = 0;
	public string Name = "";
	public bool Used = false;
    public Dictionary<PosVector, int> PositionGroupSizePair = new Dictionary<PosVector, int>();

	public static void Update()
	{
        if (!DidInit)
        {
            Init();
        }

        List<List<KeyValuePair<PosVector, int>>> saveData = new List<List<KeyValuePair<PosVector, int>>>();

        for (int i = 0; i < savedList.Count; i++)
        {
            saveData.Add(new List<KeyValuePair<PosVector, int>>());
            foreach (var item in savedList[i].PositionGroupSizePair)
            {
                saveData[i].Add(item);
            }
        }

        SaveDataController.Save(DataName.SavedFormation, saveData);
    }

    public static void Init()
    {
        DidInit = true;
        List<Dictionary<PosVector, int>> loadedData = new List<Dictionary<PosVector, int>>();

        if ( SaveDataController.TryLoad(DataName.SavedFormation, out List<List<KeyValuePair<PosVector, int>>> loadedRawData))
        {
            for (int i = 0; i < loadedRawData.Count; i++)
            {
                loadedData.Add(new Dictionary<PosVector, int>());
                foreach (var item in loadedRawData[i])
                {
                    loadedData[i].Add(item.Key, item.Value);
                }
            }
        }

        savedList = new List<SavedFormation>();

        if (loadedData == null || loadedData.Count == 0)
        {
            loadedData = new List<Dictionary<PosVector, int>>();
            for (int i = 0; i < AllowedCount; i++)
            {
                loadedData.Add(new Dictionary<PosVector, int>());
            }
        }

        for (int i = 0; i < AllowedCount; i++)
        {
            SavedFormation formation = new SavedFormation()
            {
                ID = i,
                Name = "Formation " + i.ToString(),
                Used = loadedData[i].Count != 0,
                PositionGroupSizePair = loadedData[i]

            };
            savedList.Add(formation);
        }


    }
}
