using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class BuildingSlotUISlice : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private GameObject blockedRoot, emptyRoot, fullRoot;
    [SerializeField] private Image cardImage, cardOutlineImage, slotOutlineImage;
    
    private BuildingBehaviour associatedBuildingBehaviour;
    private SlotState slotState;
    private BuildingsModule_CellSelectionUISlice buildingUISlice;
    
    public BuildingBehaviour AssociatedBuildingBehaviour => associatedBuildingBehaviour;
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
    public void Init(BuildingBehaviour buildingBehaviour, BuildingsModule_CellSelectionUISlice buildingUISlice)
    {
        Debug.Log("Init: " + buildingBehaviour.Sourcecard.name);
        
        this.buildingUISlice = buildingUISlice;
        associatedBuildingBehaviour = buildingBehaviour;
        cardImage.sprite = associatedBuildingBehaviour.Sourcecard.SpriteRegular;
        cardOutlineImage.color = associatedBuildingBehaviour.Sourcecard.Color;
        slotOutlineImage.color = Color.clear;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        Vector2 pos = Input.mousePosition;
        
        if (slotState == SlotState.Blocked)
        {
            TooltipHandler.Instance.ShowUI(pos,"Level up this settlement to unlock more slots.", this);
        }
        else if (slotState == SlotState.Empty)
        {
            TooltipHandler.Instance.ShowUI(pos,"You can place buildings on this cell by playing a card on it.", this);
        }
        else
        {
            slotOutlineImage.color = Color.white;
            TooltipHandler.Instance.ShowUI(pos,associatedBuildingBehaviour.Sourcecard.ID.ToString(), this);
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipHandler.Instance.Hide(this);
        slotOutlineImage.color = Color.clear;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        buildingUISlice.Inspect(associatedBuildingBehaviour);
    }
}