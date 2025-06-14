﻿using UnityEngine;
using System.Collections;

public class Mill : CellBehaviour
{
    Transform mill;
    public static new CellType AssociatedCellType => CellType.Mill;

    public override void Enter()
    {
        Deals = new Deal[] { new Deal(CardID.Farmer, TokenID.Wood, 4)};

        var prefab = PrefabRefID.Mill.TryGetPrefab<Transform>();
        mill = Instantiate(prefab, Context.Cell.Center);
    }
    public override void Exit()
    {
        Object.Destroy(mill.gameObject);
    }
}