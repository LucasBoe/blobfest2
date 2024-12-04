using System;
using UnityEngine;
public class CellSelectionUIManager : MonoBehaviour
{
    [SerializeField] private CellSelectionUISlice dummy;
    private CellSelectionUISlice _activeSliceInstance;
    private void Awake()
    {
        dummy.gameObject.SetActive(false);
    }
    private void OnEnable() => CellHoverAndSelectionHandler.Instance.OnSelectedChangedEvent.AddListener(OnCellSelected);
    private void OnDisable() => CellHoverAndSelectionHandler.Instance.OnSelectedChangedEvent.RemoveListener(OnCellSelected);

    private void OnCellSelected(Cell selectedCell)
    {
        if (_activeSliceInstance is not null) 
            Destroy(_activeSliceInstance.gameObject);

        if (selectedCell is null)
            return;

        _activeSliceInstance = Instantiate(dummy, dummy.transform.parent);
        _activeSliceInstance.gameObject.SetActive(true);
        _activeSliceInstance.Init(selectedCell);
    }
    private void Update()
    {
        if (!_activeSliceInstance)
            return;
        
        if (!Input.GetMouseButtonUp(1))
            return;
        
        Destroy(_activeSliceInstance.gameObject);
        _activeSliceInstance = null;
    }
}