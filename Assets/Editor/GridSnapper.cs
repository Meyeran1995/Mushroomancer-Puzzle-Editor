using UnityEditor;
using UnityEngine;
using Utilities;

namespace GridTool
{
    public static class GridSnapper
    {
        /// <summary>
        /// Snaps to a cartesian coordinate system
        /// </summary>
        /// <param name="gridCellSize">Grid cell interval</param>
        public static void SnapToGrid(float gridCellSize)
        {
            Undo.SetCurrentGroupName("Snap group of Objects");
            int group = Undo.GetCurrentGroup();

            foreach (var go in Selection.gameObjects)
            {
                Undo.RecordObject(go.transform, "Snap Object");
                go.transform.position = GetGridCoordinate(gridCellSize, go);
            }

            Undo.CollapseUndoOperations(group);
        }

        /// <summary>
        /// Snaps to a polar coordinate system
        /// </summary>
        /// <param name="gridCellSize">Grid cell interval</param>
        /// <param name="angularSize">Angular grid interval</param>
        public static void SnapToGrid(float gridCellSize, float angularSize)
        {
            Undo.SetCurrentGroupName("Snap group of Objects");
            int group = Undo.GetCurrentGroup();

            foreach (var go in Selection.gameObjects)
            {
                Undo.RecordObject(go.transform, "Snap Object");
                go.transform.position = GetGridCoordinate(gridCellSize, angularSize, go);
            }

            Undo.CollapseUndoOperations(group);
        }

        public static Vector3 GetGridCoordinate(float gridCellSize, GameObject go) =>
            (go.transform.position / gridCellSize).Round() * gridCellSize;

        public static Vector3 GetGridCoordinate(float gridCellSize, float angularSize, GameObject go)
        {
            Vector3 dir = go.transform.position; //- Vector3.zero
            float dist = Mathf.Round(dir.magnitude / gridCellSize) * gridCellSize;

            float angle = Vector3.SignedAngle(Vector3.forward, dir, Vector3.up);
            angle = Mathf.Round(angle / angularSize) * angularSize;

            return Quaternion.AngleAxis(angle, Vector3.up) * Vector3.forward * dist;
        }
    }
}