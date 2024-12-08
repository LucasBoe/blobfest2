using System.Collections.Generic;
using Engine;
using UnityEngine;

[SingletonSettings(SingletonLifetime.Scene, _canBeGenerated:false)]
public class BuildingProvider : SingletonBehaviour<BuildingProvider>
{
    [SerializeField] private List<BuildingBehaviour> buildingPrefabs = new();
    public bool TryGetPrefab(Card card, out BuildingBehaviour building)
    {
        foreach (var prefab in buildingPrefabs)
        {
            if (prefab.Sourcecard == card)
            {
                building = prefab;
                return true;
            }
        }
        
        building = null;
        return false;
    }
}