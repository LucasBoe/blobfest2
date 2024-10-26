using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Engine;
using UnityEngine;

public class Player : CreatureBase
{
    public PlayerCellDetector Cells;
    private void Awake()
    {
        Cells.Init(this);
    }

    private void Update()
    {
        var xInput = Input.GetAxis("Horizontal");
        var yInput = Input.GetAxis("Vertical");

        if (Mathf.Abs(xInput) <= 0 && Mathf.Abs(yInput) <= 0)
            return;

        Movement.SetInput(new Vector2(xInput, yInput));

        Cells.OnUpdate();
    }

}

[System.Serializable]
public class PlayerCellDetector
{
    public Cell Current;
    private Player player;

    internal void Init(Player player)
    {
        this.player = player;
    }

    internal void OnUpdate()
    {
        var closest = MapDataUtil.GetCellThatContainsPoint(MapHandler.Instance.MapData, player.Position);

        if (Current == closest)
            return;

        Current = closest;
        PlayerEventHandler.Instance.OnPlayerChangedCellEvent?.Invoke(Current);
    }
}

public class PlayerEventHandler : Singleton<PlayerEventHandler>
{
    public Event<Cell> OnPlayerChangedCellEvent = new();
}
