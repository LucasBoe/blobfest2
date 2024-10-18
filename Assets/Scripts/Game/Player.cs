using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : CreatureBase
{
    private void Update()
    {
        var xInput = Input.GetAxis("Horizontal");
        var yInput = Input.GetAxis("Vertical");

        if (Mathf.Abs(xInput) <= 0 && Mathf.Abs(yInput) <= 0)
            return;

        Movement.SetInput(new Vector2(xInput, yInput));
    }
}
