using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
internal class BuildingsModule_CellSelectionUISlice : CellSelectionUISliceModuleBase
{
    [SerializeField] private BuildingSlotUISlice dummy;
    private Settlement _settlement;
    private List<BuildingSlotUISlice> uiInstances = new();
    protected override void TryPopulate()
    {
        var maxBuildings = _settlement.Buildings.MaxCount;
        var populated = _settlement.Buildings.Count;
        
        uiInstances = CreateSlots(4);

        for (var index = 0; index < uiInstances.Count; index++)
        {
            var ui = uiInstances[index];
            
            if (index >= maxBuildings)
            {
                ui.SetState(BuildingSlotUISlice.SlotState.Blocked);
                continue;
            }
            
            if (index >= populated)
            {
                ui.SetState(BuildingSlotUISlice.SlotState.Empty);
                continue;
            }
            
            ui.SetState(BuildingSlotUISlice.SlotState.Full);
            ui.Init(_settlement.Buildings.GetAt(index));
        }

        List<BuildingSlotUISlice> CreateSlots(int amount)
        {
            List<BuildingSlotUISlice> result = new();
            result.Add(dummy);
            for (int i = 0; i < amount - 1; i++)
            {
                var slotInstance = GameObject.Instantiate(dummy, dummy.transform.parent);
                result.Add(slotInstance);
            }
            return result;
        }
    }
    protected override bool CheckShouldBeActive()
    {
        if (Cell.CurrentBehavior is not Settlement village)
            return false;

        this._settlement = village;
        return true;
    }
}