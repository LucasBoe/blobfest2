using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class DealUISlice : MonoBehaviour
{
    [SerializeField] private Image cardIconImage;          // Image for displaying the card's regular sprite
    [SerializeField] private Image tokenIconImage;         // Image for displaying the token's sprite
    [SerializeField] private TextMeshProUGUI tokenAmountText; // Text for displaying the token amount
    [SerializeField] private Button dealButton;

    private Deal deal;

    // Initialize the DealUISlice with a Deal's data
    public void Initialize(Deal deal)
    {
        this.deal = deal;

        // Set card sprite to the card's regular sprite
        cardIconImage.sprite = deal.Goods.SpriteRegular;

        // Set token sprite and amount
        tokenIconImage.sprite = deal.Pay.Token.Sprite;
        tokenAmountText.text = deal.Pay.Amount.ToString();

        dealButton.onClick.AddListener(ExecuteDeal);

        // Optional: Set up interactions, visibility, or animations if needed
        UpdateInteractableState();
    }
    private void ExecuteDeal()
    {
        deal.Execute();
    }
    // Updates the interactable state of the slice based on payment availability
    private void UpdateInteractableState()
    {
        bool canAfford = TokenHandler.Instance.CanAfford(deal.Pay); // Assuming CanAfford checks against available tokens
        dealButton.interactable = canAfford;
        cardIconImage.color = canAfford ? Color.white : Color.gray; // Grays out if unaffordable
        tokenIconImage.color = canAfford ? Color.white : Color.gray;
    }
}