using System;
using System.Collections.Generic;
using UnityEngine;

internal class Forest : CellBehaviour, ICanReceive<Villager>
{
    List<Tree> trees = new();

    public bool HasTrees => trees.Count > 0;
    public bool HasActiveProcedure => activeProcedure != null && activeProcedure.IsRunning;
    private Procedure activeProcedure;

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
    public bool CanReceiveCard(Villager card) => !HasActiveProcedure && HasTrees;

    public bool TryReceiveCard(Villager card)
    {
        if (!CanReceiveCard(card))
            return false;

        activeProcedure = ProcedureHandler.Instance.StartNewProcedure(Context.Cell, 10, card, TokenID.Wood, () =>
        {
            var tree = trees[0];
            trees.Remove(tree);
            GameObject.Destroy(tree.gameObject);
        });

        CardPlayHandler.Instance.NotifyRefresh();
        return true;
    }
}
