using System;
using System.Collections.Generic;
using UnityEngine;

public class Forest : CellBehaviour, ICanReceive<Villager>
{
    public static new CellType AssociatedCellType => CellType.Forest;

    List<Tree> trees = new();

    public bool HasTrees => trees.Count > 0;
    public bool HasActiveProcedure => activeProcedure != null && activeProcedure.IsRunning;
    private ProcedureBase activeProcedure;

    public override void Enter()
    {
        trees = SpawnTrees();
    }
    private List<Tree> SpawnTrees()
    {
        List < Tree > trees = new();
        var prefab = PrefabRefID.Tree.TryGetPrefab<Tree>();

        foreach (var poi in Context.Cell.POIs) 
            Instantiate(prefab, poi, trees);

        return trees;
    }

    public override void Exit()
    {
        foreach (var tree in trees)
            UnityEngine.Object.Destroy(tree.gameObject);
    }
    public bool CanReceiveCard(Villager card) => !HasActiveProcedure && HasTrees;

    public bool TryReceiveCard(Villager card)
    {
        if (!CanReceiveCard(card))
            return false;

        activeProcedure = ProcedureHandler.Instance.StartNewProcedure(10)
            .At(Context.Cell)
            .WithNPC()
            .WithReward(TokenID.Wood)
            .WithCallback(() =>
            {
                var tree = trees[0];
                trees.Remove(tree);
                GameObject.Destroy(tree.gameObject);

                if (trees.Count == 0)
                    Context.Cell.ChangeCellType(CellType.Meadow);
            });

        CardPlayHandler.Instance.NotifyRefresh();
        return true;
    }
}
