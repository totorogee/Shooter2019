using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[System.Serializable]
public struct PosVector
{
    public int x;
    public int y;

    public PosVector(int x, int y)
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

    public static bool operator ==(PosVector A, PosVector B)
    {
        return A.x == B.x && A.y == B.y;
    }

    public static bool operator !=(PosVector A, PosVector B)
    {
        return !(A == B);
    }

    public static PosVector operator +(PosVector A, PosVector B)
    {
        return new PosVector(A.x + B.x, A.y + B.y);
    }

    public static PosVector operator -(PosVector A, PosVector B)
    {
        return new PosVector(A.x - B.x, A.y - B.y);
    }

    public static PosVector operator *(PosVector A, float B)
    {
        return new PosVector(Mathf.RoundToInt(A.x * B), Mathf.RoundToInt(A.y * B));
    }

    public static int SqDistance(PosVector A, PosVector B)
    {
        return (A.x - B.x) * (A.x - B.x) + (A.y - B.y) * (A.y - B.y);
    }


    public static PosVector Rotate(PosVector A, int degrees)
    {
        float sin = Mathf.Sin(-degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(-degrees * Mathf.Deg2Rad);

        float tx = A.x;
        float ty = A.y;

        A.x = Mathf.RoundToInt((cos * tx) - (sin * ty));
        A.y = Mathf.RoundToInt((sin * tx) + (cos * ty));

        return A;
    }

    public static Vector3 ToVector3(PosVector A)
    {
        return new Vector3(A.x, A.y, 0);
    }

    public static float Angle(PosVector A, PosVector B)
    {
        PosVector temp = B - A;

        if (temp.x == 0 && temp.y == 0)
        {
            return 0f;
        }

        float result = Mathf.Atan2(-temp.x, -temp.y) * 180 / Mathf.PI + 180;
        return (Mathf.Abs(result - 360) <= float.Epsilon) ? 0 : result;
    }

    public override string ToString()
    {
        return "x:" + x + ",y:" + y;
    }
}

public static class VectorTools
{

    #region SandBox

    public static int ToInt(this Vector2Int vector)
    {
        return (vector.x << 16) | vector.y & 0xFFFF;
    }

    public static Vector2Int ToVector2Int(this int input)
    {
        return new Vector2Int(input >> 16, (short)(input & 0xFFFF));
    }

    public static Vector2Int RotatedVectors(this Vector2Int input, int angle)
    {
        switch (angle % 90)
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
