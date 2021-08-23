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
        private const string EDITOR_PREF_NAME_GRIDCENTER = "GridCenter";
        private const string EDITOR_PREF_NAME_ANGULARSIZE = "GridSnapperAngularSize";
        private const string GIZMO_HOLDER_NAME = "gizmoHolder";

        #endregion

        [SerializeField] private float gridCellSize; // needs to be serialized to be recorded
        [SerializeField] private int gridSize;

        [SerializeField] private GridType gridType;
        [Min(0.1f)][SerializeField] private float gridAngularSize;

        [SerializeField] private GridDrawMode gridDrawMode;
        private GameObject gizmoHolder;

        [MenuItem("Window/Grid Snap Tool")]
        public static void ShowWindow() => GetWindow<GridSnapperWindow>("Grid Snap Tool");

        private void OnEnable()
        {
            gridCellSize = EditorPrefs.GetFloat(EDITOR_PREF_NAME_CELL);
            gridSize = EditorPrefs.GetInt(EDITOR_PREF_NAME_SIZE);
            gridType = (GridType)EditorPrefs.GetInt(EDITOR_PREF_NAME_GRIDTYPE);
            gridDrawMode = (GridDrawMode)EditorPrefs.GetInt(EDITOR_PREF_NAME_GRIDCENTER);
            gridAngularSize = EditorPrefs.GetFloat(EDITOR_PREF_NAME_ANGULARSIZE);
            CreateGizmoHolder();
            GridGizmoDrawer.SetParameters(gridCellSize, gridSize, gridType, gridAngularSize);
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

            GridDrawMode newGridDrwDrawMode = (GridDrawMode)EditorGUILayout.EnumPopup("Grid DrawMode", gridDrawMode);

            if (newGridDrwDrawMode != gridDrawMode)
            {
                Undo.RecordObject(this, "Changed Grid Draw Mode");
                gridDrawMode = newGridDrwDrawMode;
                CenterGizmoHolder();
            }

            if (gridType == GridType.POLAR)
            {
                float newGridAngularSize = EditorGUILayout.FloatField("Angular Size", gridAngularSize);

                if (newGridAngularSize != gridAngularSize)
                {
                    Undo.RecordObject(this, "Changed Snap Tool Grid Angular Size");
                    gridAngularSize = newGridAngularSize;
                    GridGizmoDrawer.SetAngularSize(newGridAngularSize);
                }
            }

            GridType newGridType = (GridType)EditorGUILayout.EnumPopup("Grid Type", gridType);

            if (newGridType != gridType)
            {
                Undo.RecordObject(this, "Changed Grid Type");
                gridType = newGridType;
                GridGizmoDrawer.SetGridType(gridType);
            }

            using (new EditorGUI.DisabledScope(Selection.gameObjects.Length == 0))
            {
                if (!GUILayout.Button("Snap Selection")) return;

                if (gridType == GridType.CARTESIAN)
                {
                    GridSnapper.SnapToGrid(gridCellSize);
                }
                else
                {
                    GridSnapper.SnapToGrid(gridCellSize, gridAngularSize);
                }

                CenterGizmoHolder();
            }
        }

        private void OnDestroy()
        {
            EditorPrefs.SetFloat(EDITOR_PREF_NAME_CELL, gridCellSize); // gets called when window is closed
            EditorPrefs.SetFloat(EDITOR_PREF_NAME_ANGULARSIZE, gridAngularSize);
            EditorPrefs.SetInt(EDITOR_PREF_NAME_SIZE, gridSize);
            EditorPrefs.SetInt(EDITOR_PREF_NAME_GRIDTYPE, (int)gridType);
            EditorPrefs.SetInt(EDITOR_PREF_NAME_GRIDCENTER, (int)gridDrawMode);

            if (gizmoHolder)
                DestroyImmediate(gizmoHolder);
        }

        //private void OnLostFocus()
        //{
        //    if (gizmoHolder && Selection.gameObjects.Length == 0)
        //        DestroyImmediate(gizmoHolder);
        //}

        //private void OnFocus()
        //{
        //    if (!gizmoHolder)
        //        CreateGizmoHolder();
        //}

        private void CreateGizmoHolder()
        {
            gizmoHolder = GameObject.Find(GIZMO_HOLDER_NAME);

            if (!gizmoHolder)
                gizmoHolder = new GameObject(GIZMO_HOLDER_NAME, typeof(GridGizmoDrawer));
        }

        private void CenterGizmoHolder()
        {
            if (!gizmoHolder)
                CreateGizmoHolder();

            if (gridDrawMode == GridDrawMode.WORLDSPACE || Selection.gameObjects.Length == 0)
            {
                gizmoHolder.transform.position = Vector3.zero;
            }
            else if(gridType == GridType.CARTESIAN)
            {
                gizmoHolder.transform.position = GridSnapper.GetGridCoordinate(gridCellSize, Selection.activeGameObject);
            }
            else
            {
                gizmoHolder.transform.position = GridSnapper.GetGridCoordinate(gridCellSize, gridAngularSize, Selection.activeGameObject);
            }

            Repaint();
        }

        private void OnSelectionChanged()
        {
            if (Selection.gameObjects.Length != 0)
            {
                CenterGizmoHolder();
            }
            else
            {
                DestroyImmediate(gizmoHolder);
            }
        }
    }
}