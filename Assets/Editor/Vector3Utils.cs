using UnityEngine;

public static class Vector3Utils
{
    public static Vector3 Round(this Vector3 vec)
    {
        vec.x = Mathf.Round(vec.x);
        vec.y = Mathf.Round(vec.y);
        vec.z = Mathf.Round(vec.z);

        return vec;
    }
}
