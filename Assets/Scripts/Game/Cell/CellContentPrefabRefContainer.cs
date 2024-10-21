using System;
using System.Linq;
using Engine;
using UnityEngine;

[CreateAssetMenu]
public class CellContentPrefabRefContainer : ScriptableObjectContainer<CellContentPrefabRef>
{
    protected override string GetGeneratedEnumName() => "PrefabRefID";
    protected override bool GenerateEnum => true;
    protected override Type GetGeneratedEnumContainerType() => this.GetType();
}
#region GENERATED (ScriptableObjectContainer)

    public enum PrefabRefID : long
    {
        Tree = -22954,
    }
#endregion GENERATED (ScriptableObjectContainer)

public static class PrefabRefIDExtension
{
    public static T TryGetPrefab<T>(this PrefabRefID refID)
    {
        var all = ScriptableObjectContainerProvider.Instance.CellContents.All;
        var match = all.Where(e => e.AssetGUID == (long)refID);

        if (match.Any())
        {
            if (match.First().Prefab.TryGetComponent<T>(out var component))
            {
                return component;
            }
            else
            {
                Debug.LogError($"Failed fetching prefab of type {typeof(T).ToString()} from ID {refID}");
            }
        }
        else
        {
            Debug.LogError($"Failed fetching prefab of from ID {refID}");
        }
        return default;
    }
}
