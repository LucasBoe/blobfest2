using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DropActionUISlice : MonoBehaviour
{
    private DropAction dropAction;

    [SerializeField] private TMP_Text currentAmountText;
    [SerializeField] private Image ressourceIconImage;
    
    [SerializeField] private DropActionUICardSlice cardSliceDummy;
    [SerializeField] private Dictionary<PotentialDropCard, DropActionUICardSlice> cardSlices = new();
    private void Awake()
    {
        cardSliceDummy.gameObject.SetActive(false);
    }
    public void Init(DropAction dropAction)
    {
        this.dropAction = dropAction;
        ressourceIconImage.sprite = dropAction.InputResourceType.ToIcon();
        
        dropAction.OnRefreshValidationsEvent.AddListener(RefreshValidations);

        foreach (var card in dropAction.Cards)
        {
            var instance = Instantiate(cardSliceDummy, cardSliceDummy.transform.parent);
            instance.gameObject.SetActive(true);
            instance.Init(card);
            cardSlices.Add(card, instance);
        }
        
        RefreshValidations();
    }
    private void OnDestroy()
    {
        dropAction?.OnRefreshValidationsEvent.RemoveListener(RefreshValidations);
    }
    private void RefreshValidations()
    {
        currentAmountText.text = dropAction.CurrentAmount.ToString();
        foreach (var slice in cardSlices)
            slice.Value.SetValid(slice.Key.IsReached);
    }
}