using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProcedureUIModule : DataUIModule<IProcedureProvider>
{
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text effiencyText;
    public override void Show()
    {
        gameObject.SetActive(Data != null && Data.Procedure.IsRunning);

        if (Data != null && Data.Procedure is DynamicTimeProcecure dynamic)
        {
            dynamic.ProgressProvider.OnEfficiencyChangedEvent.AddListener(OnEfficiencyChanged);
            OnEfficiencyChanged();
        }
    }
    public override void Hide()
    {
        gameObject.SetActive(false);
        
        if (Data != null && Data.Procedure is DynamicTimeProcecure dynamic)
        {
            dynamic.ProgressProvider.OnEfficiencyChangedEvent.RemoveListener(OnEfficiencyChanged);
        }
    }
    private void OnEfficiencyChanged()
    {
        if (Data != null && Data.Procedure is DynamicTimeProcecure dynamic)
        {
            effiencyText.text = dynamic.ProgressProvider.ProgressText;
        }
    }
    private void Update()
    {
        if (Data== null)
            return;
        
        slider.value = Data.Procedure.Progression;
        
    }
}
