using System.Collections;
using System.Collections.Generic;
using Engine;
using UnityEngine;

[ExecuteAlways]
public class StaticZOffset : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Application.isPlaying)
            Destroy(this);

        var pos = transform.position;
        transform.position = pos.With(z: ZOffsetUtil.OffsetFromY(pos.y));   
    }
}
