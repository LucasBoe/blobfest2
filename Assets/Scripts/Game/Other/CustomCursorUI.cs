using UnityEngine;
using UnityEngine.UI;

public class CustomCursorUI : MonoBehaviour
{
    // Reference to the UI Image used as the custom cursor
    public Image cursorImage;

    // Offset for cursor alignment, if needed
    public Vector2 cursorOffset = Vector2.zero;

    private void Update()
    {
        if (cursorImage != null)
        {
            // Get the current mouse position and apply the offset
            Vector2 mousePosition = Input.mousePosition;
            cursorImage.transform.position = mousePosition + cursorOffset;
            Cursor.visible = false;
        }
    }

    /// <summary>
    /// Sets the cursor sprite dynamically.
    /// </summary>
    /// <param name="sprite">The new cursor sprite</param>
    public void SetCursorSprite(Sprite sprite)
    {
        if (cursorImage != null && sprite != null)
        {
            cursorImage.sprite = sprite;
        }
    }

    /// <summary>
    /// Toggles the visibility of the custom cursor.
    /// </summary>
    /// <param name="visible">True to show, false to hide</param>
    public void ToggleCursorVisibility(bool visible)
    {
        if (cursorImage != null)
        {
            cursorImage.enabled = visible;
        }
    }
}