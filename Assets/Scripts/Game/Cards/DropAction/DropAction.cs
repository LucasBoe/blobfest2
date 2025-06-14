using System;
using System.Collections;
using System.Collections.Generic;
using Engine;
using UnityEngine;
using Event = Engine.Event;

public class ActionBase
{
    public Cell Cell { get; private set; }
}

public class ConstructionSelectionAction : ActionBase
{
    public Cell Cell;
    public PotentialConstruction[] PotentialConstructions;
    private Action<PotentialConstruction> callback;
    public ConstructionSelectionAction(Cell cell, Action<PotentialConstruction> callback, params PotentialConstruction[] potentialConstructions)
    {
        this.Cell = cell;
        this.callback = callback;
        this.PotentialConstructions = potentialConstructions;
        
        ActionHandler.Instance.OnStartNewConstructionSelectionActionEvent?.Invoke(this);
    }

    public void Select(PotentialConstruction potentialConstruction)
    {
        callback?.Invoke(potentialConstruction);
        ActionHandler.Instance.OnEndConstructionSelectionActionEvent?.Invoke(this);
    }
}

public class PotentialConstruction
{
    public CellType ConstructionType { get; set; }
    public List<ResourceAmountPair> resourcesNeeded { get; set; }
    public PotentialConstruction(CellType type, params ResourceAmountPair[] resources)
    {
        ConstructionType = type;
        resourcesNeeded = new List<ResourceAmountPair>(resources);
    }
}

public class DropAction : ActionBase
{
    public Cell Cell { get; private set; }
    public ResourceType InputResourceType { get; private set; }
    public PotentialDropCard[] Cards { get; private set; }
    public int CurrentAmount { get; private set; } = 1;
    
    public Event OnEndEvent = new();
    public Event OnRefreshValidationsEvent = new();
    public DropAction(Cell cell, ResourceType inputResourceType, params PotentialDropCard[] cards)
    {
        this.Cell = cell;
        this.InputResourceType = inputResourceType;
        this.Cards = cards;
        
        foreach (PotentialDropCard card in cards)
            card.DropAction = this;
        
        ActionHandler.Instance.OnStartNewDropActionEvent?.Invoke(this);
    }
    public bool CanReceiveCard(RessourceCard card)
    {
        if (InputResourceType != card.AssociatedResourceType)
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
    public void Select(PotentialDropCard potentialDropCard)
    {
        OnEndEvent?.Invoke();
        ActionHandler.Instance.OnEndDropActionEvent?.Invoke(this);
        CollectibleSpawner.Instance.SpawnAt(potentialDropCard.Card.ToCard(), Cell.Center);
    }
}

[SingletonSettings(SingletonLifetime.Persistant)]
public class ActionHandler : Singleton<ActionHandler>
{
    public Event<DropAction> OnStartNewDropActionEvent = new();
    public Event<DropAction> OnEndDropActionEvent = new();
    public Event<ConstructionSelectionAction> OnStartNewConstructionSelectionActionEvent = new();
    public Event<ConstructionSelectionAction> OnEndConstructionSelectionActionEvent = new();
}

public class PotentialDropCard
{
    public int Amount;
    public CardID Card;
    public bool IsReached = false;
    public DropAction DropAction;
    public PotentialDropCard(int amount, CardID card)
    {
        Amount = amount;
        Card = card;
    }
    public void Select()
    {
        DropAction.Select(this);
    }
}
