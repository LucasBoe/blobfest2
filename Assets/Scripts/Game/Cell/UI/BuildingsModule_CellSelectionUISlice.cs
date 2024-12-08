using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class BuildingsModule_CellSelectionUISlice : CellSelectionUISliceModuleBase
{
    [SerializeField] private BuildingSlotUISlice dummy;
    [SerializeField] private BuildingInspectionModule_CellSelectionUISlice inspection;
    private SettlementBehaviour _settlementBehaviour;
    private List<BuildingSlotUISlice> uiInstances = new();
    protected override void TryPopulate()
    {
        var maxBuildings = _settlementBehaviour.Buildings.MaxCount;
        var populated = _settlementBehaviour.Buildings.Count;
        
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
            ui.Init(_settlementBehaviour.Buildings.GetAt(index), this);
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
    public void Inspect(BuildingBehaviour associatedBuildingBehaviour)
    {
        Debug.Log($"Inspect: {associatedBuildingBehaviour.Sourcecard.ID.ToString()}");
        
        if (associatedBuildingBehaviour != null)
            inspection.Show(associatedBuildingBehaviour);
        else
            inspection.Hide();
    }
    protected override bool CheckShouldBeActive()
    {
        if (Cell.CurrentBehavior is not SettlementBehaviour village)
            return false;

        this._settlementBehaviour = village;
        return true;
    }
}

[System.Serializable]
public class BuildingInspectionModule_CellSelectionUISlice
{
    [SerializeField] private TMP_Text buildingNameText;
    [SerializeField] private TokenFromToUIModule tokenFromTo;
    [SerializeField] private ProcedureUIModule procedureVisualizer;
    private BuildingBehaviour _buildingBehaviour;
    public void Show(BuildingBehaviour buildingBehaviour)
    {
        this._buildingBehaviour = buildingBehaviour;
        buildingNameText.text = buildingBehaviour.Sourcecard.ID.ToString();
        buildingNameText.gameObject.SetActive(true);
        tokenFromTo.Init(buildingBehaviour);
        procedureVisualizer.Init(buildingBehaviour);
        
        tokenFromTo.Show();
        procedureVisualizer.Show();
    }
    public void Hide()
    {
        tokenFromTo.Hide();
        procedureVisualizer.Hide();
        buildingNameText.gameObject.SetActive(false);
    }
}