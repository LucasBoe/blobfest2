using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehaviour : MonoBehaviour, IPositionProvider
{
    private INPCPositionProvider positionProvider;

    public const float SPEED = 3f;
    public Vector2 Position => transform.position;

    internal void Link(INPCPositionProvider positionProvider)
    {
        this.positionProvider = positionProvider;
    }

    // Update is called once per frame
    void Update()
    {
        if (positionProvider == null)
            return;

        transform.position = Vector3.MoveTowards(transform.position, positionProvider.RequestPosition(this), Time.deltaTime * SPEED);
    }
}
