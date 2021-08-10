using UnityEditor;
using UnityEngine;

public class GridSnapperWindow : EditorWindow
{
    private const string EDITOR_PREF_NAME_CELL = "GridSnapperGridCellSize";
    private const string EDITOR_PREF_NAME_SIZE = "GridSnapperGridSize";

    [SerializeField] private float gridCellSize; // needs to be serialized to be recorded
    [SerializeField] private int gridSize;

    private GameObject gizmoHolder;

    [MenuItem("Window/Grid Snap Tool")]
    public static void ShowWindow() => GetWindow<GridSnapperWindow>("Grid Snap Tool");

    private void OnEnable()
    {
        gridCellSize = EditorPrefs.GetFloat(EDITOR_PREF_NAME_CELL);
        gridSize = EditorPrefs.GetInt(EDITOR_PREF_NAME_SIZE);
        CreateGizmoHolder();
        GridGizmoDrawer.SetParameters(gridCellSize, gridSize);
        Selection.selectionChanged += OnSelectionChanged;
    }

    private void OnDisable()
    {
        DestroyImmediate(gizmoHolder);
        Selection.selectionChanged -= OnSelectionChanged;
    }

    private void OnGUI()
    {
        GUILayout.Label("Grid Settings");

        float newGridCellSize = EditorGUILayout.FloatField("Cell Size", gridCellSize);

        if (newGridCellSize != gridCellSize)
        {
            Undo.RecordObject(this, "Changed Snap Tool Grid Cell Size"); // record BEFORE doing changes
            gridCellSize = newGridCellSize;
            GridGizmoDrawer.SetCellSize(newGridCellSize);
        }

        int newGridSize = EditorGUILayout.IntField("Render Size", gridSize);

        if (newGridSize != gridSize)
        {
            Undo.RecordObject(this, "Changed Snap Tool Render Size");
            gridSize = newGridSize;
            GridGizmoDrawer.SetRenderSize(newGridSize);
        }

        using (new EditorGUI.DisabledScope(Selection.gameObjects.Length == 0))
        {
            if (!GUILayout.Button("Snap Selection")) return;

            GridSnapper.SnapToGrid(gridCellSize);

            if(gizmoHolder)
                gizmoHolder.transform.position = GridSnapper.GetGridCoordinate(gridCellSize, Selection.activeGameObject);
        }
    }

    private void OnDestroy()
    {
        EditorPrefs.SetFloat(EDITOR_PREF_NAME_CELL, gridCellSize); // gets called when window is closed
        EditorPrefs.SetInt(EDITOR_PREF_NAME_SIZE, gridSize);
    }

    private void OnLostFocus()
    {
        if(gizmoHolder)
            DestroyImmediate(gizmoHolder);
    }

    private void OnFocus()
    {
        if (!gizmoHolder)
            CreateGizmoHolder();
    }

    private void CreateGizmoHolder() => gizmoHolder = new GameObject("gizmoHolder", typeof(GridGizmoDrawer));

    private void OnSelectionChanged()
    {
        if(gizmoHolder == null) return;

        if (Selection.gameObjects.Length != 0)
        {
            gizmoHolder.SetActive(true);
            gizmoHolder.transform.position = GridSnapper.GetGridCoordinate(gridCellSize, Selection.activeGameObject);
        }
        else
        {
            gizmoHolder.SetActive(false);
        }
    }
}
