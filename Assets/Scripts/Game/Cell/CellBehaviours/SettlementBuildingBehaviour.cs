using System;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using Event = Engine.Event;

public class SettlementBuildingBehaviour : BuildingBehaviour, DynamicTimeProcecure.IProgressProvider
{
    [SerializeField] private Sprite fieldSprite;
    public float ProgressMultiplier => FieldCount / 30f;
    public string ProgressText { get; private set; }
    public Event OnEfficiencyChangedEvent { get; }
    [ShowNativeProperty] public int FieldCount { get; private set; }
    public int Effiency => FieldCount * 100;
    public override void OnAddBuilding()
    {
        RefreshFieldCountFromNeightbours();
        TryStartNewProcedure();
        CollectibleSpawner.Instance.SpawnAt(CardID.Villager.ToCard(), Position);
        Context.Cell.OnTryRefreshEvent.AddListener(RefreshFieldCountFromNeightbours);
    }
    protected override void OnRemoveBuilding()
    {
        ProcedureHandler.Instance.Stop(Procedure);
        Context.Cell.OnTryRefreshEvent.AddListener(RefreshFieldCountFromNeightbours);
    }
    private void TryStartNewProcedure()
    {
        if (Procedure != null)
            return;

        Debug.Log("Start New Procedure");

        Procedure = ProcedureHandler.Instance.StartNewProcedure(this)
            .At(Context.Cell)
            .WithReward(CardID.Villager)
            .WithCallback(() =>
            {
                Procedure = null;
                TryStartNewProcedure();
            });
    }

    public void RefreshFieldCountFromNeightbours()
    {
        int count = Context.Cell.Neightbours.Count(n => n.CellType == CellType.Farmland);
        UpdateFieldCount(count);
    }
    public void UpdateFieldCount(int fieldCount)
    {
        if (this.FieldCount == fieldCount)
            return;
        
        this.FieldCount = fieldCount;
        this.ProgressText = fieldCount < 1 ? "missing fields" : $"{Effiency.ToString()}%";
        OnEfficiencyChangedEvent?.Invoke();
    }
    public override bool TryGetFromToData(out FromToData data)
    {
        data = new FromToData(fieldSprite, FieldCount, CardID.Villager.ToCard().GetIcon(), FieldCount);
        return true;
    }
}