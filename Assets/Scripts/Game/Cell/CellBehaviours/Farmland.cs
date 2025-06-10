using System;
using System.Collections.Generic;
using UnityEngine;

public class Farmland : CellBehaviour, ICanReceive<Villager>
{
    public static new CellType AssociatedCellType => CellType.Farmland;

    List<Grain> grain = new();

    public bool HasGrainTrees => grain.Count > 0;
    public bool HasActiveProcedure => activeProcedure != null && activeProcedure.IsRunning;
    private ProcedureBase activeProcedure;

    public override void Enter()
    {
        grain = SpawnGrain();
        foreach (var neightbour in this.Context.Cell.Neightbours)
        {
            neightbour.NotifyRefresh();
        }
    }
    private List<Grain> SpawnGrain()
    {
        List < Grain > trees = new();
        var prefab = PrefabRefID.Grain.TryGetPrefab<Grain>();

        foreach (var poi in Context.Cell.GetPOIS(3)) 
            Instantiate(prefab, poi, trees);

        return trees;
    }

    public override void Exit()
    {
        foreach (var tree in grain)
            UnityEngine.Object.Destroy(tree.gameObject);
    }
    public bool CanReceiveCard(Villager card) => !HasActiveProcedure && HasGrainTrees;

    public void DoReceiveCard(Villager card)
    {
        activeProcedure = ProcedureHandler.Instance.StartNewProcedure(10)
            .At(Context.Cell)
            .WithNPC()
            .WithReward(TokenID.Grain)
            .WithCallback(() =>
            {

            });

        CardPlayHandler.Instance.NotifyRefresh();
    }
}
