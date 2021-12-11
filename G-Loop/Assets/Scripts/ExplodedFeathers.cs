using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodedFeathers : MonoBehaviour {
    new public ParticleSystem particleSystem;
    public AudioSource audioSource;
    public AudioClip explode;
    public GhoostlingManager gman;
    public const int FEATHER_COUNT = 400;
    void Start() {
        particleSystem.Stop();
    }

    public void Explode() {
        if (gman.IsFastForwarding()) {
            return;
        }
        audioSource.PlayOneShot(explode);
        particleSystem.Emit(FEATHER_COUNT);
    }
}
