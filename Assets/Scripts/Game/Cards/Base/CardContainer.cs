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
        MakeFarm = -309634,
        Settlement = -3582,
        Villager = -119086,
        Stonemason = -9118,
        Wood = -34392,
    }
#endregion GENERATED (ScriptableObjectContainer)
