using System.Collections.Generic;
using Engine;
using UnityEngine;

public class DropActionUIHandler : MonoBehaviour, IDelayedStartObserver
{
    [SerializeField] private Canvas inWolrdCanvas;
    [SerializeField] private DropActionUISlice dropActionDummy;

    private Dictionary<DropAction, DropActionUISlice> dropActionUIs = new();

    private void Awake()
    {
        dropActionDummy.gameObject.SetActive(false);
    }   
    public void DelayedStart()
    {
        inWolrdCanvas.worldCamera = Camera.main;
    }
    private void OnEnable()
    {
        ActionHandler.Instance.OnStartNewDropActionEvent.AddListener(OnStartNewDropAction);
        ActionHandler.Instance.OnEndDropActionEvent.AddListener(OnEndDropAction);
    }
    private void OnDisable()
    {
        ActionHandler.Instance.OnStartNewDropActionEvent.RemoveListener(OnStartNewDropAction);
        ActionHandler.Instance.OnEndDropActionEvent.RemoveListener(OnEndDropAction);
    }
    private void OnStartNewDropAction(DropAction dropAction)
    {
        Debug.Log("OnStartNewDropAction");
        
        var instance = Instantiate(dropActionDummy, dropActionDummy.transform.parent);
        instance.gameObject.SetActive(true);
        instance.transform.position = dropAction.Cell.Center;
        instance.Init(dropAction);
        dropActionUIs.Add(dropAction, instance);
    }   
    private void OnEndDropAction(DropAction dropAction)
    {
        Destroy(dropActionUIs[dropAction].gameObject);
        dropActionUIs.Remove(dropAction);
    }


}