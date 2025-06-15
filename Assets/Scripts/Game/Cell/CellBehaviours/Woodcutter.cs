using UnityEngine;
using System.Collections;

public class Woodcutter : CellBehaviour
{
    Transform mill;
    public static new CellType AssociatedCellType => CellType.Woodcutter;

    public override void Enter()
    {
        var prefab = PrefabRefID.Woodcutter.TryGetPrefab<Transform>();
        mill = Instantiate(prefab, Context.Cell.Center);
    }
    public override void Exit()
    {
        Object.Destroy(mill.gameObject);
    }
}