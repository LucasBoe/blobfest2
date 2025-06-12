using Simple.SoundSystem.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlaySoundOnAwake : MonoBehaviour
{
    [SerializeField] Sound sound;

    // Start is called before the first frame update
    void Awake()
    {
        sound?.Play();
    }

}
