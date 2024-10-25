using System;
using System.Collections.Generic;
using Engine;
using UnityEngine;

[SingletonSettings(_lifetime: SingletonLifetime.Scene, _canBeGenerated: true, _eager: false)]
internal class ProcedureHandler : SingletonBehaviour<ProcedureHandler>
{
    List<Procedure> activeProcedures = new ();

    public Event<Procedure> OnProcedureStartedEvent = new();
    public Event<Procedure> OnProcedureFinishedEvent = new();
    private List<Procedure> freshlyCreatedProcedures = new();

    internal Procedure StartNewProcedure(float duration)
    {
        var proc = new Procedure(duration);
        freshlyCreatedProcedures.Add(proc);
        return proc;
    }
    private void Update()
    {
        StartNewProcedures();

        List<Procedure> finished = UpdateAllActiveAndReturnFinished();

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
    private void FinishProcedures(List<Procedure> finished)
    {
        foreach (var procedure in finished)
        {
            activeProcedures.Remove(procedure);
            OnProcedureFinishedEvent?.Invoke(procedure);
        }
    }
    private List<Procedure> UpdateAllActiveAndReturnFinished()
    {
        List<Procedure> finished = new();

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

    internal void Stop(Procedure procedure)
    {
        activeProcedures.Remove(procedure);
        OnProcedureFinishedEvent?.Invoke(procedure);
    }
}
