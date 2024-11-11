
using System.Collections.Generic;
using UnityEngine;

internal class ProcedureUIManager : MonoBehaviour
{
    [SerializeField] private ProcedureUISlice procedureUISliceDummy;
    private const float BAR_OFFSET = .5f;

    private Dictionary<ProcedureBase, ProcedureUISlice> activeSlices = new();
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

    private void OnProcedureStarted(ProcedureBase procedure)
    {
        if (procedureUISliceDummy == null)
        {
            Debug.LogWarning("ProcedureUISlice prefab not assigned!");
            return;
        }

        ProcedureUISlice newSlice = Instantiate(procedureUISliceDummy, transform);
        newSlice.transform.position = procedure.AssociatedCell.Center + new Vector2(0, BAR_OFFSET);
        newSlice.gameObject.SetActive(true);
        newSlice.Init(procedure);

        activeSlices[procedure] = newSlice;
    }

    private void OnProcedureFinished(ProcedureBase procedure)
    {
        if (activeSlices.TryGetValue(procedure, out var slice))
        {
            Destroy(slice.gameObject);
            activeSlices.Remove(procedure);
        }
    }
}
