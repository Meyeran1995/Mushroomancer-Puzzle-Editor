using System;
using UnityEngine;

namespace Utilities
{
    public static class GizmoUtils
    {
        private const float THETA_SCALE = Mathf.PI / 100f;
        private const int NUMBER_OF_POINTS = 32;

        public static Vector3[] DrawCircleGizmo(float radius, Vector3 origin)
        {
            float theta = 0f, x, z;
            Vector3[] circle = new Vector3[NUMBER_OF_POINTS];
            for (int i = 0; i < NUMBER_OF_POINTS; i++)
            {
                theta += (2.0f * Mathf.PI * THETA_SCALE);
                x = radius * Mathf.Cos(theta) + origin.x;
                z = radius * Mathf.Sin(theta) + origin.z;
                circle[i] = new Vector3(x, 0, z);
            }

            return circle;
        }

        public static T[] MergeArrays<T>(T[] array1, T[] array2)
        {
            T[] newArray = new T[array1.Length + array2.Length];
            Array.Copy(array1, newArray, array1.Length);
            Array.Copy(array2, 0, newArray, array1.Length, array2.Length);

            return newArray;
        }
    }
}
