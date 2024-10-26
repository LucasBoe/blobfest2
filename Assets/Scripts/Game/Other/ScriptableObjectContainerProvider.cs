using Engine;
using UnityEngine;

public class ScriptableObjectContainerProvider : SingletonBehaviour<ScriptableObjectContainerProvider>
{
    [SerializeField] public CellContentPrefabRefContainer CellContents;
    [SerializeField] public CardContainer Cards;
    [SerializeField] public TokenContainer Tokens;
}