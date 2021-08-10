using UnityEditor;
using UnityEngine;
using Utilities;

public static class GridSnapper
{
    /// <summary>
    /// Snaps to a cartesian coordinate system
    /// </summary>
    /// <param name="gridCellSize">Grid cell interval</param>
    public static void SnapToGrid(float gridCellSize)
    {
        foreach (var go in Selection.gameObjects)
        {
            Undo.RecordObject(go, "Snap Objects");
            go.transform.position = GetGridCoordinate(gridCellSize, go);
        }
    }

    public static Vector3 GetGridCoordinate(float gridCellSize, GameObject go) => (go.transform.position / gridCellSize).Round() * gridCellSize;
}
