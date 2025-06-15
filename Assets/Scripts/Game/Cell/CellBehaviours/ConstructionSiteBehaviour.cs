using System;
using System.Collections.Generic;
using System.Linq;
using Engine;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

public class ConstructionSiteBehaviour : CellBehaviour, ICanReceive<RessourceCard>
{
    public static new CellType AssociatedCellType => CellType.ConstructionSite;
    PotentialConstruction selectedConstruction;
    private ConstructionSelectionAction constructionSelectionAction;
    private GameObject visuals;
    public override void Enter()
    {
        constructionSelectionAction = new ConstructionSelectionAction(
            Context.Cell,
            SelectionCallback, 
            new PotentialConstruction(CellType.Mill, 
                    new ResourceAmountPair(ResourceType.Wood, 3), 
                    new ResourceAmountPair(ResourceType.Stone, 2)
                ),
                new PotentialConstruction(CellType.Woodcutter, 
                    new ResourceAmountPair(ResourceType.Wood, 2)
                    ),
                new PotentialConstruction(CellType.StonemasonHut, 
                    new ResourceAmountPair(ResourceType.Wood, 4)
                )
            );
        
        var prefab = PrefabRefID.ConstructionSite.TryGetPrefab<Transform>();
        visuals = Instantiate(prefab, Context.Cell.Center).gameObject;
    }
    private void SelectionCallback(PotentialConstruction construction)
    {
        selectedConstruction = construction;
        ConstructionHandler.Instance.StartConstruction(construction);
    }
    public override void Exit()
    {
        Object.Destroy(visuals);
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
                if (resource.Amount > 0)
                    return true;
            }
        }
        
        return false;
    }
    public void DoReceiveCard(RessourceCard card)
    {
        var type = card.AssociatedResourceType;
        selectedConstruction.resourcesNeeded.First(r => r.ResourceType == type).Amount--;
        ConstructionHandler.Instance.RefreshProgression(selectedConstruction);

        
        foreach (var resource in selectedConstruction.resourcesNeeded)
        {
            if (resource.Amount > 0) 
                return;
        }

        ConstructionHandler.Instance.EndConstruction(selectedConstruction);
        Context.Cell.ChangeCellType(selectedConstruction.ConstructionType);
    }
}