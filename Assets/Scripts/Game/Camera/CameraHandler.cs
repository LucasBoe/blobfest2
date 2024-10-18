using System.Collections;
using System.Collections.Generic;
using Engine;
using UnityEngine;

public class CameraHandler : MonoBehaviour, IDelayedStartObserver
{
    Player player;
    Vector2 velocity;
    float smoothTime = 1f;

    public void DelayedStart()
    {
        player = FindObjectOfType<Player>();

        if (player == null)
            return;

        ApplyPosition(player.transform.position);
    }
    private void Update()
    {
        if (player == null)
            return;

        var pos = Vector2.SmoothDamp(transform.position, player.Position, ref velocity, smoothTime);
        ApplyPosition(pos);
    }

    private void ApplyPosition(Vector2 pos)
    {
        transform.position = new Vector3(pos.x, pos.y, -10f);
    }
}
