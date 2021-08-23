using UnityEngine;
using Utilities;

namespace GridTool
{
    public enum GridType { CARTESIAN, POLAR }

    public enum GridDrawMode { WORLDSPACE, LOCALSPACE}

    public class GridGizmoDrawer : MonoBehaviour
    {
#if UNITY_EDITOR

        private static int renderSize;
        private static float cellSize;

        private static GridType gridType;
        private static float angularSize;

        public static void SetRenderSize(int newRenderSize) => renderSize = newRenderSize;
        public static void SetCellSize(float newCellSize) => cellSize = newCellSize;
        public static void SetGridType(GridType newGridType) => gridType = newGridType;
        public static void SetAngularSize(float newAngularSize) => angularSize = newAngularSize;

        public static void SetParameters(float newCellSize, int newRenderSize, GridType newGridType, float newAngularSize)
        {
            cellSize = newCellSize;
            renderSize = newRenderSize;
            gridType = newGridType;
            angularSize = newAngularSize;
        }

        private void OnDrawGizmos()
        {
            if (gridType == GridType.CARTESIAN)
            {
                Vector3 lengthVec = Vector3.right * cellSize * renderSize,
                    moveVec = Vector3.forward * cellSize;

                //draw horizontal lines
                DrawGridLines(lengthVec, moveVec);

                //draw vertical lines
                lengthVec = Vector3.forward * cellSize * renderSize;
                moveVec = Vector3.right * cellSize;

                DrawGridLines(lengthVec, moveVec);
            }
            else
            {
                DrawGridCircles();
                DrawRotatedGridLines();
            }
        }

        private void DrawGridLines(Vector3 lengthVec, Vector3 moveVec)
        {
            Vector3 lineOrigin;

            for (int i = 0; i < renderSize; i++)
            {
                lineOrigin = transform.position + moveVec * i;
                Gizmos.DrawLine(lineOrigin + lengthVec, lineOrigin - lengthVec);

                if (i == 0) continue;

                lineOrigin = transform.position - moveVec * i;
                Gizmos.DrawLine(lineOrigin + lengthVec, lineOrigin - lengthVec);
            }
        }

        private void DrawRotatedGridLines()
        {
            Vector3 lengthVec = Vector3.right * cellSize * renderSize;

            if (angularSize == 0f)
                angularSize = 0.1f;

            for (float a = 0f; a < 360f; a += angularSize)
            {
                Gizmos.DrawLine(transform.position, transform.position + Quaternion.AngleAxis(a, Vector3.up) * lengthVec);
            }
        }

        private void DrawGridCircles()
        {
            for (int i = 0; i < renderSize; i++)
            {
                DrawGridCircle(cellSize * i);
            }
        }

        private void DrawGridCircle(float radius)
        {
            Vector3[] circle = GizmoUtils.DrawCircleGizmo(radius, transform.position);

            for (var i = 0; i < circle.Length;)
            {
                if (i + 1 < circle.Length)
                {
                    Gizmos.DrawLine(circle[i], circle[++i]);
                }
                else
                {
                    Gizmos.DrawLine(circle[i++], circle[0]);
                }
            }
        }

#endif
    }
}
