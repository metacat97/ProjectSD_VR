using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalFunction
{
    public static int GetLayerMask(params string[] layerNames)
    {
        int result = 0;
        foreach(var layerName in layerNames)
        {
            int check = 1 << LayerMask.NameToLayer(layerName);
            if (check <= 0)
            {
                Debug.LogWarning("없는 레이어");
                continue;
            }
            result +=( 1 << LayerMask.NameToLayer(layerName));
        }
        return result;
    }

    public static void ChangeMaterialColor<T>(this T material, Color color) where T : Material 
    {
        material.color = color;
    }
}
