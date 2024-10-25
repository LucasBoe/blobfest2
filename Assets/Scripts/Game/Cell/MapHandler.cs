using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
        var voronoiMap = GenerateVoronoiMap(new Vector2(50, 50), 10);
        gizmoDrawer.Map = voronoiMap;
        MapData = new MapData(voronoiMap);
        MapCenter = CalculateMapCenter(MapData.Cells);

        RandomizeCellTypes(MapData.Cells);
        SetVillageCell(MapData.Cells);
        SetMillCell(MapData.Cells);
        ApplyCellBehaviors(MapData.Cells);

        OnMapFinishedEvent?.Invoke(MapData);
    }



    private VoronoiMapData GenerateVoronoiMap(Vector2 size, int seed)
    {
        var voronoiMap = new VoronoiMapData(size, seed);
        voronoiMap.FindNeightbours();
        return voronoiMap;
    }
    private Vector2 CalculateMapCenter(Cell[] cells)
    {
        Vector2 center = Vector2.zero;
        foreach (var cell in cells)
        {
            center += cell.Center;
        }

        return center / cells.Length;
    }
    private void RandomizeCellTypes(Cell[] cells)
    {
        List<CellType> spawnCellTypePool = new() { CellType.Meadow, CellType.Forest };

        foreach (var cell in cells)
        {
            cell.CellType = spawnCellTypePool.GetRandom();
        }
    }
    private void SetVillageCell(Cell[] cells)
    {
        cells.OrderBy(c => Vector2.Distance(c.Center, MapCenter))
             .First()
             .CellType = CellType.Village;
    }
    private void SetMillCell(Cell[] cells)
    {
        var ordered = cells.OrderBy(c => Vector2.Distance(c.Center, MapCenter)).ToArray();
        ordered[Mathf.RoundToInt(ordered.Length / 6f)].CellType = CellType.Mill;
    }
    private void ApplyCellBehaviors(Cell[] cells)
    {
        // Get all types that inherit from CellBehaviour
        var behaviourTypes = typeof(CellBehaviour).Assembly.GetTypes()
            .Where(t => t.IsSubclassOf(typeof(CellBehaviour)) && !t.IsAbstract);

        foreach (var cell in cells)
        {
            // Find the behavior type that matches the cell's type
            var behaviourType = behaviourTypes.FirstOrDefault(bt =>
                (CellType)bt.GetProperty("AssociatedCellType").GetValue(null) == cell.CellType);

            if (behaviourType != null)
            {
                cell.ChangeBehaviourTo(behaviourType);
            }
        }
    }
}

public enum CellType
{
    Undefined,
    Meadow,
    Forest,
    Village,
    Mill,
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