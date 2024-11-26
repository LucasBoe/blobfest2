using UnityEngine;

public class CustomCursorHandler : MonoBehaviour
{
    public CustomCursorUI customCursor;
    public Sprite defaultCursor;
    public Sprite hoverCursor;

    private void Start()
    {
        // Set the default cursor sprite
        if (customCursor != null)
        {
            customCursor.SetCursorSprite(defaultCursor);
        }
    }

    private void OnMouseEnter()
    {
        // Change to hover cursor when mouse enters a specific area
        if (customCursor != null)
        {
            customCursor.SetCursorSprite(hoverCursor);
        }
    }

    private void OnMouseExit()
    {
        // Revert to default cursor
        if (customCursor != null)
        {
            customCursor.SetCursorSprite(defaultCursor);
        }
    }
}
