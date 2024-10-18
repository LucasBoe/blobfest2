using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverSprite : MonoBehaviour
{
    [SerializeField] float yHoverStrength = 1f;
    [SerializeField] float hoverSpeed = 1f;

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = new Vector2(0, Mathf.Sin(transform.position.x + Time.time * Mathf.PI * hoverSpeed) * yHoverStrength);
    }
}
