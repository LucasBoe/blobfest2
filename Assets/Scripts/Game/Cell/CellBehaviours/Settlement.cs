using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;


public class Settlement : CellBehaviour, DynamicTimeProcecure.IProgressProvider, ICanReceive<Stonemason>
{
    public static new CellType AssociatedCellType => CellType.Settlement;
    public SettlementBuildingModule Buildings = new();
    public float ProgressMultiplier => (1 + directNeightboursthatAreFieldsCount) * .1f;
    private int directNeightboursthatAreFieldsCount = 0;

    List<Transform> buildingsTransforms = new();
    private ProcedureBase produceVillagersProcedure;
    public override void Enter()
    {
        Buildings.Handover(AddBuildingVisualsCallback);
        Deals = new Deal[] { new Deal(CardID.Settlement, TokenID.Grain, 12) };
        Buildings.Place(CardID.Settlement.ToCard());
        TryStartNewProcedure();
        CollectibleSpawner.Instance.SpawnAt(CardID.Villager.ToCard(), Context.Cell.Center);

        foreach (var neightbour in Neightbours)
            neightbour.OnChangedCellTypeEvent.AddListener(OnChangedCellType);
    }

    private void AddBuildingVisualsCallback(PrefabRefID id)
    {
        Transform newBuildingVisuals = GameObject.Instantiate(id.TryGetPrefab<Transform>());
        buildingsTransforms.Add(newBuildingVisuals);
        
        var pois = Context.Cell.GetPOIS(buildingsTransforms.Count);
        for (int i = 0; i < pois.Length; i++)
        {
            buildingsTransforms[i].position = pois[i];
            if (buildingsTransforms[i].TryGetComponent<StaticZOffset>(out var zOffset))
                zOffset.SceduleZUpdate();
        }
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
    private void OnChangedCellType()
    {
        directNeightboursthatAreFieldsCount = Neightbours.Where(n => n.CellType == CellType.Farmland).Count();
    }
    public override void Exit()
    {
        foreach (var tree in buildingsTransforms)
            GameObject.Destroy(tree.gameObject);

        foreach (var neightbour in Neightbours)
            neightbour.OnChangedCellTypeEvent.RemoveListener(OnChangedCellType);

        ProcedureHandler.Instance.Stop(produceVillagersProcedure);
    }
    public bool CanReceiveCard(Stonemason card)
    {
        return Buildings.HasEmptySlots;
    }
    public void DoReceiveCard(Stonemason card)
    {
        Buildings.Place(card);
    }
}
[Serializable]
public class SettlementBuildingModule
{
    private Action<PrefabRefID> placementCallback;
    public List<PlacedBuilding> PlacedBuildings = new();
    public bool HasEmptySlots => Count < MaxCount;
    public int Count => PlacedBuildings.Count;
    public int MaxCount = 2;
    public void Place(Card card)
    {
        PlacedBuildings.Add(new PlacedBuilding(card));

        var refID = card.BuildingPrefabRefID;
        
        if (refID != PrefabRefID.None)
            placementCallback?.Invoke(refID);
    }
    public void Handover(Action<PrefabRefID> placementCallback) => this.placementCallback = placementCallback;
    public PlacedBuilding GetAt(int index) => PlacedBuildings[index];
}
public class PlacedBuilding
{
    public Card Sourcecard;

    public PlacedBuilding(Card card)
    {
        Sourcecard = card;
    }
}