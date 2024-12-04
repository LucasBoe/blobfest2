using System.Collections.Generic;
using Engine;
using UnityEngine;

public class GenericCard<T> : Card where T : Card
{
    public override void EndValidation(CardValidationContext context)
    {
        CellHighlightHandler.Instance.DestroyAllHighlights();
    }
    public override bool TryPlay(CardValidationContext context)
    {
        if (context.CurrentPlayerCell.CurrentBehavior is not ICanReceive<T> receiver)
            return false;

        if (!receiver.CanReceiveCard(this as T))
            return false;
        
        receiver.DoReceiveCard(this as T);
        return true;
    }
    public override bool RefreshValidation(CardValidationContext context)
    {
        CellHighlightHandler.Instance.DestroyAllHighlights();

        List<Cell> validCells = FindValidCells(context);

        CellHighlightHandler.Instance.CreateHighlightsFor(validCells, Color.green);

        return validCells.Contains(context.CurrentPlayerCell);
    }

    private List<Cell> FindValidCells(CardValidationContext context)
    {
        List<Cell> validCells = new();
        foreach (var cell in context.Map.Cells)
        {
            if (cell.CurrentBehavior is not ICanReceive<T> receiver)
                continue;

            if (receiver.CanReceiveCard(this as T))
                validCells.Add(cell);
        }

        return validCells;
    }
}

internal class CellHighlightHandler : Singleton<CellHighlightHandler>
{
    List<CellHighlight> activeHighlights = new();
    internal void CreateHighlightsFor(List<Cell> cells, Color color)
    {
        foreach (var cell in cells)
        {
            var original = cell.HighligtPrrovider.Fill;
            var instance = GameObject.Instantiate(original, original.transform.parent);
            instance.color = color;
            instance.enabled = true;
            activeHighlights.Add(new CellHighlight()
            {
                HighlightRendererInstance = instance,
            });
        }
    }
    internal void DestroyAllHighlights()
    {
        foreach (var highlight in activeHighlights)
        {
            GameObject.Destroy(highlight.HighlightRendererInstance.gameObject);
        }
        activeHighlights.Clear();
    }
}
public class CellHighlight
{
    public Cell Cell;
    public SpriteRenderer HighlightRendererInstance;
}
public interface ICanReceive<T> where T : Card
{
    bool CanReceiveCard(T card);
    void DoReceiveCard(T card);
}