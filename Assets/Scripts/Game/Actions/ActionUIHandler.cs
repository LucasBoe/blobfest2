using System.Collections.Generic;
using Engine;
using UnityEngine;

public class ActionUIHandler : MonoBehaviour, IDelayedStartObserver
{
    [SerializeField] private Canvas inWorldCanvas;
    [SerializeField] private DropActionUISlice dropActionDummy;
    [SerializeField] private ConstructionActionUISlice constructionSelectionActionDummy; 
    // the construction selection action now runs in parallel to the drop action

    // track active UI slices
    private Dictionary<DropAction, DropActionUISlice> dropActionUIs = new();
    private Dictionary<ConstructionSelectionAction, ConstructionActionUISlice> constructionActionUIs = new();

    private void Awake()
    {
        // hide both dummies until needed
        dropActionDummy.gameObject.SetActive(false);
        constructionSelectionActionDummy.gameObject.SetActive(false);
    }

    public void DelayedStart()
    {
        // hook world‐space canvas to main camera
        inWorldCanvas.worldCamera = Camera.main;
    }
    private void OnEnable()
    {
        var handler = ActionHandler.Instance;
        // drop action events
        handler.OnStartNewDropActionEvent.AddListener(OnStartNewDropAction);
        handler.OnEndDropActionEvent.AddListener(OnEndDropAction);
        // construction selection events
        handler.OnStartNewConstructionSelectionActionEvent.AddListener(OnStartNewConstructionSelectionAction);
        handler.OnEndConstructionSelectionActionEvent.AddListener(OnEndConstructionSelectionAction);
    }
    private void OnDisable()
    {
        var handler = ActionHandler.Instance;
        // drop action events
        handler.OnStartNewDropActionEvent.RemoveListener(OnStartNewDropAction);
        handler.OnEndDropActionEvent.RemoveListener(OnEndDropAction);
        // construction selection events
        handler.OnStartNewConstructionSelectionActionEvent.RemoveListener(OnStartNewConstructionSelectionAction);
        handler.OnEndConstructionSelectionActionEvent.RemoveListener(OnEndConstructionSelectionAction);
    }
    private void OnStartNewDropAction(DropAction dropAction)
    {
        InstantiationUtil.InstantiateFromDummy(dropActionDummy, dropAction, dropAction.Cell.Center, dropActionUIs);
    }
    private void OnEndDropAction(DropAction action)
    {
        InstantiationUtil.DestroyAndRemove(action, dropActionUIs);
    }
    private void OnStartNewConstructionSelectionAction(ConstructionSelectionAction action)
    {
        InstantiationUtil.InstantiateFromDummy(constructionSelectionActionDummy, action, action.Cell.Center, constructionActionUIs);
    }
    private void OnEndConstructionSelectionAction(ConstructionSelectionAction action)
    {
        InstantiationUtil.DestroyAndRemove(action, constructionActionUIs);
    }
}
