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
    }
}
