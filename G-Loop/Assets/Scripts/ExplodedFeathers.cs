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
        gman = GhoostlingManager.GetInstance();
        particleSystem.Stop();
    }

    public void Explode() {
        if (gman.IsFastForwarding() || gman.IsPaused()) {
            return;
        }
        audioSource.PlayOneShot(explode);
        particleSystem.Emit(FEATHER_COUNT);
        Debug.Log("Feathers.");
    }
}
