using UnityEditor;

public static class GridSnapper
{
    /// <summary>
    /// Snaps to a cartesian coordinate system
    /// </summary>
    /// <param name="gridSize">Grid cell interval</param>
    public static void SnapToGrid(float gridSize)
    {
        foreach (var go in Selection.gameObjects)
        {
            Undo.RecordObject(go, "Snap Objects");
            go.transform.position = (go.transform.position / gridSize).Round() * gridSize;
        }
    }
}
