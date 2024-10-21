using System;
using Engine;
using UnityEngine;

[CreateAssetMenu]
public class TokenContainer : ScriptableObjectContainer<Token>
{
    protected override string GetGeneratedEnumName() => "TokenID";
    protected override bool GenerateEnum => true;
    protected override Type GetGeneratedEnumContainerType() => this.GetType();
}
#region GENERATED (ScriptableObjectContainer)
    public enum TokenID : long
    {
        Wood = -416798,
    }
#endregion GENERATED (ScriptableObjectContainer)
