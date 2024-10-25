using System.Collections.Generic;
using Engine;
using UnityEngine;

[SingletonSettings(_lifetime: SingletonLifetime.Scene, _canBeGenerated: true, _eager: false)]
internal class ProcedureHandler : SingletonBehaviour<ProcedureHandler>
{
    List<Procedure> activeProcedures = new ();

    public Event<Procedure> OnProcedureStartedEvent = new();
    public Event<Procedure> OnProcedureFinishedEvent = new();

    internal Procedure StartNewProcedure(Cell cell, float duration, Card input, TokenID rewardToken, System.Action callback = null)
    {
        var newProcedure = new Procedure(cell, duration, input, rewardToken.ToToken(), callback: callback);
        activeProcedures.Add(newProcedure);
        OnProcedureStartedEvent?.Invoke(newProcedure);
        return newProcedure;
    }

    private void Update()
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

        foreach (var procedure in finished)
        {
            activeProcedures.Remove(procedure);
            OnProcedureFinishedEvent?.Invoke(procedure);
        }
    }
}
