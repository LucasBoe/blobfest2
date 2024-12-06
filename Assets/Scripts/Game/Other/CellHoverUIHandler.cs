using UnityEngine;
using System.Collections;
using System;
using Engine;
using Simple.SoundSystem.Core;

public class CellHoverUIHandler : MonoBehaviour
{
    [SerializeField] private CellLineRenderer cellLineRenderer;
    [SerializeField] private Sound hoverSound;
    private void OnEnable() => CellHoverAndSelectionHandler.Instance.OnHoverChangedEvent.AddListener(OnHoverChanged);
    private void OnDisable() => CellHoverAndSelectionHandler.Instance.OnHoverChangedEvent.RemoveListener(OnHoverChanged);

    private void OnHoverChanged(Cell cell)
    {
        if (cell != null)
        {
            cellLineRenderer.DrawCellOutline(cell.Edges);
            hoverSound.Play();
        }
        else
            cellLineRenderer.DrawCellOutline(null);
    }
}

[SingletonSettings(SingletonLifetime.Scene, _canBeGenerated: true, _eager: true)]
public class CellHoverTooltipHandler : Singleton<CellHoverTooltipHandler>
{
    private Cell currentHoveredCell;

    protected override void OnCreate() => CellHoverAndSelectionHandler.Instance.OnHoverChangedEvent.AddListener(OnHoverChanged);

    protected override void OnDestroy() => CellHoverAndSelectionHandler.Instance.OnHoverChangedEvent.RemoveListener(OnHoverChanged);

    private void OnHoverChanged(Cell cell)
    {
        // If the hovered cell hasn't changed, do nothing
        if (currentHoveredCell == cell)
            return;

        // Hide the tooltip for the previous hovered cell if any
        if (currentHoveredCell != null)
            TooltipHandler.Instance.Hide(this);
       
        currentHoveredCell = cell;

        // If no cell is hovered, do nothing further
        if (cell == null)
            return;

        // ShowUI the tooltip for the new hovered cell
        var text = cell.CellType.ToString();
        var position = cell.Center;
        TooltipHandler.Instance.ShowWorld(position, text, this);
    }
}
