using UnityEngine;
using System.Collections;
using System;

public static class MathExtension {

    /// <summary>
    /// Compute the angle of a vector in degrees
    /// </summary>
    /// <param name="vector">This vector</param>
    /// <returns>Angle in degree</returns>
    public static float Angle(this UnityEngine.Vector2 vector, bool InDegree = true)
    {
        float angle = Mathf.Atan2(vector.y, vector.x);
        if (InDegree)
        {
            return angle * Mathf.Rad2Deg;
        }
        else
        {
            return angle;
        }
    }

    private static float Wrap(this float number, float limit)
    {
        float res = number % limit;
        if (res < 0) res += limit;
        return res;
    }

    public static Vector2 Direction(this float angle, bool angleIsInDegree = true)
    {
        float rad = angle;
        if (angleIsInDegree) rad = angle * Mathf.Deg2Rad;
        return new UnityEngine.Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
    }
    
}
