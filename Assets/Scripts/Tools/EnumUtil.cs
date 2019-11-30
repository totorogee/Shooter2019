using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public static class EnumUtil
{
    public static IEnumerable<T> GetValues<T>()
    {
        return Enum.GetValues(typeof(T)).Cast<T>();
    }

    public static List<T> GetValuesList<T>()
    {
        List<T> Result = new List<T>(Enum.GetValues(typeof(T)).Cast<T>());
        return Result;
    }

}
