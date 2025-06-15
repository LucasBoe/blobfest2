using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConstructionActionUICardSlice : MonoBehaviour, IUISlice<PotentialConstruction>
{
    public Button Button;
    public TMP_Text CardNameLabel;
    public RectTransform CostContainer;
    public ConstructionSelectionAction ConstructionSelectionAction;
    
    [SerializeField] private List<CellTypeSpritePair> cellTypeSpritePairs = new();
    
    private PotentialConstruction data;

    public void Init(PotentialConstruction data)
    {
        this.data = data;
        
        CardNameLabel.text = data.ConstructionType.ToString();
        ((Image)Button.targetGraphic).sprite = cellTypeSpritePairs.First(ctp => ctp.CellType == data.ConstructionType).Sprite;
        Button.onClick.AddListener(Select);
        var dummy = CostContainer.GetChild(0);
        foreach (var resource in this.data.resourcesNeeded)
        {
            var instance = Instantiate(dummy, dummy.transform.parent);
            instance.GetComponentInChildren<Image>().sprite = resource.ResourceType.ToIcon();
            instance.GetComponentInChildren<TMP_Text>().text = resource.Amount.ToString();
        }
        dummy.gameObject.SetActive(false);
    }
    private void Select() => ConstructionSelectionAction.Select(data);
}

[System.Serializable]
public class CellTypeSpritePair
{
    public CellType CellType;
    public Sprite Sprite;
}