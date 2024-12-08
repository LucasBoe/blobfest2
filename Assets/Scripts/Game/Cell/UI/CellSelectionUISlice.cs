using System;
using UnityEngine;

public class CellSelectionUISlice : MonoBehaviour
{
    [SerializeField] private HeaderModule_CellSelectionUISlice header;
    [SerializeField] private BuildingsModule_CellSelectionUISlice buildings;
    
    private Cell selectedCell;

    public void Init(Cell selectedCell)
    {
        header.Init(selectedCell, this);
        buildings.Init(selectedCell, this);
        
        this.selectedCell = selectedCell;
    }
}