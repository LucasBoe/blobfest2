using System.Collections.Generic;
using UnityEngine;

public static class InstantiationUtil
{
    /// <summary>
    /// Instantiates a copy of the given dummy slice, parents it under the same transform,
    /// activates it, calls Init(data) and returns the new instance.
    /// </summary>
    /// <typeparam name="T">The data type the slice works with.</typeparam>
    /// <typeparam name="U">The concrete slice type.</typeparam>
    /// <param name="dummy">A disabled dummy in the scene.</param>
    /// <param name="data">The data to pass into Init.</param>
    /// <returns>The newly instantiated slice.</returns>
    public static U InstantiateFromDummy<T, U>(U dummy, T data, Vector3? optPosition, Dictionary<T, U> optDictionary)
        where U : MonoBehaviour, IUISlice<T>
    {
        var instance = UnityEngine.Object.Instantiate(dummy, dummy.transform.parent);
        instance.gameObject.SetActive(true);
        instance.Init(data);
        
        if (optPosition != null)
            instance.transform.position = optPosition.Value;
        
        if (optDictionary != null)
            optDictionary.Add(data, instance);
        
        return instance;
    }
    public static U InstantiateFromDummy<T, U>(U dummy, T data, Dictionary<T, U> optDictionary = null)
        where U : MonoBehaviour, IUISlice<T>
    {
        return InstantiateFromDummy(dummy, data, optPosition: null, optDictionary: optDictionary);
    }    
    public static U InstantiateFromDummy<T, U>(U dummy, T data)
        where U : MonoBehaviour, IUISlice<T>
    {
        return InstantiateFromDummy(dummy, data, optPosition: null, optDictionary: null);
    }
    public static void DestroyAndRemove<T, U>(T identifier, Dictionary<T, U> dictionary)
        where U : MonoBehaviour
    {
        if (dictionary.TryGetValue(identifier, out var slice))
        {
            Object.Destroy(slice.gameObject);
            dictionary.Remove(identifier);
        }
    }
}