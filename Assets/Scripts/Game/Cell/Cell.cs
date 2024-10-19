using System;
using EditorAttributes;
using UnityEngine;
using VoronoiMap;

public class Cell : MonoBehaviour
{
    [ReadOnly] public long GUID;
    [ReadOnly] public Vector2[] Edges;
    [ReadOnly] public CellType CellType;
    [ReadOnly] public long[] NeightbourGUIDs;

    public Vector2 Center => transform.position;

    internal static Cell CreateFromRawVoronoi(VoronoiCellData voronoiCellData, Transform root)
    {
        GameObject cellObject = new GameObject(voronoiCellData.GUID.ToString());
        cellObject.transform.parent = root;
        cellObject.transform.position = TransformToGamePos(voronoiCellData.Center);
        Cell cell = cellObject.AddComponent<Cell>();
        cell.Init(voronoiCellData);
        return cell;
    }
    private void Init(VoronoiCellData voronoiCellData)
    {
        Edges = TransformToGamePos(voronoiCellData.Edges);
        GUID = voronoiCellData.GUID;
        NeightbourGUIDs = voronoiCellData.NeightbourGUIDs;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;

        for (int i = 0; i < Edges.Length; i++)
        {
            var a = Edges[i];
            var b = Edges[(i + 1) == Edges.Length ? 0 : i + 1];

            Gizmos.DrawLine(a, b);
        }
    }
    #region SPACE TRANSFORMATION
    private Vector2[] TransformToGamePos(Vector2[] edges)
    {
        Vector2[] result = new Vector2[edges.Length];

        for (int i = 0; i < edges.Length; i++)
        {
            result[i] = TransformToGamePos(edges[i]);
        }

        return result;
    }
    private static Vector2 TransformToGamePos(Vector2 raw)
    {
        return new Vector2(raw.x, raw.y / 2f);
    }
    #endregion
}