using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class DestroySelf : MonoBehaviour
{

    [SerializeField] private bool destroyTimed = true;

    [ShowIf("destroyTimed")][SerializeField] private float destroyTime = 3f;

    // Start is called before the first frame update
    void Start()
    {
        if (destroyTimed)
        {

            Destroy(gameObject, destroyTime);
        }

        else 
            Destroy(gameObject);
    }
}
