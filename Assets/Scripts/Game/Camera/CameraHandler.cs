using System.Collections;
using System.Collections.Generic;
using Engine;
using UnityEngine;

public class CameraHandler : MonoBehaviour, IDelayedStartObserver
{
    Player player;
    Vector2 velocity;
    float smoothTime = 1f;

    [SerializeField] float zOffset = -10;
    [SerializeField] Transform audioListener;

    public void DelayedStart()
    {
        player = FindObjectOfType<Player>();

        if (player == null)
            return;

        ApplyPosition(player.transform.position);
        audioListener.transform.localPosition = new Vector3(0, 0, -zOffset);
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
        transform.position = new Vector3(pos.x, pos.y, zOffset);
    }
}
