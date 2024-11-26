using UnityEngine;

public class CellLineRenderer : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private int pixelsPerUnit = 16;
    [SerializeField] private Color borderColor = Color.white;

    public void DrawCellOutline(Vector2[] cellEdges)
    {
        if (cellEdges == null || cellEdges.Length == 0)
        {
            spriteRenderer.enabled = false;
            return;
        }

        // Calculate bounds
        Bounds bounds = CellRendererUtils.GetPolygonBounds(cellEdges);
        int textureWidth = Mathf.CeilToInt(bounds.size.x * pixelsPerUnit);
        int textureHeight = Mathf.CeilToInt(bounds.size.y * pixelsPerUnit);

        // Create texture
        Texture2D texture = new Texture2D(textureWidth, textureHeight, TextureFormat.ARGB32, false);
        texture.filterMode = FilterMode.Point;
        Color[] clearPixels = new Color[textureWidth * textureHeight];
        for (int i = 0; i < clearPixels.Length; i++) clearPixels[i] = Color.clear;
        texture.SetPixels(clearPixels);

        // Draw edges
        Vector2 origin = bounds.min;
        for (int i = 0; i < cellEdges.Length; i++)
        {
            Vector2 p1 = cellEdges[i];
            Vector2 p2 = cellEdges[(i + 1) % cellEdges.Length];
            CellRendererUtils.DrawLine(texture, p1, p2, borderColor, origin, pixelsPerUnit);
        }

        // Apply texture and create sprite
        texture.Apply();
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, textureWidth, textureHeight), new Vector2(0.5f, 0.5f), pixelsPerUnit);
        spriteRenderer.sprite = sprite;
        spriteRenderer.enabled = true;
        transform.position = bounds.center;
    }
}
