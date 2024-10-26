using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Engine;
using UnityEngine;

public class TokenHandler : Singleton<TokenHandler>
{
    public List<TokenStack> Stacks = new();
    public Event<TokenStack> OnNewStackCreatedEvent = new();
    public Event<TokenStack> OnStackUpdatedEvent = new(); // Event for updates
    public Event<TokenStack> OnOldStackDeletedEvent = new();
    public Event<TokenStack, FloatingTokenParameters> OnTokenStackAnimateEvent = new();

    private const float TOKEN_ANIM_DURATION = 0.5f;
    private const float TOKEN_ANIM_DELAY = 0.1f;

    private float lastTokenAnimationTime = -1f;
    private Queue<TokenAnimationRequest> pendingRequests = new();

    public void AddTokenAnimated(TokenID tokenID, Vector3 worldOrigin) => AddTokenAnimated(tokenID, 1, worldOrigin);
    public void AddTokenAnimated(TokenID tokenID, int amount, Vector3 worldOrigin)
    {
        for (int i = 0; i < amount; i++)
        {
            pendingRequests.Enqueue(new TokenAnimationRequest()
            {
                Token = tokenID.ToToken(), // Assuming similar extension method exists for tokens
                Position = worldOrigin
            });
        }

        RefreshRequests();
    }

    private void RefreshRequests()
    {
        if (pendingRequests.Count == 0)
            return;

        if (lastTokenAnimationTime + TOKEN_ANIM_DELAY > Time.time)
            return;

        var request = pendingRequests.Dequeue();

        OnTokenStackAnimateEvent?.Invoke(new TokenStack()
        {
            Token = request.Token,
            Amount = 1,
        }, new FloatingTokenParameters()
        {
            TargetStack = Stacks.Find(s => s.Token.ID == request.Token.ID),
            WorldOrigin = request.Position,
            AnimationDuration = TOKEN_ANIM_DURATION
        });

        lastTokenAnimationTime = Time.time;

        DOVirtual.DelayedCall(TOKEN_ANIM_DURATION, () => AddToken(request.Token.ID, 1));
        DOVirtual.DelayedCall(TOKEN_ANIM_DELAY, () => RefreshRequests());
    }

    // Add Token based on TokenID, with optional amount (default is 1)
    public void AddToken(TokenID TokenID, int amount = 1)
    {
        // Try to find an existing stack with the same Token ID
        TokenStack existingStack = Stacks.Find(stack => stack.Token.ID == TokenID);

        if (existingStack != null)
        {
            // Stack exists, so we just increase the amount
            existingStack.Amount += amount;
            OnStackUpdatedEvent.Invoke(existingStack); // Notify about stack update
        }
        else
        {
            // No existing stack, create a new one using the extension method
            Token newToken = TokenID.ToToken(); // Using extension method to get the Token instance
            TokenStack newStack = new TokenStack { Token = newToken, Amount = amount };
            Stacks.Add(newStack);
            OnNewStackCreatedEvent.Invoke(newStack); // Notify about new stack
        }
    }

    // Remove a Token or reduce the amount in the stack
    public void RemoveToken(TokenID TokenID, int amount = 1)
    {
        TokenStack existingStack = Stacks.Find(stack => stack.Token.ID == TokenID);

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
                // Stack still has Tokens, so just update it
                OnStackUpdatedEvent.Invoke(existingStack); // Notify about stack update
            }
        }
    }
    public bool CanAfford(TokenStack requiredStack)
    {
        // Find the player's stack for the required token type
        TokenStack playerStack = Stacks.Find(stack => stack.Token == requiredStack.Token);

        // Check if the player has enough tokens in that stack
        return playerStack != null && playerStack.Amount >= requiredStack.Amount;
    }
}

internal class TokenAnimationRequest
{
    public Token Token;
    public Vector3 Position;
}

public static class TokenExtensions
{
    public static Token ToToken(this TokenID TokenID)
    {
        return ScriptableObjectContainerProvider.Instance.Tokens.All.Find(Token => Token.ID == TokenID);
    }
}