using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public enum DataName
{
    DefaultFormation = 0,
    SavedFormation = 1
        
}

public class SaveDataController : PrefabSingleton<SaveDataController>
{
    private static Dictionary<string, object> SavedData = new Dictionary<string, object>();

    public static bool TryLoad(DataName dataName, out int dataOut)
    {
        string key = "I_" + dataName.ToString();
        dataOut = PlayerPrefs.GetInt(key);
        return PlayerPrefs.HasKey(key);
    }

    public static void Save(DataName dataName, int data)
    {
        string key = "I_" + dataName.ToString();
        PlayerPrefs.SetInt(key, data);
    }

    public static bool TryLoad(DataName dataName, out float dataOut)
    {
        string key = "F_" + dataName.ToString();
        dataOut = PlayerPrefs.GetFloat(key);
        return PlayerPrefs.HasKey(key);
    }

    public static void Save(DataName dataName, float data)
    {
        string key = "F_" + dataName.ToString();
        PlayerPrefs.SetFloat(key, data);
    }

    public static bool TryLoad<T>(DataName dataName, out T dataOut)
    {
        dataOut = default;
        string key = typeof(T) + "_" + dataName.ToString();

        if (SavedData.TryGetValue(key, out object thisData))
        {
            dataOut = (T)thisData;
            return true;
        }

        if (PlayerPrefs.HasKey(key))
        {
            string JsonString = PlayerPrefs.GetString(key, "");

            dataOut = JsonConvert.DeserializeObject<T>(JsonString);
            SavedData.Add(key, dataOut);
            return true;
        }

        Debug.LogWarning("Cannot Load Data : " + key);
        return false;
    }

    public static void Save<T>(DataName dataName, T data)
    {
        string key = typeof(T) + "_" + dataName.ToString();




        if (SavedData.TryGetValue(key, out object thisData))
        {
            SavedData[key] = data;
        }
        else
        {
            SavedData.Add(key, data);
        }

        string JsonString = JsonConvert.SerializeObject(data);

        PlayerPrefs.SetString(key, JsonString);
    }
}
