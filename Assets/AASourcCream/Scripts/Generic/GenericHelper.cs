using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GenericHelper
{
    private static RaycastHit hit;

    //Note will need update for co-op
    public static Vector3 GetMousePostion()
    {
        int layerMask = 1 << 8;
        var hitResult = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity, layerMask);
        var hitPos = new Vector3(hit.point.x, hit.point.y, hit.point.z);
        return hitPos;
    }

    /// <summary>
    /// Will try to get a biased number that hasn't been in the previous rolls
    /// </summary>
    public static float GetRandomBiasedNumberBetween(float from, float to, float[] previousRolls)
    {
        float previousResultAverage = previousRolls.Length > 0 ? previousRolls.Average() : 0;
        float randomNumber;

        if (previousResultAverage > 0)
        {
            if (previousResultAverage > ((from + to) / 2))
            {
                randomNumber = Random.Range(from, (to - previousResultAverage));
            }
            else
            {
                randomNumber = Random.Range((from + previousResultAverage), to);
            }
        }
        else
        {
            randomNumber = Random.Range(from, to);
        }
        return randomNumber;
    }

}