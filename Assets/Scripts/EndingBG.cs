using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingBG : MonoBehaviour
{
    public AudioSource AudioSource;

    public AudioClip SigilBlip;
    public AudioClip ExplosionStart;
    public AudioClip ExplosionFull;
    public AudioClip EndingAmbiance;

    public void PlayAudioClip(AudioClip clip)
    {
        AudioSource.clip = clip;
        AudioSource.Play();
    }
}
