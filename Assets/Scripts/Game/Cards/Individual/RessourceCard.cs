using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class RessourceCard : Card
{
    [FormerlySerializedAs("AssociatedType")] [SerializeField]
    public ResourceType AssociatedResourceType;
    public Sprite ResourceIcon;

    public override bool TryPlay(CardValidationContext context)
    {
        if (context.CurrentHoverCell == null)
            return false;

        if (context.CurrentHoverCell.CurrentBehavior == null)
            return false;

        if (context.CurrentHoverCell.CurrentBehavior is not ICanReceive<RessourceCard> ressource)
            return false;

        ressource.DoReceiveCard(this);
        return true;
    }
    public override bool RefreshValidation(CardValidationContext context)
    {
        if (context.CurrentHoverCell == null)
            return false;

        if (context.CurrentHoverCell.CurrentBehavior == null)
            return false;

        if (context.CurrentHoverCell.CurrentBehavior is ICanReceive<RessourceCard> ressource)
            return ressource.CanReceiveCard(this);

        return false;
    }

    public override void EndValidation(CardValidationContext context)
    {
    }
}

public enum ResourceType
{
    Wood,
    Stone,
    Yield,
    Villager
}

public class ResourceAmountPair
{
    public ResourceType ResourceType;
    public int Amount;
    public ResourceAmountPair(ResourceType type, int amount)
    {
        ResourceType = type;
        Amount = amount;
    }
}

public static class RessourceTypeIconUtil
{
    public static Sprite ToIcon(this ResourceType type)
    {
        switch (type)
        {
            case ResourceType.Wood:
                return FromCard(CardID.Wood);
            
            case ResourceType.Villager:
                return FromCard(CardID.Villager);
        }

        Sprite FromCard(CardID card)
        {
            var c = card.ToCard();
            if (c is RessourceCard rc && rc.ResourceIcon != null)
                return rc.ResourceIcon;
            
            return c.GetIcon();
        }

        Debug.LogError("Trying to get icon from RessourceTypeIconUtil which was not set up properly.");
        return null;
    }
}