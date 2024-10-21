using UnityEngine;
using UnityEngine.UI;

internal class CardUISlice : MonoBehaviour
{
    [SerializeField] Transform followRoot;
    [SerializeField] Image image;
    [SerializeField] TMPro.TextMeshProUGUI cardNameText; // Text element for the card's name
    [SerializeField] TMPro.TextMeshProUGUI cardAmountText; // Text element for the card's amount

    private CardStack stack; // Reference to the card stack
    private bool tween = false;


    // Method to set the card stack and update the UI initially
    public void Initialize(CardStack stack, bool tween = true)
    {
        this.stack = stack;
        this.image.sprite = stack.Card.SpriteRegular;

        this.tween = tween;

        if (tween)
            followRoot.parent = transform.parent;

        Refresh(); // Update UI immediately after assigning the stack
    }

    // Method to refresh the UI when the stack is updated
    public void Refresh()
    {
        if (stack != null)
        {
            cardNameText.text = stack.Card.ID.ToString(); // Update card name
            cardAmountText.text = stack.Amount.ToString(); // Update card amount
        }
    }
    private void Update()
    {
        if (!tween)
            return;

        followRoot.position = Vector3.Lerp(followRoot.position, transform.position, Time.deltaTime * 4f);
    }
    private void OnDestroy()
    {
        Destroy(followRoot.gameObject);
    }
}
