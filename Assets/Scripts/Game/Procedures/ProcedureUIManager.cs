
using System.Collections.Generic;
using UnityEngine;

internal class ProcedureUIManager : MonoBehaviour
{
    [SerializeField] private ProcedureUISlice procedureUISliceDummy;

    private Dictionary<Procedure, ProcedureUISlice> activeSlices = new();
    private void Awake()
    {
        procedureUISliceDummy.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        ProcedureHandler.Instance.OnProcedureStartedEvent.AddListener(OnProcedureStarted);
        ProcedureHandler.Instance.OnProcedureFinishedEvent.AddListener(OnProcedureFinished);
    }

    private void OnDisable()
    {
        ProcedureHandler.Instance.OnProcedureStartedEvent.RemoveListener(OnProcedureStarted);
        ProcedureHandler.Instance.OnProcedureFinishedEvent.RemoveListener(OnProcedureFinished);
    }

    private void OnProcedureStarted(Procedure procedure)
    {
        if (procedureUISliceDummy == null)
        {
            Debug.LogWarning("ProcedureUISlice prefab not assigned!");
            return;
        }

        // Instantiate a new UISlice for the procedure and place it at the procedure's cell position
        ProcedureUISlice newSlice = Instantiate(procedureUISliceDummy, transform);
        newSlice.transform.position = procedure.AssociatedCell.Center; // Assuming AssociatedCell has WorldPosition
        newSlice.gameObject.SetActive(true);
        newSlice.Init(procedure);

        activeSlices[procedure] = newSlice;
    }

    private void OnProcedureFinished(Procedure procedure)
    {
        if (activeSlices.TryGetValue(procedure, out var slice))
        {
            Destroy(slice.gameObject);
            activeSlices.Remove(procedure);
        }
    }
}
