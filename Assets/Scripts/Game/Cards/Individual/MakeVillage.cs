using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeVillage : Card
{
    public const CellType VALID = CellType.Meadow;

    public override void EndValidation(CardValidationContext context)
    {
        CellHighlightHandler.Instance.DestroyAllHighlights();
    }

    public override bool TryPlay(CardValidationContext context)
    {
        if (context.CurrentPlayerCell.CellType != VALID)
            return false;

        context.CurrentPlayerCell.ChangeCellType(CellType.Village);
        return true;
    }

    public override bool RefreshValidation(CardValidationContext context)
    {
        CellHighlightHandler.Instance.DestroyAllHighlights();

        List<Cell> validCells = context.Map.Cells.FilterByCellType(VALID);

        CellHighlightHandler.Instance.CreateHighlightsFor(validCells, Color.green);

        return validCells.Contains(context.CurrentPlayerCell);
    }
}