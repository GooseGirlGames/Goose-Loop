using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodedFeathers : MonoBehaviour {
    new public ParticleSystem particleSystem;
    public AudioSource audioSource;
    public AudioClip explode;
    public const int FEATHER_COUNT = 400;
    void Start() {
        particleSystem.Stop();
    }

    public void Explode() {
        audioSource.PlayOneShot(explode);
        particleSystem.Emit(FEATHER_COUNT);
    }
}
