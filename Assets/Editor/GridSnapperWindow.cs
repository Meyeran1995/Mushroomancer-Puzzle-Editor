using UnityEditor;
using UnityEngine;

namespace GridTool
{
    public class GridSnapperWindow : EditorWindow
    {
        #region Name Constants

        private const string EDITOR_GRID_CELL_SIZE = "GridCellSize";
        private const string EDITOR_GRID_RENDER_SIZE = "GridRenderSize";
        private const string EDITOR_GRID_TYPE = "GridType";
        private const string EDITOR_GRID_DRAW_MODE = "GridDrawMode";
        private const string EDITOR_GRID_ANGULAR_SIZE = "GridAngularSize";

        #endregion

        public float GridCellSize; // needs to be serialized to be recorded
        public int GridAngularSize;
        
        public int GridRenderSize;
        public Vector3 GridPivot;
        public GridType GridType;
        public GridDrawMode GridDrawMode;

        private SerializedObject windowFieldData;
        private SerializedProperty gridSizeProperty, gridRenderSizeProperty, gridAngularSizeProperty, gridTypeProperty, gridDrawModeProperty;

        [MenuItem("Window/Grid Snap Tool")]
        public static void ShowWindow() => GetWindow<GridSnapperWindow>("Grid Snap Tool");

        private void OnEnable()
        {
            GridCellSize = EditorPrefs.GetFloat(EDITOR_GRID_CELL_SIZE);
            GridRenderSize = EditorPrefs.GetInt(EDITOR_GRID_RENDER_SIZE);
            GridType = (GridType)EditorPrefs.GetInt(EDITOR_GRID_TYPE);
            GridDrawMode = (GridDrawMode)EditorPrefs.GetInt(EDITOR_GRID_DRAW_MODE);
            GridAngularSize = EditorPrefs.GetInt(EDITOR_GRID_ANGULAR_SIZE);

            windowFieldData = new SerializedObject(this);
            gridSizeProperty = windowFieldData.FindProperty(EDITOR_GRID_CELL_SIZE);
            gridRenderSizeProperty = windowFieldData.FindProperty(EDITOR_GRID_RENDER_SIZE);
            gridTypeProperty = windowFieldData.FindProperty(EDITOR_GRID_TYPE);
            gridDrawModeProperty = windowFieldData.FindProperty(EDITOR_GRID_DRAW_MODE);
            gridAngularSizeProperty = windowFieldData.FindProperty(EDITOR_GRID_ANGULAR_SIZE);

            Selection.selectionChanged += OnSelectionChanged;
            SceneView.duringSceneGui += OnDuringSceneGUI;
        }

        private void OnDisable()
        {
            Selection.selectionChanged -= OnSelectionChanged;
            SceneView.duringSceneGui -= OnDuringSceneGUI;
        }

        private void OnGUI()
        {
            GUILayout.Label("Grid Settings");

            windowFieldData.Update();
            GUILayout.Label("Visual Settings");
            EditorGUILayout.PropertyField(gridRenderSizeProperty);
            EditorGUILayout.PropertyField(gridTypeProperty);
            EditorGUILayout.PropertyField(gridDrawModeProperty);
            
            GUILayout.Label("Grid Settings");
            EditorGUILayout.PropertyField(gridSizeProperty);
            if(GridType == GridType.POLAR)
                EditorGUILayout.PropertyField(gridAngularSizeProperty);
            windowFieldData.ApplyModifiedProperties();

            using (new EditorGUI.DisabledScope(Selection.gameObjects.Length == 0))
            {
                if (!GUILayout.Button("Snap Selection")) return;

                if (GridType == GridType.CARTESIAN)
                {
                    GridSnapper.SnapToGrid(GridCellSize);
                }
                else
                {
                    GridSnapper.SnapToGrid(GridCellSize, GridAngularSize);
                }
            }
        }

        private void OnDuringSceneGUI(SceneView sceneView)
        {
            if (GridDrawMode == GridDrawMode.WORLDSPACE && Selection.gameObjects.Length == 0)
                GridPivot = Handles.PositionHandle(GridPivot, Quaternion.identity);

            foreach (var line in GridLineDrawer.GetGizmoLines(this))
            {
                Handles.DrawLine(line.Begin, line.End);
            }

            if (GridType == GridType.POLAR)
            {
                for (int i = 1; i < GridRenderSize / 2 + 1; i++)
                {
                    Handles.DrawWireDisc(GridPivot, Vector3.up, GridCellSize * i);
                }
            }
        }

        private void OnDestroy()
        {
            EditorPrefs.SetFloat(EDITOR_GRID_CELL_SIZE, GridCellSize); // gets called when window is closed
            EditorPrefs.SetInt(EDITOR_GRID_ANGULAR_SIZE, GridAngularSize);
            EditorPrefs.SetInt(EDITOR_GRID_RENDER_SIZE, GridRenderSize);
            EditorPrefs.SetInt(EDITOR_GRID_TYPE, (int)GridType);
            EditorPrefs.SetInt(EDITOR_GRID_DRAW_MODE, (int)GridDrawMode);
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

        private void OnSelectionChanged()
        {
            if (GridDrawMode == GridDrawMode.WORLDSPACE || Selection.gameObjects.Length == 0) return;

            if(GridType == GridType.CARTESIAN)
            {
                GridPivot = GridSnapper.GetGridCoordinate(GridCellSize, Selection.activeGameObject);
            }
            else
            {
                GridPivot = GridSnapper.GetGridCoordinate(GridCellSize, GridAngularSize, Selection.activeGameObject);
            }

            Repaint();
        }
    }
}