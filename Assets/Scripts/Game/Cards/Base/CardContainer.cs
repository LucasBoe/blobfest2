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
        Builder = -702438,
        Wood = -34392,
        Settler = -3582,
        Farmer = -309634,
        Villager = -119086,
        Stonemason = -9118,
        Stone = -668576,
        Yield = -668720,
    }
#endregion GENERATED (ScriptableObjectContainer)
