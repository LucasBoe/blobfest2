using Engine;
using System;
using UnityEngine;

public abstract class Card : ContaineableScriptableObject, ICollectibleIconProvider
{
    public CardID ID => (CardID)AssetGUID;
    public Sprite SpriteRegular, SpriteIcon;

    public Sprite GetIcon()
    {
        return SpriteIcon;
    }

    public abstract void StartValidation(CardValidationContext context);
    public abstract void EndValidation(CardValidationContext context);
    public abstract bool RefreshValidation(CardValidationContext context);
}
