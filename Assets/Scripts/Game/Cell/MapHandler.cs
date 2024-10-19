using System;
using System.Collections.Generic;
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
        VoronoiMapData voronoiMap = new VoronoiMapData(new Vector2(50, 50), 20);
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
        Vector2 p1, p2;
        bool inside = false;

        if (poly.Length < 3)
        {
            return inside;
        }

        var oldPoint = new Vector2(
            poly[poly.Length - 1].x, poly[poly.Length - 1].y);

        for (int i = 0; i < poly.Length; i++)
        {
            var newPoint = new Vector2(poly[i].x, poly[i].y);

            if (newPoint.x > oldPoint.x)
            {
                p1 = oldPoint;
                p2 = newPoint;
            }
            else
            {
                p1 = newPoint;
                p2 = oldPoint;
            }

            if ((newPoint.x < p.x) == (p.x <= oldPoint.x)
                && (p.y - (long)p1.y) * (p2.x - p1.x)
                < (p2.y - (long)p1.y) * (p.x - p1.x))
            {
                inside = !inside;
            }

            oldPoint = newPoint;
        }

        return inside;
    }
}