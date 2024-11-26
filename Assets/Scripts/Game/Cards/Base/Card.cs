using Engine;
using System;
using UnityEngine;

public abstract class Card : ContaineableScriptableObject, ICollectibleIconProvider
{
    public CardID ID => (CardID)AssetGUID;
    public Sprite SpriteRegular;
    public bool HasNPC = false;

    public Sprite GetIcon()
    {
        return PixelSpriteResizer.Instance.GetResizedSprite(SpriteRegular, 6, 10);
    }
    public abstract bool TryPlay(CardValidationContext context);
    public abstract bool RefreshValidation(CardValidationContext context);
    public abstract void EndValidation(CardValidationContext context);
}
