using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GradientUtil
{
    public static UnityEngine.Gradient Lerp(UnityEngine.Gradient a, UnityEngine.Gradient b, float t)
    {
        return Lerp(a, b, t, false, false);
    }

    public static UnityEngine.Gradient LerpNoAlpha(UnityEngine.Gradient a, UnityEngine.Gradient b, float t)
    {
        return Lerp(a, b, t, true, false);
    }

    public static UnityEngine.Gradient LerpNoColor(UnityEngine.Gradient a, UnityEngine.Gradient b, float t)
    {
        return Lerp(a, b, t, false, true);
    }

    static UnityEngine.Gradient Lerp(UnityEngine.Gradient a, UnityEngine.Gradient b, float t, bool noAlpha, bool noColor)
    {
        //list of all the unique key times
        var colorKeysTimes = new List<float>();
        var alphaKeysTimes = new List<float>();

        if (!noColor)
        {
            if (a.colorKeys.Length == b.colorKeys.Length)
            {
                for (int i = 0; i < a.colorKeys.Length; i++)
                {
                    float k =  Mathf.Lerp(a.colorKeys[i].time , b.colorKeys[i].time , t);
                    if (!colorKeysTimes.Contains(k))
                        colorKeysTimes.Add(k);
                }
            }
            else
            {
                for (int i = 0; i < a.colorKeys.Length; i++)
                {
                    float k = a.colorKeys[i].time;
                    if (!colorKeysTimes.Contains(k))
                        colorKeysTimes.Add(k);
                }

                for (int i = 0; i < b.colorKeys.Length; i++)
                {
                    float k = b.colorKeys[i].time;
                    if (!colorKeysTimes.Contains(k))
                        colorKeysTimes.Add(k);
                }
            }

        }

        if (!noAlpha)
        {
            if (a.alphaKeys.Length == b.alphaKeys.Length)
            {
                for (int i = 0; i < a.alphaKeys.Length; i++)
                {
                    float k = Mathf.Lerp(a.alphaKeys[i].time, b.alphaKeys[i].time, t);
                    if (!alphaKeysTimes.Contains(k))
                        alphaKeysTimes.Add(k);
                }
            }
            else
            {
                for (int i = 0; i < a.alphaKeys.Length; i++)
                {
                    float k = a.alphaKeys[i].time;
                    if (!alphaKeysTimes.Contains(k))
                        alphaKeysTimes.Add(k);
                }

                for (int i = 0; i < b.alphaKeys.Length; i++)
                {
                    float k = b.alphaKeys[i].time;
                    if (!alphaKeysTimes.Contains(k))
                        alphaKeysTimes.Add(k);
                }
            }
        }

        GradientColorKey[] clrs = new GradientColorKey[colorKeysTimes.Count];
        GradientAlphaKey[] alphas = new GradientAlphaKey[alphaKeysTimes.Count];

        //Pick colors of both gradients at key times and lerp them
        for (int i = 0; i < colorKeysTimes.Count; i++)
        {
            float key = colorKeysTimes[i];
            var clr = Color.Lerp(a.Evaluate(key), b.Evaluate(key), t);
            clrs[i] = new GradientColorKey(clr, key);
        }

        for (int i = 0; i < alphaKeysTimes.Count; i++)
        {
            float key = alphaKeysTimes[i];
            var clr = Color.Lerp(a.Evaluate(key), b.Evaluate(key), t);
            alphas[i] = new GradientAlphaKey(clr.a, key);
        }

        var g = new UnityEngine.Gradient();
        g.SetKeys(clrs, alphas);

        return g;
    }
}