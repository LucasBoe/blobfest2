using System.Collections.Generic;
using UnityEngine;

public class Stonefield : CellBehaviour, ICanReceive<Builder>
{
    public static new CellType AssociatedCellType => CellType.Stonefield;
    List<Stone> stones = new();
    public override void Enter()
    {
        stones = SpawnStones();
    }    
    public override void Exit()
    {
        //ClearStones();
    }
    private List<Stone> SpawnStones()
    {
        List < Stone > trees = new();
        var prefab = PrefabRefID.Stone.TryGetPrefab<Stone>();

        foreach (var poi in Context.Cell.GetPOIS(7)) 
            Instantiate(prefab, poi, trees);

        return trees;
    }
    private void ClearStones()
    {
        foreach (var stone in stones)
            GameObject.Destroy(stone.gameObject);
        stones.Clear();
    }

    public bool CanReceiveCard(Builder card)
    {
        return true;
    }
    public void DoReceiveCard(Builder card)
    {
        Context.Cell.ChangeCellType(CellType.ConstructionSite);
    }
}