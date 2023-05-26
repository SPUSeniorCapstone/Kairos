using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public float time;
    public float coolDown = 10;
    public void PlayOnce(AudioSource source, AudioClip clip)
    {
        if (Time.time - time > coolDown)
        {
            source.PlayOneShot(clip);
            time = Time.time;
        }
    }
}
