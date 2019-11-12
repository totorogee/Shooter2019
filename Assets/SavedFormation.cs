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

	public static void Update(List<List<PosVector>> loadedData)
	{
		DidInit = true;
        SavedList = new List<SavedFormation>();

        if (loadedData == null)
        {
            loadedData = new List<List<PosVector>>();
        }

		for (int i = 0; i < AllowedCount; i++)
		{
			SavedFormation formation = new SavedFormation()
			{
				ID = i,
				Name = "Formation " + i.ToString(),
				Used = i < loadedData.Count,
				Posistions = i < loadedData.Count ? loadedData[i] : new List<PosVector>()
			};
            SavedList.Add(formation);
		}
	}
}
