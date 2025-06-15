using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : Card
{
    public override PrefabRefID BuildingPrefabRefID => PrefabRefID.StonemasonHut;
    public override void EndValidation(CardValidationContext context)
    {
        CellHighlightHandler.Instance.DestroyAllHighlights();
    }
    public override bool TryPlay(CardValidationContext context)
    {
        context.CurrentHoverCell.ChangeCellType(CellType.ConstructionSite);
        return true;
    }
    public override bool RefreshValidation(CardValidationContext context)
    {
        CellHighlightHandler.Instance.DestroyAllHighlights();
        List<Cell> validCells = context.Map.Cells.FilterByCellType(CellType.Meadow, CellType.Forest, CellType.Stonefield);
        CellHighlightHandler.Instance.CreateHighlightsFor(validCells, Color.green);

        return validCells.Contains(context.CurrentHoverCell);
    }
}