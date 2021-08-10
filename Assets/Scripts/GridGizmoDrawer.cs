using UnityEngine;
using Utilities;

public class GridGizmoDrawer : MonoBehaviour
{
#if UNITY_EDITOR

    private static int renderSize;
    private static float cellSize;

    //private void Awake() => center = transform.position;

    //public static void SetCenter()
    //{
    //    if (Selection.gameObjects.Length == 1)
    //    {
    //        center = Selection.gameObjects[0].transform.position;
    //    }
    //    else
    //    {
    //        float cX = 0, cY = 0, cZ = 0;

    //        foreach (var go in Selection.gameObjects)
    //        {
    //            cX += go.transform.position.x;
    //            cY += go.transform.position.y;
    //            cZ += go.transform.position.z;
    //        }

    //        center = new Vector3(cX, cY, cZ) / Selection.gameObjects.Length;
    //    }
    //    Debug.Log(center);
    //}

    public static void SetRenderSize(int newRenderSize) => renderSize = newRenderSize;
    public static void SetCellSize(float newCellSize) => cellSize = newCellSize;
    public static void SetParameters(float newCellSize, int newRenderSize)
    {
        cellSize = newCellSize;
        renderSize = newRenderSize;
    }

    private void OnDrawGizmos()
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

#endif
}
