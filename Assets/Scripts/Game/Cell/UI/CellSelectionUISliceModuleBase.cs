using UnityEngine;

public abstract class CellSelectionUISliceModuleBase
{
    [SerializeField] private GameObject root;
    protected Cell Cell;
    protected CellSelectionUISlice Main;
    public virtual void Init(Cell cell, CellSelectionUISlice main)
    {
        Cell = cell;
        Main = main;

        TryDisplay();
    }
    protected void TryDisplay()
    {
        if (CheckShouldBeActive())
        {
            root.SetActive(true);
            TryPopulate();
        }
        else
            root.SetActive(false);
    }
    protected abstract void TryPopulate();
    protected virtual bool CheckShouldBeActive() => true;
}