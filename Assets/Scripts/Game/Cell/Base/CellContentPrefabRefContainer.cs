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
        Stone = -650292,
        Grain = -30326,
        Houses = -1056142,
        Mill = -151462,
        Tree = -22954,
        None = -61480,
        StonemasonHut = -64650,
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
