using System;
using System.Collections.Generic;
using System.Drawing;
using Engine;
using UnityEngine;
using VoronoiMap;

public class MapHandler : SingletonBehaviour<MapHandler>
{
    [SerializeField] VoronoiMapGizmoDrawer gizmoDrawer;
    [SerializeField] MapData mapData;
    public MapData MapData { get => mapData; private set => mapData = value; }
    public bool HasMap => MapData != null;
    public Action<MapData> OnMapFinishedEvent;
    public Vector2 MapCenter;

    public void GenerateNewMap()
    {
        VoronoiMapData voronoiMap = new VoronoiMapData(new Vector2(50, 50), 10);
        voronoiMap.FindNeightbours();
        gizmoDrawer.Map = voronoiMap;

        MapData = new MapData(voronoiMap);

        //Modify Cell Types
        List<CellType> spawnCellTypePool = new()
        {
            CellType.Meadow,
            CellType.Forest
        };

        MapCenter = Vector2.zero;

        foreach (var cell in MapData.Cells)
        {
            cell.CellType = spawnCellTypePool.GetRandom();
            MapCenter += cell.Center;
        }


        foreach (var cell in MapData.Cells)
            if (cell.CellType == CellType.Forest)
                cell.ChangeBehaviourTo<Forest>();

        MapCenter /= MapData.Cells.Length;

        OnMapFinishedEvent?.Invoke(MapData);
    }
}

public enum CellType
{
    Undefined,
    Meadow,
    Forest,
}

[System.Serializable]
public class MapData
{
    public Cell[] Cells;
    public MapData(VoronoiMapData voronoiMap)
    {
        GameObject root = GameObject.Find("MapRoot");

        if (root == null)
            root = new GameObject("MapRoot");

        int cellCount = voronoiMap.Cells.Count;
        Cells = new Cell[cellCount];

        for (int i = 0; i < cellCount; i++)
            Cells[i] = Cell.CreateFromRawVoronoi(voronoiMap.Cells[i], root.transform);
    }
}

public class MapDataUtil
{
    public static Cell GetCellThatContainsPoint(MapData map, Vector2 pointInMapSpace)
    {
        foreach (var cell in map.Cells)
        {
            if (IsInPolygon(cell.Edges, pointInMapSpace))
                return cell;
        }
        return null;
    }
    public static bool IsInPolygon(Vector2[] poly, Vector2 p)
    {
        int n = poly.Length;
        bool inside = false;

        // Loop through each edge of the polygon
        for (int i = 0, j = n - 1; i < n; j = i++)
        {
            Vector2 vi = poly[i];
            Vector2 vj = poly[j];

            // Check if the point is on the inside of this edge
            if ((vi.y > p.y) != (vj.y > p.y) &&
                p.x < (vj.x - vi.x) * (p.y - vi.y) / (vj.y - vi.y) + vi.x)
            {
                inside = !inside;
            }
        }

        return inside;
    }
}