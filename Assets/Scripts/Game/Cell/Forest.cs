using System;
using System.Collections.Generic;
using UnityEngine;

internal class Forest : CellBehaviour, ICanReceive<Villager>
{
    List<Tree> trees = new();

    public bool HasActiveProcedure = false;

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
    public bool CanReceiveCard(Villager card)
    {
        return !HasActiveProcedure;
    }
}
