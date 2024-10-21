using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Engine;
using UnityEngine;

public class CardHandler : Singleton<CardHandler>
{
    public List<CardStack> Stacks = new();
    public Event<CardStack> OnNewStackCreatedEvent = new();
    public Event<CardStack> OnStackUpdatedEvent = new(); // Event for updates
    public Event<CardStack> OnOldStackDeletedEvent = new();
    public Event<CardStack, FloatingCardParameters> OnCardStackAnimateEvent = new();

    private const float CARD_ANIM_DURATION = .5f;

    private float lastCardAnimationTime = -1f;
    private const float CARD_ANIM_DELAY = .1f;

    private Queue<CardAnimationRequest> pendingRequests = new();

    public void AddCardAnimated(CardID cardID, Vector3 worldOrigin) => AddCardAnimated(cardID, 1, worldOrigin);
    public void AddCardAnimated(CardID cardID, int amount, Vector3 worldOrigin)
    {
        for (int i = 0; i < amount; i++)
        {
            pendingRequests.Enqueue(new CardAnimationRequest()
            {
                Card = cardID.ToCard(),
                Position = worldOrigin
            });
        }

        RefreshRequests();
    }

    private void RefreshRequests()
    {
        if (pendingRequests.Count == 0)
            return;

        if (lastCardAnimationTime + CARD_ANIM_DELAY > Time.time)
            return;

        var reuqest = pendingRequests.Dequeue();

        OnCardStackAnimateEvent?.Invoke(new CardStack()
        {
            Card = reuqest.Card,
            Amount = 1,
        }, new FloatingCardParameters()
        {
            TargetStack = Stacks.Find(s => s.Card.ID == reuqest.Card.ID),
            WorldOrigin = reuqest.Position,
            AnimationDuration = CARD_ANIM_DURATION
        });

        lastCardAnimationTime = Time.time;

        DOVirtual.DelayedCall(CARD_ANIM_DURATION, () => AddCard(reuqest.Card.ID, 1));
        DOVirtual.DelayedCall(CARD_ANIM_DELAY, () => RefreshRequests());
    }

    // Add card based on CardID, with optional amount (default is 1)
    public void AddCard(CardID cardID, int amount = 1)
    {
        // Try to find an existing stack with the same card ID
        CardStack existingStack = Stacks.Find(stack => stack.Card.ID == cardID);

        if (existingStack != null)
        {
            // Stack exists, so we just increase the amount
            existingStack.Amount += amount;
            OnStackUpdatedEvent.Invoke(existingStack); // Notify about stack update
        }
        else
        {
            // No existing stack, create a new one using the extension method
            Card newCard = cardID.ToCard(); // Using extension method to get the card instance
            CardStack newStack = new CardStack { Card = newCard, Amount = amount };
            Stacks.Add(newStack);
            OnNewStackCreatedEvent.Invoke(newStack); // Notify about new stack
        }
    }

    // Remove a card or reduce the amount in the stack
    public void RemoveCard(CardID cardID, int amount = 1)
    {
        CardStack existingStack = Stacks.Find(stack => stack.Card.ID == cardID);

        if (existingStack != null)
        {
            existingStack.Amount -= amount;

            if (existingStack.Amount <= 0)
            {
                // Stack is empty, remove it
                Stacks.Remove(existingStack);
                OnOldStackDeletedEvent.Invoke(existingStack); // Notify about deletion
            }
            else
            {
                // Stack still has cards, so just update it
                OnStackUpdatedEvent.Invoke(existingStack); // Notify about stack update
            }
        }
    }
}

[System.Serializable]
public class CardStack
{
    public Card Card;
    public int Amount;
}

public static class CardExtensions
{
    public static Card ToCard(this CardID cardID)
    {
        return ScriptableObjectContainerProvider.Instance.Cards.All.Find(card => card.ID == cardID);
    }
}

public class CardAnimationRequest
{
    public Card Card;
    public Vector2 Position;
}