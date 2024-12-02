using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
internal class BuildingsModule_CellSelectionUISlice : CellSelectionUISliceModuleBase
{
    [SerializeField] private BuildingSlotUISlice dummy;
    private Village village;
    private List<BuildingSlotUISlice> buildings = new();
    protected override void TryPopulate()
    {
        buildings = CreateSlots(4);

        var populated = village.GetNumberOfBuildings();
        for (var index = 0; index < buildings.Count; index++)
        {
            var building = buildings[index];
            building.SetState(index < populated ? BuildingSlotUISlice.SlotState.Full : BuildingSlotUISlice.SlotState.Empty);
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
        if (Cell.CurrentBehavior is not Village village)
            return false;

        this.village = village;
        return true;
    }
}