using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using Engine;

[SingletonSettings(SingletonLifetime.Scene, _canBeGenerated: true, _eager: true)]
public class MapCellGridDisplayer : SingletonBehaviour<MapCellGridDisplayer>, IDelayedStartObserver
{
    public int pixelsPerUnit = 16;  // Pixel resolution
    public bool ThickBorders = false;
    [SerializeField, ShowIf("ThickBorders")] public float borderThickness = 0.5f; // Thickness of borders
    public Color borderColor = new Color(1, 1, 1, 0.2f); // Border color
    [SerializeField] private bool fillPolygonsWithRandomColors = false;

    [SerializeField] private SpriteRenderer spriteRenderer;

    public void DelayedStart()
    {
        if (spriteRenderer == null)
        {
            // Dynamically create the SpriteRenderer if not already set
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        var polygons = MapHandler.Instance.MapData.Cells.Select(c => c.Edges).ToList();
        GenerateVoronoiSprite(polygons);
    }

    private void GenerateVoronoiSprite(List<Vector2[]> polygons)
    {
        if (polygons == null || polygons.Count == 0)
        {
            Debug.LogWarning("Polygons list is empty or null.");
            return;
        }

        // Calculate bounds
        Bounds bounds = CellRendererUtils.GetOverallBounds(polygons);
        int textureWidth = Mathf.CeilToInt(bounds.size.x * pixelsPerUnit);
        int textureHeight = Mathf.CeilToInt(bounds.size.y * pixelsPerUnit);

        // Create a texture
        Texture2D texture = new Texture2D(textureWidth, textureHeight, TextureFormat.ARGB32, false);
        texture.filterMode = FilterMode.Point;
        Color[] clearPixels = new Color[textureWidth * textureHeight];
        for (int i = 0; i < clearPixels.Length; i++) clearPixels[i] = Color.clear;
        texture.SetPixels(clearPixels);

        // Origin for mapping coordinates
        Vector2 origin = bounds.min;

        // Fill polygons with random colors if enabled
        if (fillPolygonsWithRandomColors)
        {
            foreach (var polygon in polygons)
            {
                Color randomColor = new Color(Random.value, Random.value, Random.value);
                CellRendererUtils.FillPolygon(texture, polygon, randomColor, origin, pixelsPerUnit);
            }
        }

        // Draw polygon edges
        foreach (var polygon in polygons)
        {
            for (int i = 0; i < polygon.Length; i++)
            {
                Vector2 p1 = polygon[i];
                Vector2 p2 = polygon[(i + 1) % polygon.Length];

                if (ThickBorders)
                {
                    CellRendererUtils.DrawThickLine(texture, p1, p2, borderColor, origin, pixelsPerUnit, borderThickness);
                }
                else
                {
                    CellRendererUtils.DrawLine(texture, p1, p2, borderColor, origin, pixelsPerUnit);
                }
            }
        }

        // Apply texture changes and create a sprite
        texture.Apply();
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, textureWidth, textureHeight), new Vector2(0.5f, 0.5f), pixelsPerUnit);
        spriteRenderer.sprite = sprite;
        transform.position = bounds.center;
    }
}