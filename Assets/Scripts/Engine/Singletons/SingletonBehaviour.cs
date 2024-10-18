using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Engine
{

    public abstract class SingletonBehaviour<T> : MonoBehaviour, ISingleton where T : SingletonBehaviour<T>
    {
        protected static T instance;

        public static T Instance => GetInstance();

        public static bool InstanceExists => instance != null;

        private static T GetInstance()
        {
#if UNITY_EDITOR
            if(!Application.isPlaying)
            {
                Debug.LogError($"Trying to access singleton {typeof(T).Name} in Edit mode.");
                return null;
            }
#endif

            if (instance == null)
                TryCreateInstance();

            return instance;
        }

        public void DestroySingleton()
        {
            SingletonManager.Instance.Unregister(instance);
            GameObject.Destroy(instance.gameObject);
            instance = null;
        }

        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = (T)this;
                SingletonManager.Instance.Register(instance);
            }
            else
            {
                Debug.LogError($"Duplicate Singleton of Type {typeof(T).Name}. Removing.");
                GameObject.Destroy(this);
            }
        }

        protected virtual void OnDestroy()
        {
            if (instance == this && Application.isPlaying)
            {
                SingletonManager.Instance.Unregister(instance);
                instance = null;
            }
        }

        private static void TryCreateInstance()
        {
            if (SingletonManager.inApplicationQuit)
                return;

            var prefab = SingletonResourceTracker.Instance.TryGetPrefab(typeof(T));

            if (prefab == null)
            {
                if (CanBeGenerated(typeof(T)))
                {
                    var go = new GameObject(typeof(T).Name + " - Generated Singleton");
                    DontDestroyOnLoad(go);
                    go.AddComponent<T>();
                    return;
                }
            }
            else
            {
                var go = GameObject.Instantiate(prefab);
                DontDestroyOnLoad(go);
                go.name += " - Lazy Singleton";
                return;
            }

            Debug.LogError($"Failed to create Singleton for {typeof(T).Name}");
        }

        private static bool CanBeGenerated(Type type)
        {
            var atr = type.GetCustomAttributes(typeof(SingletonSettingsAttribute), true).FirstOrDefault() as SingletonSettingsAttribute;

            if (atr != null)
                return atr.CanBeGenerated;

            return false;
        }
    }
}
