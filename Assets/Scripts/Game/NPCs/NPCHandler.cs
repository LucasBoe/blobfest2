using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Engine;
using UnityEngine;

[SingletonSettings(SingletonLifetime.Scene, _canBeGenerated: false, _eager: true)]
public class NPCHandler : SingletonBehaviour<NPCHandler>
{
    [SerializeField] NPCBehaviour npcDummy;

    private List<NPCBehaviour> playerNPCs = new();
    private List<NPCBehaviour> freeNPC = new();

    private void OnEnable()
    {
        CardHandler.Instance.OnAddCardEvent.AddListener(OnAddCardToPlayerCards);
        CardHandler.Instance.OnRemoveCardEvent.AddListener(OnRemoveCardFromPlayerCards);
    }
    private void OnDisable()
    {
        CardHandler.Instance.OnAddCardEvent.RemoveListener(OnAddCardToPlayerCards);
        CardHandler.Instance.OnRemoveCardEvent.RemoveListener(OnRemoveCardFromPlayerCards);
    }
    private void OnAddCardToPlayerCards(Card card)
    {
        if (!card.HasNPC)
            return;

        AddNewPlayerNPCs();
    }
    private void OnRemoveCardFromPlayerCards(Card card)
    {
        if (!card.HasNPC)
            return;

        FreePlayerNPC();
    }
    private void AddNewPlayerNPCs()
    {
        var player = FindObjectOfType<Player>();
        var npc = Instantiate(npcDummy, player.Position, Quaternion.identity, null);
        npc.Link(player);
        playerNPCs.Add(npc);

        Debug.Log("Added new NPC");
    }
    private void FreePlayerNPC()
    {
        var npc = playerNPCs.Last();
        playerNPCs.Remove(npc);
        freeNPC.Add(npc);

        Debug.Log("Freed NPC");
    }
    internal int GetPlayerNPCIndex(NPCBehaviour npc)
    {
        if (playerNPCs.Contains(npc))
            return playerNPCs.IndexOf(npc);

        return 0;
    }

    internal bool TryRequestFreedNPC(out NPCBehaviour npc)
    {
        if (freeNPC.Any())
        {
            npc = freeNPC.Last();
            Debug.Log("Requested free NPC succesfully");
            return true;
        }

        Debug.LogWarning("Failed requesting free NPC");
        npc = null;
        return false;
    }
}
public interface INPCPositionProvider
{
    public Vector2 RequestPosition(NPCBehaviour npc);
}