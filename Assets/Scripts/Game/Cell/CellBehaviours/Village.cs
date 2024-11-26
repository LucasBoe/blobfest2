using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class Village : CellBehaviour, DynamicTimeProcecure.IProgressProvider
{
    public static new CellType AssociatedCellType => CellType.Village;
    public float ProgressMultiplier => (1 + directNeightboursthatAreFieldsCount) * .1f;
    private int directNeightboursthatAreFieldsCount = 0;

    List<Transform> huts = new();
    private ProcedureBase produceVillagersProcedure;

    public override void Enter()
    {
        Deals = new Deal[] { new Deal(CardID.MakeVillage, TokenID.Grain, 12) };
        huts = SpawnHut();
        TryStartNewProcedure();
        CollectibleSpawner.Instance.SpawnAt(CardID.Villager.ToCard(), Context.Cell.Center);

        foreach (var neightbour in Neightbours)
            neightbour.OnChangedCellTypeEvent.AddListener(OnChangedCellType);
    }
    public override void OnDelayedStart()
    {
        TryStartNewProcedure();
    }
    private void TryStartNewProcedure()
    {
        if (produceVillagersProcedure != null)
            return;

        Debug.Log("Start New Procedure");

        produceVillagersProcedure = ProcedureHandler.Instance.StartNewProcedure(this)
            .At(Context.Cell)
            .WithReward(CardID.Villager)
            .WithCallback(() =>
            {
                produceVillagersProcedure = null;
                TryStartNewProcedure();
            });
    }

    private List<Transform> SpawnHut()
    {
        List<Transform> huts = new();
        var prefab = PrefabRefID.Houses.TryGetPrefab<Transform>();
        var poi = Context.Cell.POIs.First();
        Instantiate(prefab, poi, huts);
        return huts;
    }
    private void OnChangedCellType()
    {
        directNeightboursthatAreFieldsCount = Neightbours.Where(n => n.CellType == CellType.Farmland).Count();
    }
    public override void Exit()
    {
        foreach (var tree in huts)
            GameObject.Destroy(tree.gameObject);

        foreach (var neightbour in Neightbours)
            neightbour.OnChangedCellTypeEvent.RemoveListener(OnChangedCellType);

        ProcedureHandler.Instance.Stop(produceVillagersProcedure);
    }
}
