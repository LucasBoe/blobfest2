using System.Collections.Generic;
using Engine;
using UnityEngine;

public class TooltipHandler : SingletonBehaviour<TooltipHandler>
{
    [SerializeField] private TooltipUISlice dummy; 
    [SerializeField] private RectTransform worldSpaceParent, uiSpaceParent;
    private List<TooltipInstance> activeTooltips = new List<TooltipInstance>();
    protected override void Awake()
    {
        base.Awake();
        dummy.gameObject.SetActive(false);
    }

    public void ShowUI(Vector2 screenPosition, string content, object requestor)
    {
        Show(screenPosition, content, requestor, uiSpaceParent);
    }
    public void ShowWorld(Vector3 worldPosition, string content, object requestor)
    {
        Show(worldPosition, content, requestor, worldSpaceParent);
    }
    private void Show(Vector3 position, string content, object requestor, Transform parent)
    {
        if (activeTooltips.Exists(t => t.Requestor == requestor))
        {
            Debug.LogWarning("Tooltip for this requestor already exists.");
            return;
        }

        TooltipUISlice tooltipUISlice = Instantiate(dummy, parent);
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
        if (!tooltipUISlice)
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
