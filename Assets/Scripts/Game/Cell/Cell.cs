using System;
using Engine;
using NaughtyAttributes;
using UnityEngine;
using VoronoiMap;

public partial class Cell : MonoBehaviour, IDelayedStartObserver
{
    [ReadOnly] public long GUID;
    [ReadOnly] public Vector2[] Edges;
    [ReadOnly] public Vector2[] POIs;
    [ReadOnly] public CellType CellType;
    [ReadOnly] public long[] NeightbourGUIDs;


    public CellPixelRenderer HighligtPrrovider;
    public Vector2 Center => transform.position;
    public Transform ContentTransform;

    public CellBehaviour CurrentBehavior;

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
        POIs = PolygonUtil.GetDynamicRandomPointsInPolygon(Edges, 3).ToArray();
        HighligtPrrovider.GenerateSprites(Edges);
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

        Gizmos.color = Color.yellow;
        for (int i = 0; i < POIs.Length; i++)
        {
            Gizmos.DrawWireSphere(POIs[i], .2f);
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

    private void Update()
    {
        if (CurrentBehavior != null)
            CurrentBehavior.Update();
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
    #endregion
}