using System.Collections;
using Engine;
using UnityEngine;

public class GenerateMap_BootStep : MonoBehaviour, ISceneChangeTask
{
    public IEnumerator Execute(SceneChangeContext _context)
    {
        if (_context.ToPlayScene)
        {
            MapHandler.Instance.GenerateNewMap();
        }

        yield return null;
    }
}
