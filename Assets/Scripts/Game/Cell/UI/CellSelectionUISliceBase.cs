﻿using System;
using UnityEngine;

public class CellSelectionUISliceBase : MonoBehaviour
{
    [SerializeField] private HeaderModule_CellSelectionUISlice header;
    [SerializeField] private BuildingsModule_CellSelectionUISlice buildings;

    public void Init(Cell selectedCell)
    {
        header.Init(selectedCell);
        buildings.Init(selectedCell);
    }
}