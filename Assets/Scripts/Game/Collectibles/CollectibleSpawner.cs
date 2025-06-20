using System;
using System.Collections;
using System.Collections.Generic;
using Engine;
using UnityEngine;

public class CollectibleSpawner : SingletonBehaviour<CollectibleSpawner>
{
    [SerializeField] Collectible prefab;

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1))
            SpawnAtCursor(CardID.Villager.ToCard());

        if (Input.GetKeyUp(KeyCode.Alpha2))
            SpawnAtCursor(CardID.Farmer.ToCard());

        if (Input.GetKeyUp(KeyCode.Alpha3))
            SpawnAtCursor(CardID.Settler.ToCard());

        if (Input.GetKeyUp(KeyCode.H))
            SpawnAtCursor(TokenID.Wood.ToToken());
    }

    private void SpawnAtCursor(ContaineableScriptableObject _object)
    {
        var pos = GetCurrentCursorWorldPosition();
        SpawnAt(_object, pos);
    }

    public void SpawnAt(ContaineableScriptableObject _object, Vector2 pos)
    {
        var instance = Instantiate(prefab, pos, Quaternion.identity);
        instance.Init(_object);
    }

    private Vector2 GetCurrentCursorWorldPosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

}
