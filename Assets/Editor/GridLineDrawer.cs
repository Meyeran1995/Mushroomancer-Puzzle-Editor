using UnityEngine;
using Utilities;

namespace GridTool
{
    public enum GridType { CARTESIAN, POLAR }

    public enum GridDrawMode { WORLDSPACE, LOCALSPACE}

    public struct Line3
    {
        public Line3(Vector3 begin, Vector3 end)
        {
            Begin = begin;
            End = end;
        }
        public Vector3 Begin, End;
    }

    public static class GridLineDrawer
    {
        public static Line3[] GetGizmoLines(GridSnapperWindow snapTool)
        {
            if (snapTool.GridType == GridType.CARTESIAN)
            {
                //draw horizontal lines
                Vector3 lengthVec = Vector3.right * snapTool.GridCellSize * snapTool.GridRenderSize / 2,
                    moveVec = Vector3.forward * snapTool.GridCellSize;

                Line3[] horizontalLines = DrawGridLines(snapTool.GridPivot, lengthVec, moveVec, snapTool.GridRenderSize);

                //draw vertical lines
                lengthVec = Vector3.forward * snapTool.GridCellSize * snapTool.GridRenderSize / 2;
                moveVec = Vector3.right * snapTool.GridCellSize;

                Line3[] verticalLines = DrawGridLines(snapTool.GridPivot, lengthVec, moveVec, snapTool.GridRenderSize);

                return GizmoUtils.MergeArrays(horizontalLines, verticalLines);
            }
            else
            {
                return DrawRotatedGridLines(snapTool.GridPivot,
                    Vector3.right * snapTool.GridCellSize * snapTool.GridRenderSize / 2, snapTool.GridAngularSize);
            }
        }

        private static Line3[] DrawGridLines(Vector3 pivotPosition, Vector3 lengthVec, Vector3 moveVec, int renderSize)
        {
            Vector3 lineOrigin;
            if (renderSize % 2 != 0)
                renderSize += 1;
            Line3[] lines = new Line3[renderSize];
            int half = renderSize / 2;

            for (int i = 0; i < renderSize / 2; i++)
            {
                lineOrigin = pivotPosition + moveVec * i;
                lines[i] = new Line3(lineOrigin + lengthVec, lineOrigin - lengthVec);
            }

            for (int i = 0; i < half; i++)
            {
                lineOrigin = pivotPosition - moveVec * i;
                lines[i + half] = new Line3(lineOrigin + lengthVec, lineOrigin - lengthVec);
            }

            return lines;
        }

        private static Line3[] DrawRotatedGridLines(Vector3 pivotPosition, Vector3 lengthVec, int angularSize)
        {
            if (angularSize == 0)
                angularSize = 90;

            Line3[] lines = new Line3[360 / angularSize];

            for (int a = 0; a < lines.Length; a++)
            {
                lines[a] = new Line3(pivotPosition,
                    pivotPosition + Quaternion.AngleAxis(a * angularSize, Vector3.up) * lengthVec);
            }

            return lines;
        }
    }
}
