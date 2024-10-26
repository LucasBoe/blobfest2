using System.Collections.Generic;
using UnityEngine;

public class DealUIBox : MonoBehaviour
{
    [SerializeField] private DealUISlice dealUISliceDummy;   // Dummy DealUISlice (disabled by default)
    [SerializeField] private Transform dealSliceContainer;  // Container to hold DealUISlices

    private List<DealUISlice> dealSlices = new List<DealUISlice>(); // Track DealUISlice instances in this box
    private Vector2 offset = new Vector2(0, 1);

    private void Awake()
    {
        dealUISliceDummy.gameObject.SetActive(false);
    }
    // Initialize the UI box with deals and set its position relative to the cell
    public void Initialize(Deal[] deals, Vector3 cellWorldPosition)
    {
        SetPositionAboveCell(cellWorldPosition);

        // Create and set up a DealUISlice for each deal
        foreach (var deal in deals)
        {
            AddDealSlice(deal, dealUISliceDummy);
        }
    }
    // Creates a DealUISlice by cloning the dummy and adding it to the container
    private void AddDealSlice(Deal deal, DealUISlice dealSliceDummy)
    {
        // Clone the dummy, set it active, and parent it under the container
        var dealUISliceObject = Instantiate(dealSliceDummy, dealSliceContainer);
        dealUISliceObject.gameObject.SetActive(true);

        // Initialize the cloned DealUISlice with deal data
        dealUISliceObject.Initialize(deal);

        // Track the DealUISlice in this box
        dealSlices.Add(dealUISliceObject);
    }
    // Sets the position of the UI box relative to the specified cell
    private void SetPositionAboveCell(Vector2 cellWorldPosition)
    {
        transform.position = cellWorldPosition + offset;
    }
    // Clears all DealUISlices from the UI box
    public void ClearSlices()
    {
        foreach (var slice in dealSlices)
        {
            Destroy(slice.gameObject);
        }
        dealSlices.Clear();
    }
}
