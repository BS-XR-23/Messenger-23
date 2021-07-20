
using UnityEngine;

public static class TransformExtention 
{
    public static void DestoryAllChild(this Transform transform)
    {
        int childcound = transform.childCount;
        for (int i = 0; i <childcound ; i++)
        {
            Object.Destroy(transform.GetChild(0).gameObject);
        }
    }

    public static void DestoryAllChildImmediate(this Transform transform)
    {
        int childcound = transform.childCount;
        for (int i = 0; i < childcound; i++)
        {
            Object.DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }
}
