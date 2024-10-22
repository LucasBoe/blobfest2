using Engine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressbarManager : SingletonBehaviour<ProgressbarManager>
{
    [SerializeField] ProgressBarUISlice prefab;
}

public class ProgressBarUISlice : MonoBehaviour
{
    public bool FollowTransform = false;
    public Transform TransformToFollow;
    public Vector2 TargetOffset = new Vector2(0, .25f);
    public SpriteRenderer BarRenderer, IconRenderer;

    private Material materialInstance;    
}