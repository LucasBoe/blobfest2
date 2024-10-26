using Simple.SoundSystem.Core;
using UnityEngine;

public class PlayerMoveSoundPlayer : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] Sound playerMoveLoop;

    private bool isMoving = false;

    private void Update()
    {
        bool wasMoving = isMoving;
        isMoving = player.Velocity.magnitude > .25f;

        if (isMoving == wasMoving)
            return;

        if (isMoving)
            OnBeginMove();
        else
            OnEndMove();
    }
    private void OnBeginMove()
    {
        playerMoveLoop.Play(loop: true);
    }
    private void OnEndMove()
    {
        playerMoveLoop.Stop();
    }
}