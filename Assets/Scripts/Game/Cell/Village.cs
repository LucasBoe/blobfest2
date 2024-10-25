using System;
using System.Collections.Generic;
using UnityEngine;



public class Village : CellBehaviour
{
    public static new CellType AssociatedCellType => CellType.Village;

    List<Transform> huts = new();
    private Procedure produceVillagersProcedure;


    public override void Enter()
    {
        huts = SpawnHuts();
    }
    public override void OnDelayedStart()
    {
        StartNewProcedure();
    }
    private void StartNewProcedure()
    {
        Debug.Log("Start New Procedure");

        produceVillagersProcedure = ProcedureHandler.Instance.StartNewProcedure(30)
            .At(Context.Cell)
            .WithReward(CardID.Villager)
            .WithCallback(StartNewProcedure);
    }

    private List<Transform> SpawnHuts()
    {
        List<Transform> huts = new();
        var prefab = PrefabRefID.Hut.TryGetPrefab<Transform>();

        foreach (var poi in Context.Cell.POIs)
             Instantiate(prefab, poi, huts);

        return huts;
    }
    public override void Exit()
    {
        foreach (var tree in huts)
            GameObject.Destroy(tree.gameObject);

        ProcedureHandler.Instance.Stop(produceVillagersProcedure);
    }
}
