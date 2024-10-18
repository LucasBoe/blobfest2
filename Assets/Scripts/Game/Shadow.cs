using System.Collections;
using System.Collections.Generic;
using Engine;
using UnityEngine;

public class Shadow : MonoBehaviour
{
    [SerializeField] SpriteRenderer shadowRenderer;

    private void Awake()
    {
        GetComponent<SpriteRenderer>().color = ShadowHandler.Instance.SHADOW_COLOR.With(a: .33f);
    }
}

public class ShadowHandler : Singleton<ShadowHandler>
{
    public Color SHADOW_COLOR => FromHTMLString("#222");

    private Dictionary<string, Color> resolvedHtmlColors = new();

    public Color FromHTMLString(string str)
    {
        if (resolvedHtmlColors.ContainsKey(str))
            return resolvedHtmlColors[str];

        if (ColorUtility.TryParseHtmlString(str, out var c))
        {
            resolvedHtmlColors.Add(str, c);
            return c;
        }

        return Color.cyan;
    }
}