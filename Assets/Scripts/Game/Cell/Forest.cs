using System;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourCellContext
{
    public Cell Cell { get; set; }
}

public abstract class CellBehaviour
{
    protected BehaviourCellContext Context;

    public void Init(BehaviourCellContext behaviourCellContext)
    {
        Context = behaviourCellContext;
    }
    public abstract void Enter();
    public abstract void Exit();
    public virtual void Update() {}
}

internal class Forest : CellBehaviour
{
    List<Tree> trees = new();
    public override void Enter()
    {
        trees = SpawnTrees();
    }
    private List<Tree> SpawnTrees()
    {
        List < Tree > trees = new();
        var prefab = PrefabRefID.Tree.TryGetPrefab<Tree>();

        foreach (var poi in Context.Cell.POIs)
        {
            var instance = GameObject.Instantiate(prefab, poi, Quaternion.identity, Context.Cell.ContentTransform);
            trees.Add(instance);
        }

        return trees;
    }

    public override void Exit()
    {
        foreach (var tree in trees)
        {
            GameObject.Destroy(tree.gameObject);
        }
    }
}
