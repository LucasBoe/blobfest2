using System;
using System.Collections.Generic;
using Engine;
using UnityEngine;

[SingletonSettings(_lifetime: SingletonLifetime.Scene, _canBeGenerated: true, _eager: false)]
internal class ProcedureHandler : SingletonBehaviour<ProcedureHandler>
{
    List<ProcedureBase> activeProcedures = new ();

    public Event<ProcedureBase> OnProcedureStartedEvent = new();
    public Event<ProcedureBase> OnProcedureFinishedEvent = new();
    private List<ProcedureBase> freshlyCreatedProcedures = new();

    internal StaticTimeProcedure StartNewProcedure(float duration)
    {
        var proc = new StaticTimeProcedure(duration);
        freshlyCreatedProcedures.Add(proc);
        return proc;
    }
    internal DynamicTimeProcecure StartNewProcedure(DynamicTimeProcecure.IProgressProvider progressProvider)
    {
        var proc = new DynamicTimeProcecure(progressProvider);
        freshlyCreatedProcedures.Add(proc);
        return proc;
    }
    private void Update()
    {
        StartNewProcedures();

        List<ProcedureBase> finished = UpdateAllActiveAndReturnFinished();

        FinishProcedures(finished);
    }
    private void StartNewProcedures()
    {
        foreach (var procedure in freshlyCreatedProcedures)
        {
            activeProcedures.Add(procedure);
            OnProcedureStartedEvent?.Invoke(procedure);
        }

        freshlyCreatedProcedures.Clear();
    }
    private void FinishProcedures(List<ProcedureBase> finished)
    {
        foreach (var procedure in finished)
        {
            activeProcedures.Remove(procedure);
            OnProcedureFinishedEvent?.Invoke(procedure);
        }
    }
    private List<ProcedureBase> UpdateAllActiveAndReturnFinished()
    {
        List<ProcedureBase> finished = new();

        var time = Time.time;

        foreach (var procedure in activeProcedures)
        {
            bool wasRunningBefore = procedure.IsRunning;
            procedure.Update(time);

            if (wasRunningBefore == procedure.IsRunning)
                continue;

            finished.Add(procedure);
        }

        return finished;
    }

    internal void Stop(ProcedureBase procedure)
    {
        activeProcedures.Remove(procedure);
        OnProcedureFinishedEvent?.Invoke(procedure);
    }
}
