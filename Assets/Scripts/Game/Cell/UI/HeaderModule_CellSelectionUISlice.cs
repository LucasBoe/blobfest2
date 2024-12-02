using TMPro;
using UnityEngine;

[System.Serializable] 
public class HeaderModule_CellSelectionUISlice : CellSelectionUISliceModuleBase
{
    [SerializeField] TMP_Text cellNameText;
    protected override void TryPopulate()
    {
        PopulateName();

        void PopulateName()
        {
            var name = Cell.CellType.ToString();

            if (Cell.CurrentBehavior != null && (Cell.CurrentBehavior is ICustomDisplayNameProvider provider))
                name = provider.GetName();

            cellNameText.text = name.ToUpper();
        }
    }
}
public interface ICustomDisplayNameProvider
{
    string GetName();
}