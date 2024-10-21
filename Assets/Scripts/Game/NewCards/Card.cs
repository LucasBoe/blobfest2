using Engine;
using UnityEngine;

public abstract class Card : ContaineableScriptableObject, ICollectibleIconProvider
{
    public CardID ID => (CardID)AssetGUID;
    public Sprite SpriteRegular, SpriteIcon;

    public Sprite GetIcon()
    {
        return SpriteIcon;
    }
}
