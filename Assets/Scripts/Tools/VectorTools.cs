using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[System.Serializable]
public struct PosVector
{
    public int x;
    public int y;

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

    public PosVector(Vector2 pos)
    {
        this.x = Mathf.RoundToInt(pos.x);
        this.y = Mathf.RoundToInt(pos.y);
    }

    public static PosVector operator + ( PosVector A , PosVector B)
    {
        return new PosVector(A.x + B.x, A.y + B.y);
    }

    public override string ToString()
    {
        return "x:" + x + ",y:" + y;
        // [{"x:2,y:0":2,"x:0,y:0":2,"x:-2,y:0":2,"x:-1,y:2":2,"x:1,y:2":2,"x:1,y:-2":2},{},{}]
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
