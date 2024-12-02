using System;
using System.Collections;
using System.Collections.Generic;
using Engine;
using NaughtyAttributes;
using UnityEngine;

[SingletonSettings(SingletonLifetime.Scene, _canBeGenerated: true, _eager: true)]
public class CellHoverAndSelectionHandler : SingletonBehaviour<CellHoverAndSelectionHandler>
{

    [ShowNativeProperty] public Cell CurrentHover { get; private set; }
    [ShowNativeProperty] public Cell Selected { get; private set; }

    public Event<Cell> OnHoverChangedEvent = new();
    public Event<Cell> OnSelectedChangedEvent = new();

    // Update is called once per frame
    void Update()
    {
        UpdateHover();
        UpdateSelection();

        void UpdateHover()
        {
            if (Camera.main == null)
                return;

            Vector3 mousePosition = Input.mousePosition;
            Vector2 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            var hover = MapDataUtil.GetCellThatContainsPoint(MapHandler.Instance.MapData, worldPosition);

            if (CurrentHover == hover)
                return;

            CurrentHover = hover;
            OnHoverChangedEvent?.Invoke(hover);
        }

        void UpdateSelection()
        {

            if (!Input.GetMouseButtonUp(0))
                return;

            if (CurrentHover == Selected)
                return;

            Selected = CurrentHover;
            OnSelectedChangedEvent?.Invoke(Selected);
        }
    }
}
