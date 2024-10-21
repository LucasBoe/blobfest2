using System.Collections.Generic;
using DG.Tweening;
using Simple.SoundSystem.Core;
using UnityEngine;

public class CardUIManager : MonoBehaviour
{
    [SerializeField] CardUISlice cardUIPrefab; // Reference to a prefab for UI slices
    [SerializeField] Transform cardRow;
    [SerializeField] Transform lastCard;
    [SerializeField] Sound collectCardSound;
    private Dictionary<CardStack, CardUISlice> stackToUISlice = new(); // Map stacks to their UI elements

    private void OnEnable()
    {
        CardHandler.Instance.OnNewStackCreatedEvent.AddListener(OnNewStack);
        CardHandler.Instance.OnStackUpdatedEvent.AddListener(OnStackUpdated);
        CardHandler.Instance.OnOldStackDeletedEvent.AddListener(OnStackDeleted);
        CardHandler.Instance.OnCardStackAnimateEvent.AddListener(SpawnFloatingCard);
    }

    // Called when a new card stack is created
    private void OnNewStack(CardStack stack)
    {
        CardUISlice newSlice = Instantiate(cardUIPrefab, cardRow);
        newSlice.Initialize(stack); // Assign and display the stack in the slice
        stackToUISlice[stack] = newSlice; // Map the stack to the UI slice
        lastCard.SetAsLastSibling();
    }

    // Called when an existing stack is updated (e.g., cards added or removed)
    private void OnStackUpdated(CardStack stack)
    {
        if (stackToUISlice.TryGetValue(stack, out CardUISlice slice))
        {
            slice.Refresh(); // Refresh the UI slice to reflect the changes
        }
    }

    // Called when a stack is deleted (e.g., when amount reaches zero)
    private void OnStackDeleted(CardStack stack)
    {
        if (stackToUISlice.TryGetValue(stack, out CardUISlice slice))
        {
            Destroy(slice.gameObject); // Remove the UI slice from the scene
            stackToUISlice.Remove(stack); // Remove it from the dictionary
        }
    }
    // Method to spawn a floating card that lerps to its final position in the UI
    public void SpawnFloatingCard(CardStack stack, FloatingCardParameters parameters)
    {
        Vector2 startPosition = Camera.main.WorldToScreenPoint(parameters.WorldOrigin);
        collectCardSound.PlayAt(parameters.WorldOrigin);

        CardUISlice newSlice = Instantiate(cardUIPrefab, startPosition, Quaternion.identity, transform);
        newSlice.Initialize(stack, tween: false); // Assign and display the stack in the slice
        Vector2 targetUIPosition = GetTargetUIPosition();

        newSlice.transform.DOMove(targetUIPosition, parameters.AnimationDuration, true).SetEase(Ease.InOutSine).onComplete = () =>
        {
            Destroy(newSlice.gameObject);
        };

        Vector2 GetTargetUIPosition()
        {
            if (parameters.TargetStack != null)
                if (stackToUISlice.ContainsKey(parameters.TargetStack))
                    return stackToUISlice[parameters.TargetStack].transform.position;
            

            return lastCard.position;
        }
    }
}

public class FloatingCardParameters
{
    public Vector2 WorldOrigin;
    public CardStack TargetStack;
    public float AnimationDuration;
}
