using System;
using UnityEngine;
using NaughtyAttributes;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Engine
{
    public abstract class ScriptableObjectContainerBase : ScriptableObject
    {

        [HideInInspector] public bool AllowInstancesForBaseType = true;
        [HideInInspector] public bool OnlyAllowOneInstancePerChildType = false;
        public virtual Type ChildClassesBaseType => null;
        public virtual int Count => 0;
        public abstract void Add(ContaineableScriptableObject toAdd);
#if UNITY_EDITOR
        public T CreateNewInstanceOfType<T>() where T : ContaineableScriptableObject => CreateNewInstanceOfType(typeof(T)) as T;
        public ContaineableScriptableObject CreateNewInstanceOfType(Type type)
        {
            var instance = ScriptableObject.CreateInstance(type) as ContaineableScriptableObject;
            instance.name = $"{type.Name}_{instance.AssetGUID}";
            AssetDatabase.AddObjectToAsset(instance, this);
            instance.Container = this;
            Add(instance);
            AssetDatabase.SaveAssets();
            return instance;            
        }
#endif
        public abstract bool ContainsElementOfType(Type type);       
        public abstract void Destroy(ContaineableScriptableObject toDestroy);
    }
}
