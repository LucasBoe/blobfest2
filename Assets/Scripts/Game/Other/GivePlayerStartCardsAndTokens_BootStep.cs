using UnityEngine;
using Engine;
using System.Collections;

public class GivePlayerStartCardsAndTokens_BootStep : MonoBehaviour, ISceneChangeTask
{
    public Token[] StarterTokens;
    public Card[] StarterCards;

    public IEnumerator Execute(SceneChangeContext _context)
    {
        var pos = FindObjectOfType<Player>().transform.position;

        foreach (var token in StarterTokens)
            TokenHandler.Instance.AddTokenAnimated(token.ID, pos);

        foreach (var card in StarterCards)
            CardHandler.Instance.AddCardAnimated(card.ID, pos);

        yield break;
    }
}

