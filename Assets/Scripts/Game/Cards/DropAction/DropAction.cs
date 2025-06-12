using System.Collections;
using System.Collections.Generic;
using Engine;
using UnityEngine;
using Event = Engine.Event;

public class DropAction
{
    public Cell Cell { get; private set; }
    public RessourceType InputRessourceType { get; private set; }
    public PotentialCard[] Cards { get; private set; }
    public int CurrentAmount { get; private set; } = 1;
    
    public Event OnEndEvent = new();
    public Event OnRefreshValidationsEvent = new();
    public DropAction(Cell cell, RessourceType inputRessourceType, params PotentialCard[] cards)
    {
        this.Cell = cell;
        this.InputRessourceType = inputRessourceType;
        this.Cards = cards;
        
        foreach (PotentialCard card in cards)
            card.DropAction = this;
        
        ActionHandler.Instance.OnStartNewDropActionEvent?.Invoke(this);
    }
    public bool CanReceiveCard(RessourceCard card)
    {
        if (InputRessourceType != card.AssociatedRessourceType)
            return false;
        
        return true;
    }
    public void DoReceiveCard(RessourceCard card)
    {
        CurrentAmount++;
        RefreshValidations();
    }
    private void RefreshValidations()
    {
        foreach (var card in Cards)
            card.IsReached = CurrentAmount >= card.Amount;
        
        OnRefreshValidationsEvent?.Invoke();
    }
    public void Select(PotentialCard potentialCard)
    {
        OnEndEvent?.Invoke();
        ActionHandler.Instance.OnEndDropActionEvent?.Invoke(this);
        CollectibleSpawner.Instance.SpawnAt(potentialCard.Card.ToCard(), Cell.Center);
    }
}

[SingletonSettings(SingletonLifetime.Persistant)]
public class ActionHandler : Singleton<ActionHandler>
{
    public Event<DropAction> OnStartNewDropActionEvent = new();
    public Event<DropAction> OnEndDropActionEvent = new();
    protected override void OnCreate()
    {
        base.OnCreate();
        
    }
}

public class PotentialCard
{
    public int Amount;
    public CardID Card;
    public bool IsReached = false;
    public DropAction DropAction;
    public PotentialCard(int amount, CardID card)
    {
        Amount = amount;
        Card = card;
    }
    public void Select()
    {
        DropAction.Select(this);
    }
}
