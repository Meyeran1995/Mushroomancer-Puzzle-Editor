using UnityEditor;
using UnityEngine;

namespace GridTool
{
    public class GridSnapperWindow : EditorWindow
    {
        #region Name Constants

        private const string EDITOR_PREF_NAME_CELL = "GridSnapperGridCellSize";
        private const string EDITOR_PREF_NAME_SIZE = "GridSnapperGridSize";
        private const string EDITOR_PREF_NAME_GRIDTYPE = "GridType";
        private const string GIZMO_HOLDER_NAME = "gizmoHolder";

        #endregion

        [SerializeField] private float gridCellSize; // needs to be serialized to be recorded
        [SerializeField] private int gridSize;

        [SerializeField] private GridType gridType;

        private GameObject gizmoHolder;

        [MenuItem("Window/Grid Snap Tool")]
        public static void ShowWindow() => GetWindow<GridSnapperWindow>("Grid Snap Tool");

        private void OnEnable()
        {
            gridCellSize = EditorPrefs.GetFloat(EDITOR_PREF_NAME_CELL);
            gridSize = EditorPrefs.GetInt(EDITOR_PREF_NAME_SIZE);
            gridType = (GridType)EditorPrefs.GetInt(EDITOR_PREF_NAME_GRIDTYPE);
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

            GridType newGridType = (GridType)EditorGUILayout.EnumPopup("Grid Type", gridType);

            if (newGridType != gridType)
            {
                Undo.RecordObject(this, "Changed Grid Type");
                gridType = newGridType;
                //GridGizmoDrawer.SetGridType(gridType);
            }

            using (new EditorGUI.DisabledScope(Selection.gameObjects.Length == 0))
            {
                if (!GUILayout.Button("Snap Selection")) return;

                GridSnapper.SnapToGrid(gridCellSize);

                if (gizmoHolder)
                    gizmoHolder.transform.position = GridSnapper.GetGridCoordinate(gridCellSize, Selection.activeGameObject);
            }
        }

        private void OnDestroy()
        {
            EditorPrefs.SetFloat(EDITOR_PREF_NAME_CELL, gridCellSize); // gets called when window is closed
            EditorPrefs.SetInt(EDITOR_PREF_NAME_SIZE, gridSize);
            EditorPrefs.SetInt(EDITOR_PREF_NAME_GRIDTYPE, (int)gridType);

            if (gizmoHolder)
                DestroyImmediate(gizmoHolder);
        }

        private void OnLostFocus()
        {
            if (gizmoHolder && Selection.gameObjects.Length == 0)
                DestroyImmediate(gizmoHolder);
        }

        private void OnFocus()
        {
            if (!gizmoHolder)
                CreateGizmoHolder();
        }

        private void CreateGizmoHolder()
        {
            gizmoHolder = GameObject.Find(GIZMO_HOLDER_NAME);

            if (!gizmoHolder)
                gizmoHolder = new GameObject(GIZMO_HOLDER_NAME, typeof(GridGizmoDrawer));
        }

        private void OnSelectionChanged()
        {
            if (Selection.gameObjects.Length != 0)
            {
                if (!gizmoHolder)
                    CreateGizmoHolder();

                gizmoHolder.transform.position = GridSnapper.GetGridCoordinate(gridCellSize, Selection.activeGameObject);
                Repaint();
            }
            else
            {
                DestroyImmediate(gizmoHolder);
            }
        }
    }
}