using System.Collections.Generic;
using UnityEngine;

public class DealUIManager : MonoBehaviour
{
    [SerializeField] private DealUIBox dealUIBoxDummy;       // Dummy DealUIBox (disabled by default)
    [SerializeField] private Transform dealUIContainer;      // Container for UI elements

    private DealUIBox activeDealBox; // Current active DealUIBox

    private void Awake()
    {
        // Ensure dummy objects are disabled at start
        dealUIBoxDummy.gameObject.SetActive(false);
    }

    private void OnEnable() => PlayerEventHandler.Instance.OnPlayerChangedCellEvent += OnPlayerChangedCell;
    private void OnDisable() => PlayerEventHandler.Instance.OnPlayerChangedCellEvent -= OnPlayerChangedCell;

    private void OnPlayerChangedCell(Cell newCell)
    {
        // Clear any existing DealUIBox before setting up a new one
        ClearOldDealBox();

        // Return early if the new cell's behavior does not implement IDealProvider
        if (newCell.CurrentBehavior is not IDealProvider dealProvider)
            return;

        // Return early if there are no deals to display
        if (dealProvider.Deals is null || dealProvider.Deals.Length == 0)
            return;

        // Instantiate and set up the DealUIBox, as we now know there are deals available
        activeDealBox = Instantiate(dealUIBoxDummy, dealUIContainer);
        activeDealBox.gameObject.SetActive(true);

        // Initialize the DealUIBox with the list of deals and the cell’s world position
        activeDealBox.Initialize(dealProvider.Deals, newCell.Center);
    }
    private void ClearOldDealBox()
    {
        if (activeDealBox != null)
        {
            activeDealBox.ClearSlices();
            Destroy(activeDealBox.gameObject);
            activeDealBox = null;
        }
    }
}