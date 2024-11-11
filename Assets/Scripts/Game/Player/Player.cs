using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Engine;
using UnityEngine;

public class Player : CreatureBase, IPositionProvider, INPCPositionProvider
{
    public PlayerCellDetector Cells;

    public Vector2 RequestPosition(NPCBehaviour npc)
    {
        int positionInPlayerNPCList = NPCHandler.Instance.GetPlayerNPCIndex(npc);
        return Vector2.MoveTowards(Position, npc.Position, .4f + 0.2f * positionInPlayerNPCList);
    }
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
        var neightbours = Current.Neightbours;
        foreach (var item in neightbours)
        {
            FogOfWar.Instance.SetCellVisibility(item, true);
        }

        PlayerEventHandler.Instance.OnPlayerChangedCellEvent?.Invoke(Current);
    }
}

public class PlayerEventHandler : Singleton<PlayerEventHandler>
{
    public Event<Cell> OnPlayerChangedCellEvent = new();
}
