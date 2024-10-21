using UnityEngine;
using UnityEngine.UI;

internal class TokenUISlice : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TMPro.TextMeshProUGUI TokenAmountText; // Text element for the Token's amount

    private TokenStack stack; // Reference to the Token stack

    // Method to set the Token stack and update the UI initially
    public void Initialize(TokenStack stack)
    {
        this.stack = stack;
        this.icon.sprite = stack.Token.Sprite;

        Refresh(); // Update UI immediately after assigning the stack
    }

    // Method to refresh the UI when the stack is updated
    public void Refresh()
    {
        if (stack != null)
        {
            TokenAmountText.text = stack.Amount.ToString(); // Update Token amount
        }
    }
}
