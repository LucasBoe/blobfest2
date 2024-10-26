using System;
using System.Collections;
using System.Collections.Generic;
using Simple.SoundSystem.Core;
using Engine;
using UnityEngine;

public class SingletonTest : MonoBehaviour, IDelayedStartObserver
{
    [SerializeField] Sound testSound;
    private void OnEnable()
    {
        TestSingleton.Instance.TestEvent.AddListener(TestFunction);
    }

    private void Start()
    {
    }

    private void OnDisable()
    {
        TestSingleton.Instance.TestEvent.RemoveListener(TestFunction);
    }

    private void TestFunction()
    {
        Debug.Log("FF");

        if (testSound != null)
            testSound.Play();
    }

    public void DelayedStart()
    {
        Debug.Log("Delayed Start");
        TestSingleton.Instance.TestEvent.Invoke();
    }
}

public class TestSingleton : Singleton<TestSingleton>
{
    public Engine.Event TestEvent = new();
}
