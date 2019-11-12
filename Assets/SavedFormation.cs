using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavedFormation
{
	public static int AllowedCount = 3;
	public static List<SavedFormation> SavedList = new List<SavedFormation>();
	public static bool DidInit = false;

	public int ID = 0;
	public string Name = "";
	public bool Used = false;
	public List<PosVector> Posistions;

	public static void Update()
	{
        if (!DidInit)
        {
            Init();
        }

        List<List<PosVector>> saveData = new List<List<PosVector>>();

        for (int i = 0; i < SavedList.Count; i++)
        {
            saveData.Add(SavedList[i].Posistions);
        }

        SaveDataController.Save(DataName.SavedFormation, saveData);
    }

    public static void Init()
    {
        DidInit = true;
        SaveDataController.TryLoad(DataName.SavedFormation, out List<List<PosVector>> loadedData);

        SavedList = new List<SavedFormation>();

        if (loadedData == null)
        {
            loadedData = new List<List<PosVector>>();
            for (int i = 0; i < AllowedCount; i++)
            {
                loadedData.Add(new List<PosVector>());
            }
        }

        for (int i = 0; i < AllowedCount; i++)
        {
            SavedFormation formation = new SavedFormation()
            {
                ID = i,
                Name = "Formation " + i.ToString(),
                Used = loadedData[i].Count != 0,
                Posistions = loadedData[i]

            };
            SavedList.Add(formation);
        }


    }
}
