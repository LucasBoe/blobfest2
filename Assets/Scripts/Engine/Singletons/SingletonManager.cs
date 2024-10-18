using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Engine
{
    [SingletonSettings(SingletonLifetime.Persistant)]
    public class SingletonManager : Singleton<SingletonManager>
    {
        public static Type[] singletonTypes;
        public static bool inApplicationQuit;

        private List<ISingleton> sceneSingletons = new();
        private List<ISingleton> allSingletons = new();
        private bool inDisposingOperation;


        public SingletonManager()
        {
            Application.quitting += OnApplicationQuit;
        }

        private void OnApplicationQuit()
        {
            Application.quitting -= OnApplicationQuit;
            inApplicationQuit = true;
        }

        public IEnumerable<T> FindRawSingletonsThatImplement<T>()
        {
            for (int i = 0; i < allSingletons.Count; i++)
            {
                ISingleton item = allSingletons[i];
                if (item is T tItem && item is not MonoBehaviour)
                    yield return tItem;
            }
        }

        public void Register(ISingleton singleton)
        {
            if (inDisposingOperation)
            {
                Debug.LogError($"New Singleton of type {singleton.GetType()} was registered while singletion disposing process is running.");
                return;
            }

            if (HasSceneLifetime(singleton.GetType()))
                sceneSingletons.Add(singleton);

            allSingletons.Add(singleton);
        }

        public void Unregister(ISingleton instance)
        {
            if (HasSceneLifetime(instance.GetType()))
                sceneSingletons.Remove(instance);

            allSingletons.Remove(instance);
        }

        public int DisposeOfSceneSingletons()
        {
            int count = 0;
            inDisposingOperation = true;
            foreach (var item in sceneSingletons.ToArray())
            {
                item.DestroySingleton();
                count++;
            }

            sceneSingletons.Clear();
            inDisposingOperation = false;
            return count;
        }

        public int SpawnEagerSingletons(bool _includeGameplay)
        {
            int count = SingletonResourceTracker.Instance.SpawnEagerSingletons(_includeGameplay);

            foreach (var type in singletonTypes)
            {
                var settings = GetSettingsForType(type);
                if (settings != null && settings.Eager && !IsSingletonSet(type))
                {
                    //Call the "Instance" property, triggering creation
                    GetInstanceProperty(type).GetValue(null);
                    count++;
                }
            }

            return count;
        }


        private static bool HasSceneLifetime(Type type)
        {
            var atr = GetSettingsForType(type);
            if (atr != null)
                return atr.Lifetime == SingletonLifetime.Scene;

            return true;
        }

        private static SingletonSettingsAttribute GetSettingsForType(Type type)
        {
            return type.GetCustomAttributes(typeof(SingletonSettingsAttribute), true).FirstOrDefault() as SingletonSettingsAttribute;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void StaticsCleanup()
        {
            inApplicationQuit = false;
            Type[] types = FindSingletonTypes();
            singletonTypes = types;

            foreach (var item in types)
            {
                if (item.ContainsGenericParameters)
                    continue;

                var field = GetInstanceField(item);
                field?.SetValue(null, null);
            }
        }

        private static Type[] FindSingletonTypes()
        {
            return typeof(ISingleton).Assembly.GetTypes().Where(x => !x.IsAbstract && typeof(ISingleton).IsAssignableFrom(x)).ToArray();
        }

        public static bool IsSingletonSet(Type _t)
        {
            return GetInstanceField(_t).GetValue(null) != null;
        }

        public static FieldInfo GetInstanceField(Type _t)
        {
            return _t.GetField("instance",
                   System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
        }

        public static PropertyInfo GetInstanceProperty(Type _t)
        {
            return _t.GetProperty("Instance",
                   System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | BindingFlags.FlattenHierarchy);
        }
    }
}
