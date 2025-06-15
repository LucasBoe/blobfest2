using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

internal class ConstructionUISlice : MonoBehaviour, IUISlice<PotentialConstruction>
{
    public RectTransform CostContainer;
    private PotentialConstruction data;
    private Dictionary<ResourceType, TMP_Text> costTexts = new ();
    public void Init(PotentialConstruction data)
    {
        this.data = data;
        var dummy = CostContainer.GetChild(0);
        foreach (var resource in this.data.resourcesNeeded)
        {
            var instance = Instantiate(dummy, dummy.transform.parent);
            instance.GetComponentInChildren<Image>().sprite = resource.ResourceType.ToIcon();
            instance.GetComponentInChildren<TMP_Text>().text = resource.Amount.ToString();
            costTexts.Add(resource.ResourceType, instance.GetComponentInChildren<TMP_Text>());
            Debug.Log($"Needs {resource} and thereof {resource.Amount}.");
        }
        dummy.gameObject.SetActive(false);
        data.OnRefreshEvent.AddListener(OnRefresh);
    }
    private void OnRefresh()
    {
        foreach (var entry in costTexts)
        {
            int amount = data.resourcesNeeded.First(r => r.ResourceType == entry.Key).Amount;
            Debug.Log($"Refresh... {entry.Key}: {amount}");
            if (amount <= 0)
            {
                Destroy(entry.Value);
                costTexts.Remove(entry.Key);
            }
            else
            {
                costTexts[entry.Key].text = amount.ToString();
            }
        }
    }
    private void OnDestroy()
    {
        data?.OnRefreshEvent.RemoveListener(OnRefresh);
    }
}