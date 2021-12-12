using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VHSEffectToggle : MonoBehaviour {
    public MonoBehaviour effect;
    private GhoostlingManager gman;
    private void Start() {
        gman = GhoostlingManager.GetInstance();
    }
    void Update() {
        effect.enabled = gman.IsFastForwarding() || gman.IsPaused();
    }
}
