using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Engine;

[SingletonSettings(SingletonLifetime.Persistant, _canBeGenerated: true, _eager: false)]
public class PixelSpriteResizer : Singleton<PixelSpriteResizer>
{
    private Dictionary<string, Sprite> resizedSpriteCache = new Dictionary<string, Sprite>();

    public Sprite GetResizedSprite(Sprite originalSprite, int targetWidth, int targetHeight)
    {
        string cacheKey = $"{originalSprite.name}_{targetWidth}x{targetHeight}";

        if (resizedSpriteCache.TryGetValue(cacheKey, out Sprite cachedSprite))
            return cachedSprite;
        

        Texture2D resizedTexture = ResizeTexture(originalSprite.texture, targetWidth, targetHeight);
        Sprite newSprite = Sprite.Create(
            resizedTexture,
            new Rect(0, 0, resizedTexture.width, resizedTexture.height),
            new Vector2(0.5f, 0.5f), // Pivot
            originalSprite.pixelsPerUnit
        );

        resizedSpriteCache[cacheKey] = newSprite;
        return newSprite;
    }

    private Texture2D ResizeTexture(Texture2D originalTexture, int targetWidth, int targetHeight)
    {
        Texture2D resizedTexture = new Texture2D(targetWidth, targetHeight, originalTexture.format, false);
        resizedTexture.filterMode = FilterMode.Point; 
        resizedTexture.wrapMode = originalTexture.wrapMode;

        for (int y = 0; y < targetHeight; y++)
        {
            for (int x = 0; x < targetWidth; x++)
            {
                int originalX = Mathf.FloorToInt((float)x / targetWidth * originalTexture.width);
                int originalY = Mathf.FloorToInt((float)y / targetHeight * originalTexture.height);
                resizedTexture.SetPixel(x, y, originalTexture.GetPixel(originalX, originalY));
            }
        }

        resizedTexture.Apply();
        return resizedTexture;
    }
}

