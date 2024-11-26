using System;
using Engine;
using NaughtyAttributes;
using UnityEngine;

public class Stone : MonoBehaviour
{
    [SerializeField] SpriteRenderer stoneRenderer;
    [SerializeField] Sprite[] stoneSprites;

    private void Awake()
    {
        stoneRenderer.sprite = stoneSprites[transform.GetSiblingIndex() % stoneSprites.Length];
    }
}