using System;
using System.Collections.Generic;
using UnityEngine;

namespace Engine
{
    [SingletonSettings(SingletonLifetime.Persistant)]
    public class SingletonResourceTracker : Singleton<SingletonResourceTracker>
    {
        private Dictionary<Type, GameObject> lazySingletons = new();
        private Dictionary<Type, GameObject> eagerGlobalSingletons = new();
        private Dictionary<Type, GameObject> eagerGameplaySingletons = new();

        private List<Type> eagerGlobalSingletonsNoPrefab = new();
        private List<Type> eagerGlobalSingletonsNoMono = new();

        private List<Type> eagerGameplaySingletonsNoPrefab = new();
        private List<Type> eagerGameplaySingletonsNoMono = new();

        public SingletonResourceTracker()
        {
            LoadAll(SingletonType.EagerGlobal, eagerGlobalSingletons);
            LoadAll(SingletonType.EagerGameplay, eagerGameplaySingletons);
            LoadAll(SingletonType.Lazy, lazySingletons);
        }

        public int SpawnEagerSingletons(bool _includeGameplay)
        {
            int count = 0;

            foreach (var item in eagerGlobalSingletons)
            {
                if (TrySpawnSingleton(item.Key, item.Value))
                    count++;
            }

            if (_includeGameplay)
            {
                foreach (var item in eagerGameplaySingletons)
                {
                    if (TrySpawnSingleton(item.Key, item.Value))
                        count++;
                }
            }

            return count;
        }

        private static bool TrySpawnSingleton(Type _type, GameObject _prefab)
        {
            if (!SingletonManager.IsSingletonSet(_type))
            {
                var go = GameObject.Instantiate(_prefab);
                GameObject.DontDestroyOnLoad(go);
                go.name += " - Eager Singleton";
                return true;
            }
            return false;
        }

        private static void LoadAll(SingletonType _type, Dictionary<Type, GameObject> _collection)
        {
            UnityEngine.Object[] prefas = LoadAll(_type);

            foreach (GameObject go in prefas)
            {
                foreach (var script in go.GetComponents<ISingleton>())
                {
                    if (script != null)
                        _collection.Add(script.GetType(), go);
                }
            }
        }

        public GameObject TryGetPrefab(Type type)
        {
            if (eagerGlobalSingletons.TryGetValue(type, out GameObject go))
                return go;

            if (eagerGameplaySingletons.TryGetValue(type, out go))
                return go;

            if (lazySingletons.TryGetValue(type, out go))
                return go;

            return null;
        }

        public static UnityEngine.Object[] LoadAll(SingletonType type)
        {
            var path = TypeToPath(type);
            UnityEngine.Object[] loadedObjects = Resources.LoadAll(path);
            return loadedObjects;
        }

        public static string TypeToPath(SingletonType ownType)
        {
            return "Singletons/" + ownType.ToString() + "Singletons";
        }

        public enum SingletonType
        {
            Lazy,
            EagerGlobal,
            EagerGameplay,
        }


    }
}
