using System.Collections.Generic;
using DG.Tweening;
using Simple.SoundSystem.Core;
using UnityEngine;

public class TokenUIManager : MonoBehaviour
{
    [SerializeField] TokenUISlice TokenUISlice; // Reference to a prefab for UI slices
    [SerializeField] Transform TokenRow;
    [SerializeField] Transform lastToken;
    [SerializeField] Sound collectTokenSound; 
    private Dictionary<TokenStack, TokenUISlice> stackToUISlice = new(); // Map stacks to their UI elements

    private void OnEnable()
    {
        TokenHandler.Instance.OnNewStackCreatedEvent.AddListener(OnNewStack);
        TokenHandler.Instance.OnStackUpdatedEvent.AddListener(OnStackUpdated);
        TokenHandler.Instance.OnOldStackDeletedEvent.AddListener(OnStackDeleted);
        TokenHandler.Instance.OnTokenStackAnimateEvent.AddListener(SpawnFloatingToken);
    }
    private void OnDisable()
    {
        if (!TokenHandler.InstanceExists)
            return;

        TokenHandler.Instance.OnNewStackCreatedEvent.RemoveListener(OnNewStack);
        TokenHandler.Instance.OnStackUpdatedEvent.RemoveListener(OnStackUpdated);
        TokenHandler.Instance.OnOldStackDeletedEvent.RemoveListener(OnStackDeleted);
        TokenHandler.Instance.OnTokenStackAnimateEvent.RemoveListener(SpawnFloatingToken);
    }
    // Called when a new Token stack is created
    private void OnNewStack(TokenStack stack)
    {
        TokenUISlice newSlice = Instantiate(TokenUISlice, TokenRow);
        newSlice.Initialize(stack); // Assign and display the stack in the slice
        stackToUISlice[stack] = newSlice; // Map the stack to the UI slice
        lastToken.SetAsLastSibling();
    }

    // Called when an existing stack is updated (e.g., Tokens added or removed)
    private void OnStackUpdated(TokenStack stack)
    {
        if (stackToUISlice.TryGetValue(stack, out TokenUISlice slice))
        {
            slice.Refresh(); // Refresh the UI slice to reflect the changes
        }
    }

    // Called when a stack is deleted (e.g., when amount reaches zero)
    private void OnStackDeleted(TokenStack stack)
    {
        if (stackToUISlice.TryGetValue(stack, out TokenUISlice slice))
        {
            Destroy(slice.gameObject); // Remove the UI slice from the scene
            stackToUISlice.Remove(stack); // Remove it from the dictionary
        }
    }
    // Method to spawn a floating Token that lerps to its final position in the UI
    public void SpawnFloatingToken(TokenStack stack, FloatingTokenParameters parameters)
    {
        if (TokenUISlice == null)
            return;

        Vector2 startPosition = Camera.main.WorldToScreenPoint(parameters.WorldOrigin);
        collectTokenSound.PlayAt(parameters.WorldOrigin);

        TokenUISlice newSlice = Instantiate(TokenUISlice, startPosition, Quaternion.identity, transform);
        newSlice.Initialize(stack); // Assign and display the stack in the slice
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
            

            return lastToken.position;
        }
    }
}

public class FloatingTokenParameters
{
    public Vector2 WorldOrigin;
    public TokenStack TargetStack;
    public float AnimationDuration;
}
