using System.Collections;
using System.Collections.Generic;
using Engine;
using UnityEngine;

[ExecuteAlways]
public class DynamicZOffset : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        var pos = transform.position;
        transform.position = pos.With(z: ZOffsetUtil.OffsetFromY(pos.y));   
    }
}
