using System;
using UnityEngine;

namespace Engine
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class SingletonSettingsAttribute : Attribute
    {
        public readonly SingletonLifetime Lifetime;
        public readonly bool CanBeGenerated;
        public readonly bool Eager;

        public SingletonSettingsAttribute(SingletonLifetime _lifetime, bool _canBeGenerated = false, bool _eager = false)
        {
            Lifetime = _lifetime;
            CanBeGenerated = _canBeGenerated;
            Eager = _eager;
        }
    }
    public enum SingletonLifetime
    {
        Scene,
        Persistant
    }
    public interface ISingleton
    {
        void DestroySingleton();
    }
    public abstract class Singleton<T> : ISingleton where T : Singleton<T>, new()
    {
        protected static T instance;

        public static T Instance => GetInstance();

        public static bool InstanceExists => instance != null;

        private static T GetInstance()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Debug.LogError($"Trying to access singleton {typeof(T).Name} in Edit mode.");
                return null;
            }
#endif


            if (instance == null)
                TryCreateInstance();

            return instance;
        }

        private static void TryCreateInstance()
        {
            if (SingletonManager.inApplicationQuit)
                return;

            instance = new T();
            instance.OnCreate();
            SingletonManager.Instance.Register(instance);
        }
        public void DestroySingleton()
        {
            instance.OnDestroy();
            SingletonManager.Instance.Unregister(instance);
            instance = null;
        }
        protected virtual void OnCreate() { }
        protected virtual void OnDestroy() { }
    }
}
