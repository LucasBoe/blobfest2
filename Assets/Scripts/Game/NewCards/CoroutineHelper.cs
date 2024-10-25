using Engine;
using System;
using System.Collections;
using UnityEngine;

[SingletonSettings(SingletonLifetime.Scene, _canBeGenerated: true, _eager: false)]
public class CoroutineHelper : SingletonBehaviour<CoroutineHelper>
{
    internal void OnNextFrame(Action callback)
    {
        StartCoroutine(DelayedCallbackRoutine(callback, new WaitForEndOfFrame()));
    }

    private IEnumerator DelayedCallbackRoutine(Action callback, YieldInstruction yieldInstruction)
    {
        yield return yieldInstruction;
        callback?.Invoke();
    }
}