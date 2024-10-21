using System;
using System.Collections;
using System.Collections.Generic;
using Engine;
using UnityEngine;

public static class Collider2DExtensions
{
    // Extension method to check if the Collider2D belongs to a player
    public static bool IsPlayer(this Collider2D collider)
    {
        return collider.CompareTag("Player");
    }
}
public class Collectible : MonoBehaviour
{
    [SerializeField] private SpriteRenderer iconRenderer;
    [SerializeField] private Card card;
    [SerializeField] private Token token;
    [SerializeField] private int amount = 1;

    internal void Init(ContaineableScriptableObject _object)
    {
        if (_object is not ICollectibleIconProvider iconProvider)
        {
            Destroy(gameObject);
            return;
        }

        iconRenderer.sprite = iconProvider.GetIcon();

        if (_object is Token t)
            token = t;

        if (_object is Card c)
            card = c;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.IsPlayer())
        {
            if (card != null)
                CardHandler.Instance.AddCardAnimated(card.ID, amount, transform.position);

            if (token != null)
                TokenHandler.Instance.AddTokenAnimated(token.ID, amount, transform.position);

            Destroy(gameObject);
        }
    }
}

