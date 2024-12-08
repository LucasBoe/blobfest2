using System;
using UnityEngine;

public class SettlementBuildingBehaviour : BuildingBehaviour, DynamicTimeProcecure.IProgressProvider
{
    [SerializeField] private Sprite fieldSprite;
    public float ProgressMultiplier => 1f / 30f;
    public override void OnAddBuilding()
    {
        TryStartNewProcedure();
        CollectibleSpawner.Instance.SpawnAt(CardID.Villager.ToCard(), Position);
    }
    protected override void OnRemoveBuilding()
    {
        ProcedureHandler.Instance.Stop(Procedure);
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

    public override bool TryGetFromToData(out FromToData data)
    {
        data = new FromToData(fieldSprite, 1, CardID.Villager.ToCard().GetIcon(), 1);
        return true;
    }
}