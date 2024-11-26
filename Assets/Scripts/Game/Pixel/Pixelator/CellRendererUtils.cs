using System.Collections.Generic;
using UnityEngine;

public static class CellRendererUtils
{
    // Draw a line on the texture using Bresenham's algorithm
    public static void DrawLine(Texture2D texture, Vector2 p1, Vector2 p2, Color color, Vector2 origin, int pixelsPerUnit)
    {
        int x0 = Mathf.FloorToInt((p1.x - origin.x) * pixelsPerUnit);
        int y0 = Mathf.FloorToInt((p1.y - origin.y) * pixelsPerUnit);
        int x1 = Mathf.FloorToInt((p2.x - origin.x) * pixelsPerUnit);
        int y1 = Mathf.FloorToInt((p2.y - origin.y) * pixelsPerUnit);

        int dx = Mathf.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
        int dy = Mathf.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
        int err = (dx > dy ? dx : -dy) / 2, e2;

        while (true)
        {
            texture.SetPixel(x0, y0, color); // Draw the pixel
            if (x0 == x1 && y0 == y1) break;

            e2 = err;
            if (e2 > -dx) { err -= dy; x0 += sx; }
            if (e2 < dy) { err += dx; y0 += sy; }
        }
    }

    // Draw a thick line by expanding the line thickness
    public static void DrawThickLine(Texture2D texture, Vector2 p1, Vector2 p2, Color color, Vector2 origin, int pixelsPerUnit, float thickness)
    {
        int minThickness = Mathf.RoundToInt(-thickness * pixelsPerUnit);
        int maxThickness = Mathf.RoundToInt(thickness * pixelsPerUnit);

        for (int i = minThickness; i <= maxThickness; i++)
        {
            for (int j = minThickness; j <= maxThickness; j++)
            {
                DrawLine(texture, p1 + new Vector2(i / (float)pixelsPerUnit, j / (float)pixelsPerUnit),
                         p2 + new Vector2(i / (float)pixelsPerUnit, j / (float)pixelsPerUnit),
                         color, origin, pixelsPerUnit);
            }
        }
    }

    // Fill a polygon on the texture
    public static void FillPolygon(Texture2D texture, Vector2[] poly, Color fillColor, Vector2 origin, int pixelsPerUnit)
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

                if (IsPointInPolygon(poly, worldPos))
                {
                    texture.SetPixel(x, y, fillColor);
                }
            }
        }
    }

    // Determine if a point is inside a polygon
    public static bool IsPointInPolygon(Vector2[] poly, Vector2 point)
    {
        int n = poly.Length;
        bool inside = false;

        for (int i = 0, j = n - 1; i < n; j = i++)
        {
            if (((poly[i].y > point.y) != (poly[j].y > point.y)) &&
                (point.x < (poly[j].x - poly[i].x) * (point.y - poly[i].y) / (poly[j].y - poly[i].y) + poly[i].x))
            {
                inside = !inside;
            }
        }

        return inside;
    }

    // Calculate the bounds of a polygon
    public static Bounds GetPolygonBounds(Vector2[] poly)
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

    // Calculate the overall bounds for a list of polygons
    public static Bounds GetOverallBounds(List<Vector2[]> polygons)
    {
        Vector2 min = polygons[0][0];
        Vector2 max = polygons[0][0];

        foreach (var poly in polygons)
        {
            foreach (var v in poly)
            {
                min = Vector2.Min(min, v);
                max = Vector2.Max(max, v);
            }
        }

        return new Bounds((min + max) / 2, max - min);
    }
}
