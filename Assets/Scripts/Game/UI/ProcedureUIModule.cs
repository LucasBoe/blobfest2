using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProcedureUIModule : DataUIModule<IProcedureProvider>
{
    [SerializeField] private Slider slider;
    public override void Show()
    {
        gameObject.SetActive(Data != null && Data.Procedure.IsRunning);
    }
    public override void Hide()
    {
        gameObject.SetActive(false);
    }
    private void Update()
    {
        if (Data== null)
            return;
        
        slider.value = Data.Procedure.Progression;
    }
}
