using TMPro;
using UnityEngine;
using UnityEngine.UI;

internal class DropActionUICardSlice : MonoBehaviour
{
    [SerializeField] private Image cardImage;
    [SerializeField] private TMP_Text cardCostText;
    [SerializeField] private Button cardButton;
    public void Init(PotentialCard card)
    {
        cardButton.onClick.AddListener(card.Select);
        cardImage.sprite = card.Card.ToCard().SpriteRegular;
        cardCostText.text = card.Amount.ToString();
    }
    public void SetValid(bool keyIsReached)
    {
        cardButton.interactable = keyIsReached;
    }
}