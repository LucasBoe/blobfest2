using System;
using System.Collections.Generic;
using UnityEngine;

namespace Engine
{
    [System.Serializable]
    public abstract class EventBase
    {
        [SerializeField] protected List<UnityEngine.Object> TargetObjs = new();
#if UNITY_EDITOR
        public float lastInvoked = -1f;
#endif

        protected void TryAdd(object target, UnityEngine.Object listener)
        {
            if (listener != null)
                TargetObjs.Add(listener);
            else if (target != null)
                TargetObjs.Add(target as UnityEngine.Object);
        }
        protected void TryRemove(object target, UnityEngine.Object listener)
        {
            if (listener != null)
                TargetObjs.Remove(listener);
            else if (target != null)
                TargetObjs.Remove(target as UnityEngine.Object);
        }
    }

    [System.Serializable]
    public class Event : EventBase
    {
        private event System.Action action;
        internal void Invoke()
        {
            action?.Invoke();

#if UNITY_EDITOR
            lastInvoked = Time.time;
#endif
        }
        public void Trigger()
        {
            action?.Invoke();

#if UNITY_EDITOR
            lastInvoked = Time.time;
#endif
        }
        public void AddListener(Action overload, UnityEngine.Object obj = null)
        {
            action += overload;
            TryAdd(overload.Target, obj);
        }
        public void RemoveListener(Action overload, UnityEngine.Object obj = null)
        {
            action -= overload;
            TryRemove(overload.Target, obj);
        }
        public void RemoveAllListeners()
        {
            action = null;
            TargetObjs.Clear();
        }
        internal void Clear()
        {
            action = null;
        }

        public static Event operator +(Event myEvent, Action listener)
        {
            myEvent.AddListener(listener);
            return myEvent;
        }

        public static Event operator -(Event myEvent, Action listener)
        {
            myEvent.RemoveListener(listener);
            return myEvent;
        }
    }


    [System.Serializable]
    public class Event<T> : EventBase
    {
        private event System.Action<T> action;
        internal void Invoke(T item)
        {
            action?.Invoke(item);
#if UNITY_EDITOR
                lastInvoked = Time.time;
#endif
        }
        public void Trigger(T item)
        {
            action?.Invoke(item);
#if UNITY_EDITOR
                lastInvoked = Time.time;
#endif
        }
        public void AddListener(Action<T> overload, UnityEngine.Object obj = null)
        {
            action += overload;
            TryAdd(overload.Target, obj);
        }
        public void RemoveListener(Action<T> overload, UnityEngine.Object obj = null)
        {
            action -= overload;
            TryRemove(overload.Target, obj);
        }
        public void RemoveAllListeners()
        {
            action = null;
            TargetObjs.Clear();
        }
        public static Event<T> operator +(Event<T> myEvent, Action<T> listener)
        {
            myEvent.AddListener(listener);
            return myEvent;
        }

        public static Event<T> operator -(Event<T> myEvent, Action<T> listener)
        {
            myEvent.RemoveListener(listener);
            return myEvent;
        }

        public void Clear()
        {
            action = null;
        }
    }

    [System.Serializable]
    public class Event<T1, T2> : EventBase
    {
        private event System.Action<T1, T2> action;
        internal void Invoke(T1 item1, T2 item2)
        {
            action?.Invoke(item1, item2);
#if UNITY_EDITOR
            lastInvoked = Time.time;
#endif
        }
        public void Trigger(T1 item1, T2 item2)
        {
            action?.Invoke(item1, item2);
#if UNITY_EDITOR
            lastInvoked = Time.time;
#endif
        }

        public void AddListener(Action<T1, T2> overload, UnityEngine.Object obj = null)
        {
            action += overload;
            TryAdd(overload.Target, obj);
        }
        public void RemoveListener(Action<T1, T2> overload, UnityEngine.Object obj = null)
        {
            action -= overload;
            TryRemove(overload.Target, obj);
        }
        public void RemoveAllListeners()
        {
            action = null;
            TargetObjs.Clear();
        }
        public static Event<T1, T2> operator +(Event<T1, T2> myEvent, Action<T1, T2> listener)
        {
            myEvent.AddListener(listener);
            return myEvent;
        }
        public static Event<T1, T2> operator -(Event<T1, T2> myEvent, Action<T1, T2> listener)
        {
            myEvent.RemoveListener(listener);
            return myEvent;
        }
    }
}