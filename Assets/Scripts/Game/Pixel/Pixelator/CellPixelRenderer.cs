using UnityEngine;

public class CellPixelRenderer : MonoBehaviour
{
    public const int PPU = 16;
    private float border = 1f;

    // To create the sprite at runtime
    [SerializeField] private SpriteRenderer fillRenderer, outlineRenderer;

    public void GenerateSprites(Vector2[] polygonVertices, float borderInPixels = 1f)
    {
        this.border = borderInPixels;

        Color randomColor = new Color(Random.value, Random.value, Random.value);

        // Find bounds of the polygon and expand by the border
        Bounds polygonBounds = GetPolygonBounds(polygonVertices);
        polygonBounds.Expand(borderInPixels / PPU);

        // Calculate size of the sprite in pixels
        int textureWidth = Mathf.CeilToInt(polygonBounds.size.x * PPU);
        int textureHeight = Mathf.CeilToInt(polygonBounds.size.y * PPU);

        // Create texture and set pixels
        Texture2D texture = new Texture2D(textureWidth, textureHeight, TextureFormat.ARGB32, false);
        texture.filterMode = FilterMode.Point; // Keep pixel-perfect

        // Get the offset for mapping world coordinates to texture pixels
        Vector2 origin = new Vector2(polygonBounds.min.x, polygonBounds.min.y);

        // Loop through the texture pixels and determine if they're inside the polygon
        for (int x = 0; x < textureWidth; x++)
        {
            for (int y = 0; y < textureHeight; y++)
            {
                Vector2 worldPos = new Vector2(x / (float)PPU + origin.x, y / (float)PPU + origin.y);

                // Check if the pixel is inside the polygon
                if (MapDataUtil.IsInPolygon(polygonVertices, worldPos))
                {
                    texture.SetPixel(x, y, randomColor);  // Inside the polygon, paint color
                }
                else
                {
                    texture.SetPixel(x, y, Color.clear);  // Outside the polygon, transparent
                }
            }
        }

        texture.Apply();

        // Create a sprite from the texture
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, textureWidth, textureHeight), new Vector2(0.5f, 0.5f), PPU);

        fillRenderer.sprite = sprite;
        outlineRenderer.sprite = sprite;

        fillRenderer.transform.position = polygonBounds.center;
        outlineRenderer.transform.position = polygonBounds.center;
    }

    // Utility to calculate the bounding box of a polygon
    private Bounds GetPolygonBounds(Vector2[] poly)
    {
        if (poly == null || poly.Length == 0) return new Bounds();

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