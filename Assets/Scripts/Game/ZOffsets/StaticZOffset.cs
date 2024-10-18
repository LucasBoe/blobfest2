using System.Collections;
using System.Collections.Generic;
using Engine;
using UnityEngine;

[ExecuteAlways]
public class StaticZOffset : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (Application.isPlaying)
            Destroy(this);
    }

    // Update is called once per frame
    void Update()
    {
        var pos = transform.position;
        transform.position = pos.With(z: ZOffsetUtil.OffsetFromY(pos.y));   
    }
}
