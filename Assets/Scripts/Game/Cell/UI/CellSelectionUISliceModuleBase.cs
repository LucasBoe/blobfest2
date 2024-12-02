using UnityEngine;

public abstract class CellSelectionUISliceModuleBase
{
    [SerializeField] private GameObject root;
    protected Cell Cell;
    public virtual void Init(Cell cell)
    {
        Cell = cell;

        if (CheckShouldBeActive())
            TryPopulate();
        else
            root.SetActive(false);
    }

    protected abstract void TryPopulate();
    protected virtual bool CheckShouldBeActive() => true;
}