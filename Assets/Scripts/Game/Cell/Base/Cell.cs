﻿using System;
using System.Collections.Generic;
using System.Linq;
using Engine;
using NaughtyAttributes;
using UnityEngine;
using VoronoiMap;

public partial class Cell : MonoBehaviour, IDelayedStartObserver, INPCPositionProvider
{
    [ReadOnly] public long GUID;
    [ReadOnly] public Vector2[] Edges;
    [ReadOnly] public Vector2[][] POIs;
    [SerializeField, ReadOnly] private CellType cellType;
    [ReadOnly] public long[] NeightbourGUIDs;
    [ReadOnly] public Cell[] Neightbours;

    public CellPixelSpriteGenerator HighligtPrrovider;
    public Vector2 Center => transform.position;
    public const int MAX_POI_COUNT = 10;

    public CellType CellType
    {
        get => cellType;
        private set => cellType = value;
    }

    public Transform ContentTransform;

    public CellBehaviour CurrentBehavior;
    public Engine.Event OnChangedCellTypeEvent = new();
    public Engine.Event OnTryRefreshEvent = new();

    internal static Cell CreateFromRawVoronoi(VoronoiCellData voronoiCellData, Transform root)
    {
        Cell cell = CellSpawner.Instance.SpawnNew();
        cell.name = voronoiCellData.GUID.ToString();
        cell.transform.parent = root;
        cell.transform.position = TransformToGamePos(voronoiCellData.Center);
        cell.Init(voronoiCellData);
        return cell;
    }

    public void DelayedStart()
    {
        if (CurrentBehavior != null)
            CurrentBehavior.OnDelayedStart();
    }

    private void Init(VoronoiCellData voronoiCellData)
    {
        Edges = TransformToGamePos(voronoiCellData.Edges);
        GUID = voronoiCellData.GUID;
        NeightbourGUIDs = voronoiCellData.NeightbourGUIDs;
        GeneratePOIS();
        HighligtPrrovider.GenerateSprites(Edges);
    }

    private void GeneratePOIS()
    {
        POIs = new Vector2[MAX_POI_COUNT][];
        for (int i = 0; i < MAX_POI_COUNT; i++)
        {
            if (i <= 1)
            {
                POIs[0] = new Vector2[0];
                POIs[1] = new[] { Center };
            }
            else
            {
                POIs[i] = NewPolygonUtil.CalculateDynamicPointsInPolygon(Edges, i).ToArray();
            }
        }
    }

    internal void ConnectNeightbours(Cell[] cells)
    {
        Neightbours = cells.Where(c => NeightbourGUIDs.Contains(c.GUID)).ToArray();
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

    public static Vector2[] TransformToGamePos(Vector2[] points)
    {
        Vector2[] result = new Vector2[points.Length];

        for (int i = 0; i < points.Length; i++)
            result[i] = TransformToGamePos(points[i]);

        return result;
    }

    public static Vector2[] TransformGameToTopPos(Vector2[] point)
    {
        Vector2[] result = new Vector2[point.Length];

        for (int i = 0; i < point.Length; i++)
            result[i] = TransformGameToTopPos(point[i]);

        return result;
    }

    public static Vector2 TransformToGamePos(Vector2 point)
    {
        return new Vector2(point.x, point.y / 2f);
    }

    public static Vector2 TransformGameToTopPos(Vector2 point)
    {
        return new Vector2(point.x, point.y * 2f);
    }

    #endregion

    private void Update()
    {
        if (CurrentBehavior != null)
            CurrentBehavior.Update();
    }

    public void ChangeCellType(CellType cellType, bool refreshBehaviour = true)
    {
        CellType = cellType;

        if (refreshBehaviour)
            RefreshBehaviourToMatchTypeFrom(MapDataUtil.GetAllCellBehaviourTypes());

        OnChangedCellTypeEvent?.Invoke();
    }

    public void ChangeBehaviourTo<T>() where T : CellBehaviour
    {
        ChangeBehaviourTo((T)Activator.CreateInstance(typeof(T)));
    }

    public void ChangeBehaviourTo(Type behaviourType)
    {
        if (Activator.CreateInstance(behaviourType) is CellBehaviour behaviour)
        {
            ChangeBehaviourTo(behaviour);
        }
    }

    private void ChangeBehaviourTo<T>(T behaviour) where T : CellBehaviour
    {
        if (CurrentBehavior != null)
            CurrentBehavior.Exit();

        CurrentBehavior = behaviour;
        CurrentBehavior.Init(new BehaviourCellContext()
        {
            Cell = this
        });

        CurrentBehavior.Enter();
    }

    internal void RefreshBehaviourToMatchTypeFrom(IEnumerable<Type> behaviourTypes)
    {
        Debug.Log("Starting RefreshBehaviourToMatchTypeFrom...");

        foreach (var bt in behaviourTypes)
        {
            var associatedCellTypeProp = bt.GetProperty("AssociatedCellType");
            if (associatedCellTypeProp == null)
            {
                Debug.Log($"Type {bt.Name} does not have an 'AssociatedCellType' property.");
                continue;
            }

            var value = associatedCellTypeProp.GetValue(null);
            Debug.Log($"Checking type: {bt.Name}, AssociatedCellType: {value}");

            if ((CellType)value == CellType)
            {
                Debug.Log($"Match found: {bt.Name}");
                ChangeBehaviourTo(bt);
                return;
            }
        }

        Debug.Log("No matching behavior type found.");
    }

    public Vector2 RequestPosition(NPCBehaviour npc) => Center;

    public Vector2[] GetPOIS(int i)
    {
        return POIs[i];
    }

    public void NotifyRefresh() => OnTryRefreshEvent?.Invoke();
}