using System;
using UnityEngine;

internal class ConstructionActionUISlice : MonoBehaviour, IUISlice<ConstructionSelectionAction>
{
    [SerializeField] private ConstructionActionUICardSlice cardDummy;
    private void Awake()
    {
        cardDummy.gameObject.SetActive(false);
    }
    public void Init(ConstructionSelectionAction data)
    {
        foreach (var card in data.PotentialConstructions)
        {
            var instance = InstantiationUtil.InstantiateFromDummy(cardDummy, card);
            instance.ConstructionSelectionAction = data;
        }
    }
}