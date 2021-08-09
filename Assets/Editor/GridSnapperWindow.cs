using UnityEditor;
using UnityEngine;

public class GridSnapperWindow : EditorWindow
{
    private const string EDITOR_PREF_NAME = "GridSnapperGridSize";
    [SerializeField] private float gridSize; // needs to be serialized to be recorded

    [MenuItem("Window/Grid Snap Tool")]
    public static void ShowWindow() => GetWindow<GridSnapperWindow>("Grid Snap Tool");

    private void OnEnable()
    {
        gridSize = EditorPrefs.GetFloat(EDITOR_PREF_NAME);
    }

    private void OnGUI()
    {
        GUILayout.Label("Grid Settings");
        float newGridSize = EditorGUILayout.FloatField("Size", gridSize);

        if (newGridSize != gridSize)
        {
            Undo.RecordObject(this, "Changed Snap Tool Grid Size"); // record BEFORE doing changes
            gridSize = newGridSize;
        }

        using (new EditorGUI.DisabledScope(Selection.gameObjects.Length == 0))
        {
            if(GUILayout.Button("Snap Selection"))
                GridSnapper.SnapToGrid(gridSize);
        }
    }

    private void OnDestroy()
    {
        EditorPrefs.SetFloat(EDITOR_PREF_NAME, gridSize); // gets called when window is closed
    }
}
