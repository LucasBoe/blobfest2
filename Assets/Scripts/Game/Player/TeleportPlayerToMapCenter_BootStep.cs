using System.Collections;
using System.Collections.Generic;
using Engine;
using UnityEngine;

public class TeleportPlayerToMapCenter_BootStep : MonoBehaviour, ISceneChangeTask
{
    public IEnumerator Execute(SceneChangeContext _context)
    {
        var player = GameObject.FindAnyObjectByType<Player>();

        if (player != null)
            player.SetPosition(MapHandler.Instance.MapCenter);

        yield return null;
    }
}
