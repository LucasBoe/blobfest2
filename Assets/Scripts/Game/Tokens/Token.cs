using Engine;
using UnityEngine;

public class Token : ContaineableScriptableObject, ICollectibleIconProvider
{
    public TokenID ID => (TokenID)AssetGUID;
    public Sprite Sprite;

    public Sprite GetIcon()
    {
        return PixelSpriteResizer.Instance.GetResizedSprite(Sprite, 12, 12);
    }
}

internal interface ICollectibleIconProvider
{
    public Sprite GetIcon();
}