using System;
using UnityEngine;

public class CellSelectionUISlice : MonoBehaviour
{
    [SerializeField] private HeaderModule_CellSelectionUISlice header;
    [SerializeField] private BuildingsModule_CellSelectionUISlice buildings;
    [SerializeField] private InspectionModule_CellSelectionUISlice inspection;
    
    private Cell selectedCell;

    public void Init(Cell selectedCell)
    {
        header.Init(selectedCell, this);
        buildings.Init(selectedCell, this);
        inspection.Init(selectedCell, this);
        
        this.selectedCell = selectedCell;
    }
    public void TryInspectAt(int index)
    {
        inspection.TrySelectFromIndex(index);
    }
}

[Serializable]
public class InspectionModule_CellSelectionUISlice : CellSelectionUISliceModuleBase
{
    ICanBeInspected inspectionTarget;
    protected override void TryPopulate()
    {
        
    }
    protected override bool CheckShouldBeActive()
    {
        return inspectionTarget != null;
    }
    public void TrySelectFromIndex(int index)
    {
        if (Cell.CurrentBehavior is not Settlement village)
            return;

        if (!village.Buildings.TryGetAt(index, out var building))
            return;

        inspectionTarget = building;
        if (inspectionTarget == null)
            return;
        
        TryDisplay();
    }
}

public interface ICanBeInspected
{
    
}