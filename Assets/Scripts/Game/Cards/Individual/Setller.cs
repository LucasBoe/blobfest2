using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Setller : Card
{
    public override PrefabRefID BuildingPrefabRefID => PrefabRefID.Houses;
    public override void EndValidation(CardValidationContext context)
    {
        CellHighlightHandler.Instance.DestroyAllHighlights();
    }
    public override bool TryPlay(CardValidationContext context)
    {
        if (context.CurrentHoverCell.CellType == CellType.Meadow)
        {
            context.CurrentHoverCell.ChangeCellType(CellType.Settlement);
            return true;
        }

        if (context.CurrentHoverCell.CellType == CellType.Settlement)
        {
            if (context.CurrentHoverCell.CurrentBehavior is ICanReceive<Setller> settler &&
                settler.CanReceiveCard(this))
            {
                settler.DoReceiveCard(this);
                return true;
            }
        }
        return false;
    }
    public override bool RefreshValidation(CardValidationContext context)
    {
        CellHighlightHandler.Instance.DestroyAllHighlights();
        List<Cell> validCells = context.Map.Cells.FilterByCellType(CellType.Meadow);

        foreach (var cell in context.Map.Cells.FilterByCellType(CellType.Settlement).Where(c => c.CurrentBehavior is ICanReceive<Setller> setller && setller.CanReceiveCard(this)))
            validCells.Add(cell);
        
        CellHighlightHandler.Instance.CreateHighlightsFor(validCells, Color.green);

        return validCells.Contains(context.CurrentHoverCell);
    }
}