using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoronoiMap;

public class CellBorderLineRenderer : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer;
    private Cell cell;
    private void Awake()
    {
        ShowCell(null);
    }
    private void OnEnable()
    {
        PlayerEventHandler.Instance.OnPlayerChangedCellEvent.AddListener(ShowCell);
    }
    private void OnDisable()
    {
        PlayerEventHandler.Instance.OnPlayerChangedCellEvent.RemoveListener(ShowCell);
    }
    public void ShowCell(Cell cell)
    {
        this.cell = cell;

        if (cell == null)
        {
            lineRenderer.positionCount = 0;
            return;
        }

        lineRenderer.positionCount = cell.Edges.Length;
        for (int i = 0; i < cell.Edges.Length; i++)
        {
            lineRenderer.SetPosition(i, cell.Edges[i]);
        }
    }
}
