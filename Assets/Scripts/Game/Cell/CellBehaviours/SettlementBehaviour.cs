﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;


public class SettlementBehaviour : CellBehaviour, ICanReceive<Stonemason>
{
    public static new CellType AssociatedCellType => CellType.Settlement;
    public SettlementBuildingModule Buildings = new();
    public float ProgressMultiplier => (1 + directNeightboursthatAreFieldsCount) * .1f;
    private int directNeightboursthatAreFieldsCount = 0;

    List<Transform> buildingsTransforms = new();
    public override void Enter()
    {
        Buildings.Handover(AddBuildingVisualsCallback);
        Deals = new Deal[] { new Deal(CardID.Settlement, TokenID.Grain, 12) };
        Buildings.Place(CardID.Settlement.ToCard());

        foreach (var neightbour in Neightbours)
            neightbour.OnChangedCellTypeEvent.AddListener(OnChangedCellType);
    }

    private BuildingBehaviour AddBuildingVisualsCallback(BuildingBehaviour prefab)
    {
        BuildingBehaviour instance = GameObject.Instantiate(prefab);
        instance.GiveContext(Context);
        buildingsTransforms.Add(instance.transform);
        
        var pois = Context.Cell.GetPOIS(buildingsTransforms.Count);
        for (int i = 0; i < pois.Length; i++)
        {
            buildingsTransforms[i].position = pois[i];
            if (buildingsTransforms[i].TryGetComponent<StaticZOffset>(out var zOffset))
                zOffset.SceduleZUpdate();
        }

        return instance;
    }
    private void OnChangedCellType()
    {
        directNeightboursthatAreFieldsCount = Neightbours.Where(n => n.CellType == CellType.Farmland).Count();
    }
    public override void Exit()
    {
        foreach (var building in buildingsTransforms)
            GameObject.Destroy(building.gameObject);

        foreach (var neightbour in Neightbours)
            neightbour.OnChangedCellTypeEvent.RemoveListener(OnChangedCellType);
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
    private Func<BuildingBehaviour, BuildingBehaviour> placementCallback;
    public List<BuildingBehaviour> PlacedBuildings = new();
    public bool HasEmptySlots => Count < MaxCount;
    public int Count => PlacedBuildings.Count;
    public int MaxCount = 2;
    public void Place(Card card)
    {
        if (BuildingProvider.Instance.TryGetPrefab(card, out var prefab))
        {
            var placed = placementCallback?.Invoke(prefab);
            PlacedBuildings.Add(placed);
            placed.OnAddBuilding();
        }
    }
    public void Handover(Func<BuildingBehaviour, BuildingBehaviour> placementCallback) => this.placementCallback = placementCallback;
    public BuildingBehaviour GetAt(int index)
    {
        if (index < 0 || index >= PlacedBuildings.Count)
            return null;
        
        return PlacedBuildings[index];
    }   
    public bool TryGetAt(int index, out BuildingBehaviour buildingBehaviour)
    {
        if (index < 0 || index >= PlacedBuildings.Count)
        {
            buildingBehaviour = null;
            return false;
        }

        buildingBehaviour = PlacedBuildings[index];
        return true;
    }
}
public abstract class BuildingBehaviour : MonoBehaviour, IPositionProvider, IFromToDataProvider, IProcedureProvider
{
    protected BehaviourCellContext Context;
    public Card Sourcecard;
    public ProcedureBase Procedure { get; protected set; }
    public Vector2 Position => transform.position;
    public void GiveContext(BehaviourCellContext context) => Context = context;
    private void OnDisable() => OnRemoveBuilding();
    public abstract void OnAddBuilding();
    protected abstract void OnRemoveBuilding();
    public virtual bool TryGetFromToData(out FromToData data)
    {
        data = default;
        return false;
    }
}
public interface IProcedureProvider
{
    public ProcedureBase Procedure { get; }
}