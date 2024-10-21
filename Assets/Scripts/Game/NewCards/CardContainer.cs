using System;
using Engine;
using UnityEngine;

[CreateAssetMenu]
public class CardContainer : ScriptableObjectContainer<Card>
{
    protected override string GetGeneratedEnumName() => "CardID";
    protected override bool GenerateEnum => true;
    protected override Type GetGeneratedEnumContainerType() => this.GetType();
}
#region GENERATED (ScriptableObjectContainer)

    public enum CardID : long
    {
        Villager = -119086,
    }
#endregion GENERATED (ScriptableObjectContainer)
