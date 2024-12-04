using System.Collections;
using System.Collections.Generic;
using Engine;
using UnityEngine;

[ExecuteAlways]
public class StaticZOffset : MonoBehaviour
{
    bool zNeedsUpdate = true;
    // Update is called once per frame
    void Update()
    {
        if (!zNeedsUpdate)
            return;

        var pos = transform.position;
        transform.position = pos.With(z: ZOffsetUtil.OffsetFromY(pos.y));
        zNeedsUpdate = false;
    }
    public void SceduleZUpdate()
    {
        zNeedsUpdate = true;
    }
}
