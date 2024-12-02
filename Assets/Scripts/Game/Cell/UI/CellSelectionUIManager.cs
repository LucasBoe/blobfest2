using UnityEngine;
public class CellSelectionUIManager : MonoBehaviour
{
    [SerializeField] private CellSelectionUISliceBase dummy;
    private CellSelectionUISliceBase _activeSliceBaseInstance;

    private void Awake()
    {
        dummy.gameObject.SetActive(false);
    }

    private void OnEnable() => CellHoverAndSelectionHandler.Instance.OnSelectedChangedEvent.AddListener(OnCellSelected);
    private void OnDisable() => CellHoverAndSelectionHandler.Instance.OnSelectedChangedEvent.RemoveListener(OnCellSelected);

    private void OnCellSelected(Cell selectedCell)
    {
        if (_activeSliceBaseInstance is not null) 
            Destroy(_activeSliceBaseInstance.gameObject);

        if (selectedCell is null)
            return;

        _activeSliceBaseInstance = Instantiate(dummy, dummy.transform.parent);
        _activeSliceBaseInstance.gameObject.SetActive(true);
        _activeSliceBaseInstance.Init(selectedCell);
    }
}