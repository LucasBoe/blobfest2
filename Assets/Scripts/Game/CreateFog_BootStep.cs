using UnityEngine;
using Engine;
using System.Collections;
using System.Linq;

public class CreateFog_BootStep : MonoBehaviour, ISceneChangeTask
{
    public IEnumerator Execute(SceneChangeContext _context)
    {
        FogOfWar.Instance.GenerateFogMesh(MapHandler.Instance.MapData.Cells.ToList());
        yield break;
    }
}

