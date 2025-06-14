using TMPro;
using UnityEngine;
using UnityEngine.UI;

internal class DropActionUICardSlice : MonoBehaviour, IUISlice<PotentialDropCard>
{
    [SerializeField] private Image cardImage;
    [SerializeField] private TMP_Text cardCostText;
    [SerializeField] private Button cardButton;
    public void Init(PotentialDropCard dropCard)
    {
        cardButton.onClick.AddListener(dropCard.Select);
        cardImage.sprite = dropCard.Card.ToCard().SpriteRegular;
        cardCostText.text = dropCard.Amount.ToString();
    }
    public void SetValid(bool keyIsReached)
    {
        cardButton.interactable = keyIsReached;
    }
}