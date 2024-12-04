using Engine;
using System;
using NaughtyAttributes;
using UnityEngine;

public abstract class Card : ContaineableScriptableObject, ICollectibleIconProvider
{
    public CardID ID => (CardID)AssetGUID;
    public virtual PrefabRefID BuildingPrefabRefID => PrefabRefID.None;
    public Color Color => Color.yellow;
    public Sprite SpriteRegular;
    public bool HasNPC = false;
    public Sprite GetIcon()
    {
        return PixelSpriteResizer.Instance.GetResizedSprite(SpriteRegular, 6, 10);
    }
    public abstract bool TryPlay(CardValidationContext context);
    public abstract bool RefreshValidation(CardValidationContext context);
    public abstract void EndValidation(CardValidationContext context);
    [Button]
    private void _Editor_AddToPlayerHand()
    {
        if (!Application.isPlaying)
            return;
        
        var player = FindObjectOfType<Player>();
        if (player == null)
            return;
        
        CardHandler.Instance.AddCardAnimated(ID, player.Position);
    }
}
