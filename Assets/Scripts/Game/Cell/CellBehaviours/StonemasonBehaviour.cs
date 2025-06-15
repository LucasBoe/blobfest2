using UnityEngine;
using System.Collections;

public class StonemasonBehaviour : CellBehaviour
{
    Transform hut;
    public static new CellType AssociatedCellType => CellType.StonemasonHut;

    public override void Enter()
    {
        var prefab = PrefabRefID.StonemasonHut.TryGetPrefab<Transform>();
        hut = Instantiate(prefab, Context.Cell.Center);
    }
    public override void Exit()
    {
        Object.Destroy(hut.gameObject);
    }
}