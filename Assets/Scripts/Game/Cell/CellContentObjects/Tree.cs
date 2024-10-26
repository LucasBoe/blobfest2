using System.Collections;
using System.Collections.Generic;
using Engine;
using UnityEngine;

public class Tree : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Sprite[] treeSprites;
    private void Awake()
    {
        spriteRenderer.sprite = treeSprites.GetRandom();
    }
}
