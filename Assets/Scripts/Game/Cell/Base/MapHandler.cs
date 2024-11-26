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
        //SetVillageCell(MapData.Cells);
        SetCellTyeWithMultiplier(MapData.Cells, 3f, CellType.Stonefield);
        SetCellTyeWithMultiplier(MapData.Cells, 6f, CellType.Mill);
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
            cell.ChangeCellType(spawnCellTypePool.GetRandom(), refreshBehaviour: false);
        }
    }
    //deprecated as the "make village" card is handed to the player at game start
    private void SetVillageCell(Cell[] cells)
    {
        cells.OrderBy(c => Vector2.Distance(c.Center, MapCenter))
             .First()
             .ChangeCellType(CellType.Village, refreshBehaviour: false);
    }
    private void SetCellTyeWithMultiplier(Cell[] cells, float multiplier, CellType type)
    {
        var ordered = cells.OrderBy(c => Vector2.Distance(c.Center, MapCenter)).ToArray();
        ordered[Mathf.RoundToInt(ordered.Length / multiplier)].ChangeCellType(type, refreshBehaviour: false);
    }
    private void ApplyCellBehaviors(Cell[] cells)
    {
        IEnumerable<Type> behaviourTypes = MapDataUtil.GetAllCellBehaviourTypes();

        foreach (var cell in cells)
            cell.RefreshBehaviourToMatchTypeFrom(behaviourTypes);
    }
}

public enum CellType
{
    Undefined,
    Meadow,
    Forest,
    Village,
    Mill,
    Farmland,
    Stonefield,
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

        foreach (var cell in Cells)
            cell.ConnectNeightbours(Cells);
    }
}

public static class MapDataUtil
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

    public static List<Cell> FilterByCellType(this Cell[] cells, params CellType[] types)
    {
        List<Cell> validCells = new();
        foreach (var cell in cells)
        {
            if (!types.Contains(cell.CellType))
                continue;

            validCells.Add(cell);
        }

        return validCells;
    }

    internal static IEnumerable<Type> GetAllCellBehaviourTypes()
    {
            // Get all types that inherit from CellBehaviour
            return typeof(CellBehaviour).Assembly.GetTypes()
                .Where(t => t.IsSubclassOf(typeof(CellBehaviour)) && !t.IsAbstract);
        
    }
}