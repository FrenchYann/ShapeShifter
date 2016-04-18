using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class IListExtension
{
    public static T Random<T>(this IList<T> list)
    {
        if (list.Count > 0)
        {
            return list[UnityEngine.Random.Range(0, list.Count)];
        }
        else
        {
            return default(T);
        }
    }
    public static T Last<T>(this IList<T> list)
    {
        if (list.Count > 0)
        {
            return list[list.Count - 1];
        }
        else
        {
            return default(T);
        }
    }
    public static bool IsEmpty<T>(this IList<T> list)
    {
        return list.Count == 0;
    }

    public static void Shuffle<T>(this IList<T> list)
    {
        int current = list.Count-1;
        while (current > 0)
        {
            int idx = UnityEngine.Random.Range(0, current+1);
            T value = list[idx];
            list[idx] = list[current];
            list[current] = value;
            current--;
        }
    }
}