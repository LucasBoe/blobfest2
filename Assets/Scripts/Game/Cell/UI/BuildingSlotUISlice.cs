using UnityEngine;

[System.Serializable]
public class BuildingSlotUISlice : MonoBehaviour
{
    [SerializeField] private GameObject blockedRoot, emptyRoot, fullRoot;
    public void SetState(BuildingSlotUISlice.SlotState state)
    {
        blockedRoot.SetActive(state == SlotState.Blocked);
        emptyRoot.SetActive(state == SlotState.Empty);
        fullRoot.SetActive(state == SlotState.Full);
    }
    public enum SlotState
    {
        Blocked,
        Empty,
        Full,
    }
}