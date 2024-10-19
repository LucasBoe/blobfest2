using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Engine;
using NaughtyAttributes;

public class VoronoiSpriteRenderer : MonoBehaviour, IDelayedStartObserver
{
    public int pixelsPerUnit = 16;    // Pixel resolution
    public bool ThickBorders = false;
    [SerializeField, ShowIf("ThickBorders")] public float borderThickness = .5f;   // Thickness of the borders between polygons
    public Color borderColor = Color.white; // Color of the border lines

    [SerializeField] bool fillPolygonsWithRandomColors = false;

    public void DelayedStart()
    {
        GenerateVoronoiSprite(MapHandler.Instance.MapData.Cells.Select(c => c.Edges).ToList());
    }

    void GenerateVoronoiSprite(List<Vector2[]> polygons)
    {
        if (polygons == null || polygons.Count == 0) return;

        // Calculate the bounds of the entire set of polygons
        Bounds bounds = GetOverallBounds(polygons);

        // Calculate texture size based on the bounds
        int textureWidth = Mathf.CeilToInt(bounds.size.x * pixelsPerUnit);
        int textureHeight = Mathf.CeilToInt(bounds.size.y * pixelsPerUnit);

        // Create a new texture
        Texture2D texture = new Texture2D(textureWidth, textureHeight, TextureFormat.ARGB32, false);
        texture.filterMode = FilterMode.Point;  // Pixel-perfect rendering

        Color[] pixels = Enumerable.Repeat(Color.clear, textureWidth * textureHeight).ToArray();
        texture.SetPixels(pixels);

        // Get the origin for mapping world coordinates to texture coordinates
        Vector2 origin = new Vector2(bounds.min.x, bounds.min.y);

        if (fillPolygonsWithRandomColors)
        {
            // Fill polygons with random colors
            foreach (var poly in polygons)
            {
                Color randomColor = new Color(Random.value, Random.value, Random.value);
                FillPolygon(texture, poly, randomColor, origin);
            }
        }

        // Draw borders between polygons
        foreach (var poly in polygons)
        {
            DrawPolygonEdges(texture, poly, origin);
        }

        texture.Apply();

        // Create a sprite from the texture
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, textureWidth, textureHeight), new Vector2(0.5f, 0.5f), pixelsPerUnit);

        // Attach the sprite to the SpriteRenderer component
        SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;

        // Position the sprite to match the bounds center
        transform.position = bounds.center;
    }

    // Utility to calculate the overall bounds of all polygons
    Bounds GetOverallBounds(List<Vector2[]> polys)
    {
        Vector2 min = polys[0][0];
        Vector2 max = polys[0][0];

        foreach (var poly in polys)
        {
            foreach (var v in poly)
            {
                min = Vector2.Min(min, v);
                max = Vector2.Max(max, v);
            }
        }

        return new Bounds((min + max) / 2, max - min);
    }

    // Fill a polygon on the texture with a color
    void FillPolygon(Texture2D texture, Vector2[] poly, Color fillColor, Vector2 origin)
    {
        Bounds polyBounds = GetPolygonBounds(poly);

        int minX = Mathf.FloorToInt((polyBounds.min.x - origin.x) * pixelsPerUnit);
        int minY = Mathf.FloorToInt((polyBounds.min.y - origin.y) * pixelsPerUnit);
        int maxX = Mathf.CeilToInt((polyBounds.max.x - origin.x) * pixelsPerUnit);
        int maxY = Mathf.CeilToInt((polyBounds.max.y - origin.y) * pixelsPerUnit);

        for (int x = minX; x < maxX; x++)
        {
            for (int y = minY; y < maxY; y++)
            {
                Vector2 worldPos = new Vector2(x / (float)pixelsPerUnit + origin.x, y / (float)pixelsPerUnit + origin.y);

                if (MapDataUtil.IsInPolygon(poly, worldPos))
                {
                    texture.SetPixel(x, y, fillColor);
                }
            }
        }
    }

    // Draw the edges of a polygon on the texture
    void DrawPolygonEdges(Texture2D texture, Vector2[] poly, Vector2 origin)
    {
        for (int i = 0; i < poly.Length; i++)
        {
            Vector2 p1 = poly[i];
            Vector2 p2 = poly[(i + 1) % poly.Length];

            if (ThickBorders)
                DrawLine(texture, p1, p2, borderColor, origin);
            else
                DrawLine1Px(texture, p1, p2, borderColor, origin);
        }
    }

    // Draw a line between two points on the texture
    void DrawLine(Texture2D texture, Vector2 p1, Vector2 p2, Color color, Vector2 origin)
    {
        int x0 = Mathf.FloorToInt((p1.x - origin.x) * pixelsPerUnit);
        int y0 = Mathf.FloorToInt((p1.y - origin.y) * pixelsPerUnit);
        int x1 = Mathf.FloorToInt((p2.x - origin.x) * pixelsPerUnit);
        int y1 = Mathf.FloorToInt((p2.y - origin.y) * pixelsPerUnit);

        // Bresenham's line algorithm to draw a pixel-perfect line
        int dx = Mathf.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
        int dy = Mathf.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
        int err = (dx > dy ? dx : -dy) / 2, e2;

        int minThickness = Mathf.RoundToInt(-borderThickness);
        int maxThickness = Mathf.RoundToInt(borderThickness);

        while (true)
        {
            for (int i = minThickness; i <= maxThickness; i++)
            {
                for (int j = minThickness; j <= maxThickness; j++)
                {
                    texture.SetPixel(x0 + i, y0 + j, color);
                }
            }

            if (x0 == x1 && y0 == y1) break;
            e2 = err;
            if (e2 > -dx) { err -= dy; x0 += sx; }
            if (e2 < dy) { err += dx; y0 += sy; }
        }
    }

    // Draw a 1-pixel thick line between two points on the texture
    void DrawLine1Px(Texture2D texture, Vector2 p1, Vector2 p2, Color color, Vector2 origin)
    {
        int x0 = Mathf.FloorToInt((p1.x - origin.x) * pixelsPerUnit);
        int y0 = Mathf.FloorToInt((p1.y - origin.y) * pixelsPerUnit);
        int x1 = Mathf.FloorToInt((p2.x - origin.x) * pixelsPerUnit);
        int y1 = Mathf.FloorToInt((p2.y - origin.y) * pixelsPerUnit);

        // Bresenham's line algorithm to draw a 1-pixel thick line
        int dx = Mathf.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
        int dy = Mathf.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
        int err = (dx > dy ? dx : -dy) / 2, e2;

        while (true)
        {
            texture.SetPixel(x0, y0, color);  // Draw the pixel at the current point

            if (x0 == x1 && y0 == y1) break;
            e2 = err;
            if (e2 > -dx) { err -= dy; x0 += sx; }
            if (e2 < dy) { err += dx; y0 += sy; }
        }
    }

    // Utility to calculate the bounds of a polygon
    Bounds GetPolygonBounds(Vector2[] poly)
    {
        Vector2 min = poly[0];
        Vector2 max = poly[0];

        foreach (Vector2 v in poly)
        {
            min = Vector2.Min(min, v);
            max = Vector2.Max(max, v);
        }

        return new Bounds((min + max) / 2, max - min);
    }
}