using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class ConstructionSiteBehaviour : CellBehaviour, ICanReceive<RessourceCard>
{
    PotentialConstruction selectedConstruction;
    public static new CellType AssociatedCellType => CellType.ConstructionSite;
    private ConstructionSelectionAction constructionSelectionAction;
    public override void Enter()
    {
        constructionSelectionAction = new ConstructionSelectionAction(
            Context.Cell,
            SelectionCallback,
            new PotentialConstruction(CellType.Mill, 
                new ResourceAmountPair(ResourceType.Wood, 3), 
                new ResourceAmountPair(ResourceType.Stone, 2)
                ));
    }

    private void SelectionCallback(PotentialConstruction construction)
    {
        selectedConstruction = construction;
    }
    public override void Exit()
    {
        
    }
    public bool CanReceiveCard(RessourceCard card)
    {
        if (CurrentDropAction != null)
            return CurrentDropAction.CanReceiveCard(card);

        switch (card.AssociatedResourceType)
        {
            case ResourceType.Wood:
            case ResourceType.Villager:
                return true;
            
        }
        
        return false;
    }

    public void DoReceiveCard(RessourceCard card)
    {
        Debug.Log("DoReceiveCard");
        
        if (CurrentDropAction != null)
        {
            CurrentDropAction.DoReceiveCard(card);
            return;
        }
        
        switch (card.AssociatedResourceType)
        {
            case ResourceType.Wood:
                CurrentDropAction = new DropAction(Context.Cell, ResourceType.Wood, new PotentialDropCard(3, CardID.Builder));
                break;
            
            case ResourceType.Villager:
                CurrentDropAction = new DropAction(Context.Cell, ResourceType.Villager, new PotentialDropCard(2, CardID.Settler));
                break;
        }
        
        CurrentDropAction?.OnEndEvent.AddListener(RemoveDropAction);
    }

    private void RemoveDropAction()
    {
        CurrentDropAction.OnEndEvent.RemoveListener(RemoveDropAction);
        CurrentDropAction = null;
    }
}