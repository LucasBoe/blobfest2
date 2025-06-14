using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipUISlice : MonoBehaviour
{
    [SerializeField] private TMP_Text tooltipText; // Assign the Text component in the Inspector
    [SerializeField] private Image[] backgroundImages;

    private TooltipData data;
    private const float FULL_REVEAL_DURATION = 0.1f;

    public void Init(TooltipData data)
    {
        this.data = data;
        tooltipText.text = string.Empty;

        if (data.OverrideColor != null)
        {
            foreach (var image in backgroundImages)
            {
                image.color = data.OverrideColor.Value;
            }
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
        var content = data.Content;
        
        if (data.AllCaps)
            content = content.ToUpper();
        
        StartCoroutine(RevealText(content));
    }

    public void Hide(System.Action onComplete)
    {
        gameObject.SetActive(false);
        onComplete?.Invoke();
    }

    private IEnumerator RevealText(string content)
    {
        tooltipText.text = string.Empty;
        var wait = new WaitForSeconds(FULL_REVEAL_DURATION / content.Length);

        foreach (char c in content)
        {
            tooltipText.text += c;
            yield return wait;
        }
    }
}
