using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConstructionActionUICardSlice : MonoBehaviour, IUISlice<PotentialConstruction>
{
    public Button Button;
    public TMP_Text CardNameLabel;
    public RectTransform CostContainer;
    public ConstructionSelectionAction ConstructionSelectionAction;

    private PotentialConstruction data;

    public void Init(PotentialConstruction data)
    {
        this.data = data;
        
        CardNameLabel.text = data.ConstructionType.ToString();
        Button.onClick.AddListener(Select);
        var dummy = CostContainer.GetChild(0);
        foreach (var resource in this.data.resourcesNeeded)
        {
            var instance = Instantiate(dummy);
            instance.GetComponentInChildren<Image>().sprite = resource.ResourceType.ToIcon();
            instance.GetComponentInChildren<TMP_Text>().text = resource.Amount.ToString();
        }
        dummy.gameObject.SetActive(false);
    }
    private void Select() => ConstructionSelectionAction.Select(data);
}