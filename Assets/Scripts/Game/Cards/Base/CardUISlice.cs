using Engine;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

internal class CardUISlice : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField] Transform followRoot;
    [SerializeField] Image image, outline;
    [SerializeField] TMPro.TextMeshProUGUI cardNameText; // Text element for the card's name
    [SerializeField] TMPro.TextMeshProUGUI cardAmountText; // Text element for the card's amount

    private CardStack stack; // Reference to the card stack
    private bool tween = false;
    private Vector2 selectedOffset = new Vector2(0, 32f);
    private bool isSelected = false;

    private CardValidationRequest activeRequest;

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
        outline.color = activeRequest == null ? Color.white : (activeRequest.LastResult ? Color.green : Color.red);

        if (!tween)
            return;

        var targetPosition = (Vector2)transform.position + (isSelected ? selectedOffset : Vector2.zero);
        followRoot.position = Vector3.Lerp(followRoot.position, targetPosition, Time.deltaTime * 8f);
    }
    private void OnDestroy()
    {
        OnPointerExit(null);
        Destroy(followRoot.gameObject);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        isSelected = true;

        activeRequest = CardPlayHandler.Instance.RequestValidation(stack.Card);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        isSelected = false;

        if (activeRequest == null)
            return;

        CardPlayHandler.Instance.RemoveValidationRequest(activeRequest);
        activeRequest = null;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        CardPlayHandler.Instance.TryPlay(activeRequest);
    }
}

internal class CardPlayHandler : Singleton<CardPlayHandler>
{
    private List<CardValidationRequest> requests = new();
    private CardValidationContext context = new();

    protected override void OnCreate()
    {
        PlayerEventHandler.Instance.OnPlayerChangedCellEvent.AddListener(OnPlayerChangedCell);
        context.Map = MapHandler.Instance.MapData;
    }
    protected override void OnDestroy()
    {
        PlayerEventHandler.Instance.OnPlayerChangedCellEvent.RemoveListener(OnPlayerChangedCell);
    }
    private void OnPlayerChangedCell(Cell cell)
    {
        context.CurrentPlayerCell = cell;
        RefreshAll();
    }
    internal CardValidationRequest RequestValidation(Card card)
    {
        CardValidationRequest request = new() { Card = card, Context = context };

        requests.Add(request);
        request.Refresh();

        return request;
    }
    internal void RemoveValidationRequest(CardValidationRequest request)
    {
        request.End();
        requests.Remove(request);
    }
    private void RefreshAll()
    {
        foreach (var request in requests)
            request.Refresh();
    }

    internal void TryPlay(CardValidationRequest activeRequest)
    {
        if (!activeRequest.LastResult)
            return;

        if (activeRequest.Card.TryPlay(context))
            CardHandler.Instance.RemoveCard(activeRequest.Card.ID);   
    }

    internal void NotifyRefresh()
    {
        CoroutineHelper.Instance.OnNextFrame(() => RefreshAll());
    }
}

public class CardValidationContext
{
    public Cell CurrentPlayerCell;
    public MapData Map;
}
public class CardValidationRequest
{
    public Card Card;
    public CardValidationContext Context;
    public bool LastResult;

    internal void End()
    {
        Card.EndValidation(Context);
    }
    internal void Refresh()
    {
        LastResult = Card.RefreshValidation(Context);
    }
}