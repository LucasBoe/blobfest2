using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class CellBehaviour : IDealProvider
{
    public static CellType AssociatedCellType { get; } 
    protected BehaviourCellContext Context;
    protected Cell[] Neightbours => Context.Cell.Neightbours;
    public Deal[] Deals { get; protected set; }
    public bool HasDeals => Deals != null && Deals.Length > 0; 

    public void Init(BehaviourCellContext behaviourCellContext)
    {
        Context = behaviourCellContext;
    }
    public abstract void Enter();
    public abstract void Exit();
    public virtual void OnDelayedStart() { }
    public virtual void Update() { }

    protected T Instantiate<T>(T prefab, Vector2 position, List<T> optionalList = null) where T : Component
    {
        var instance = GameObject.Instantiate(prefab, position, Quaternion.identity, Context.Cell.ContentTransform);

        if (optionalList != null)
            optionalList.Add(instance);

        return instance;
    }
}
public class BehaviourCellContext
{
    public Cell Cell { get; set; }
}