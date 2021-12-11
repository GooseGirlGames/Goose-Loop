using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodedFeathers : MonoBehaviour {
    new public ParticleSystem particleSystem;
    public const int FEATHER_COUNT = 400;
    void Start() {
        particleSystem.Stop();
    }

    void Explode() {
        particleSystem.Emit(FEATHER_COUNT);
    }
}
