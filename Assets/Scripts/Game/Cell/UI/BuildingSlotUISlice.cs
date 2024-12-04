using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class BuildingSlotUISlice : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject blockedRoot, emptyRoot, fullRoot;
    [SerializeField] private Image cardImage, cardOutlineImage, slotOutlineImage;
    
    private PlacedBuilding associatedBuilding;
    private SlotState slotState;
    public void SetState(BuildingSlotUISlice.SlotState state)
    {
        this.slotState = state;
        
        blockedRoot.SetActive(state == SlotState.Blocked);
        emptyRoot.SetActive(state == SlotState.Empty);
        fullRoot.SetActive(state == SlotState.Full);
    }
    public enum SlotState
    {
        Blocked,
        Empty,
        Full,
    }
    public void Init(PlacedBuilding building)
    {
        associatedBuilding = building;
        cardImage.sprite = associatedBuilding.Sourcecard.SpriteRegular;
        cardOutlineImage.color = associatedBuilding.Sourcecard.Color;
        slotOutlineImage.color = Color.clear;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        Vector2 pos = Input.mousePosition;
        
        if (slotState == SlotState.Blocked)
        {
            TooltipHandler.Instance.Show(pos,"Level up this settlement to unlock more slots.", this);
        }
        else if (slotState == SlotState.Empty)
        {
            TooltipHandler.Instance.Show(pos,"You can place buildings on this cell by playing a card on it.", this);
        }
        else
        {
            slotOutlineImage.color = Color.white;
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipHandler.Instance.Hide(this);
        slotOutlineImage.color = Color.clear;
    }
}