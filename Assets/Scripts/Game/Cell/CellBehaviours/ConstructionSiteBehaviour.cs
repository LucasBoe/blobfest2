using System;
using System.Collections.Generic;
using System.Linq;
using Engine;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class ConstructionSiteBehaviour : CellBehaviour, ICanReceive<RessourceCard>
{
    public static new CellType AssociatedCellType => CellType.ConstructionSite;
    PotentialConstruction selectedConstruction;
    private ConstructionSelectionAction constructionSelectionAction;
    private Dictionary<ResourceType, int> paidResources = new ();
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
        if (selectedConstruction == null)
            return false;

        var type = card.AssociatedResourceType;
        foreach (var resource in selectedConstruction.resourcesNeeded)
        {
            if (resource.ResourceType == type)
            {
                if (!paidResources.ContainsKey(type) || paidResources[type] < resource.Amount)
                    return true;
            }
        }
        
        return false;
    }
    public void DoReceiveCard(RessourceCard card)
    {
        var type = card.AssociatedResourceType;
        paidResources.AddOrInit(type, 1);

        
        foreach (var resource in selectedConstruction.resourcesNeeded)
        {
            if (resource.ResourceType == type)
            {
                if (!paidResources.ContainsKey(type) || paidResources[type] < resource.Amount)
                    return;
            }
        }
        
        Context.Cell.ChangeCellType(selectedConstruction.ConstructionType);
    }
}