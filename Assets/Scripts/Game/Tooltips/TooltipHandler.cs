using System.Collections.Generic;
using Engine;
using UnityEngine;

public class TooltipHandler : SingletonBehaviour<TooltipHandler>
{
    [SerializeField] private TooltipUISlice dummy; 
    private List<TooltipInstance> activeTooltips = new List<TooltipInstance>();

    protected override void Awake()
    {
        base.Awake();
        dummy.gameObject.SetActive(false);
    }

    public void Show(Vector3 position, string content, object requestor)
    {
        // Check if there's already a tooltip for the requestor
        if (activeTooltips.Exists(t => t.Requestor == requestor))
        {
            Debug.LogWarning("Tooltip for this requestor already exists.");
            return;
        }

        TooltipUISlice tooltipUISlice = Instantiate(dummy, transform);
        TooltipData tooltipData = new TooltipData
        {
            Content = content
        };

        tooltipUISlice.Init(tooltipData);
        tooltipUISlice.transform.position = position;

        activeTooltips.Add(new TooltipInstance
        {
            Requestor = requestor,
            Slice = tooltipUISlice
        });

        tooltipUISlice.Show();
    }
    public void Hide(object requestor)
    {
        TooltipInstance instance = activeTooltips.Find(t => t.Requestor == requestor);
        if (instance == null)
            return;

        TooltipUISlice tooltipUISlice = instance.Slice;
        if (tooltipUISlice == null)
            return;

        tooltipUISlice.Hide(() =>
        {
            Destroy(instance.Slice.gameObject);
            activeTooltips.Remove(instance);
        });
    }
    private class TooltipInstance
    {
        public object Requestor;
        public TooltipUISlice Slice;
    }
}

public class TooltipData
{
    public string Content;
}
