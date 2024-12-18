using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoronoiMap;

//DEPRECATED
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
        //PlayerEventHandler.Instance.OnPlayerChangedCellEvent.AddListener(ShowCell);
    }
    private void OnDisable()
    {
        //PlayerEventHandler.Instance.OnPlayerChangedCellEvent.RemoveListener(ShowCell);
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
    private void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        // Convert the screen position to world coordinates using the main camera
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        ShowCell(MapDataUtil.GetCellThatContainsPoint(MapHandler.Instance.MapData, worldPosition));
    }
}
