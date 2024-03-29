﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PosVector
{
    public int x = 0;
    public int y = 0;

    public PosVector()
    {

    }

    public PosVector(int x , int y)
    {
        this.x = x;
        this.y = y;
    }

    public PosVector(Vector3 pos)
    {
        this.x = Mathf.RoundToInt(pos.x);
        this.y = Mathf.RoundToInt(pos.y);
    }
}

public static class VectorTools
{

    #region SandBox

    public static int ToInt (this Vector2Int vector)
    {
        return (vector.x << 16) | vector.y & 0xFFFF;
    }

    public static Vector2Int ToVector2Int(this int input)
    {
        return new Vector2Int(input >> 16, (short)(input & 0xFFFF));
    }

    public static Vector2Int RotatedVectors(this Vector2Int input, int angle)
    {
        switch (angle%90)
        {
            case 0:
                return input;
            case 1:
                return new Vector2Int(input.y, -input.x);
            case 2:
                return input * -1;
            case 3:
                return new Vector2Int(-input.y, input.x);
            default:
                return input;
        }
    }

    public static List<Vector2Int> RotatedVectors(this Vector2Int input)
    {
        return new List<Vector2Int>
        {
            input,
            new Vector2Int(input.y, -input.x),
            input * -1,
            new Vector2Int(-input.y, input.x),
        };
    }
    #endregion
}
