using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using Event = Engine.Event;
using Object = UnityEngine.Object;

public class SettlementBuildingBehaviour : BuildingBehaviour, DynamicTimeProcecure.IProgressProvider
{
    [SerializeField] private Sprite fieldSprite;
    [SerializeField] private SpriteRenderer housesSpriteRenderer;
    [SerializeField] private List<ListWrapper<Sprite>> settlementSprites = new();
    public float ProgressMultiplier => FieldCount / 30f;
    public string ProgressText { get; private set; }
    public Event OnEfficiencyChangedEvent { get; }
    [ShowNativeProperty] public int FieldCount { get; private set; }
    public int Effiency => FieldCount * 100;
    public override void OnAddBuilding()
    {
        Refresh();
        TryStartNewProcedure();
        CollectibleSpawner.Instance.SpawnAt(CardID.Villager.ToCard(), Position);
        Context.Cell.OnTryRefreshEvent.AddListener(Refresh);
    }
    protected override void OnRemoveBuilding()
    {
        ProcedureHandler.Instance.Stop(Procedure);
        Context.Cell.OnTryRefreshEvent.AddListener(Refresh);
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

    public void Refresh()
    {
        int count = Context.Cell.Neightbours.Count(n => n.CellType == CellType.Farmland);
        UpdateFieldCount(count);

        int level = 0;
        if (Context.Cell.CurrentBehavior is SettlementBehaviour settlement)
            level = settlement.Level;
        
        housesSpriteRenderer.sprite = settlementSprites[Mathf.Min(level, 2)][Mathf.Min(3, count)];
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
    [System.Serializable]
    public class ListWrapper<T> where T : Object
    {
        public List<T> myList;
        public T this[int key]
        {
            get { return myList[key]; }
            set { myList[key] = value; }
        }

    }
}