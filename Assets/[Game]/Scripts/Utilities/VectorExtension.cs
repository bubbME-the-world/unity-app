using UnityEngine;

public static class Vector3Extensions
{
    public static Vector3 SetZ(this Vector3 vector, float z)
    {
        return new Vector3(vector.x, vector.y, z);
    }
}