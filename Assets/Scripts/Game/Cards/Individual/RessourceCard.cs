using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class RessourceCard : Card
{
    [FormerlySerializedAs("AssociatedType")] [SerializeField]
    public RessourceType AssociatedRessourceType;

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

public enum RessourceType
{
    Wood,
    Stone,
    Yield
}

public static class RessourceTypeIconUtil
{
    public static Sprite ToIcon(this RessourceType type)
    {
        switch (type)
        {
            case RessourceType.Wood:
                return FromCard(CardID.Wood);
        }

        Sprite FromCard(CardID card)
        {
            return card.ToCard().GetIcon();
        }

        Debug.LogError("Trying to get icon from RessourceTypeIconUtil which was not set up properly.");
        return null;
    }
}