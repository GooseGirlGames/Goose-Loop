using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhoostlingRecorder : MonoBehaviour {
    private float timeAlive = 0;
    private float timeSinceLastCapture = 0;
    private GhoostlingData data;
    void Awake() {
        
    }

    void Update() {
        timeAlive += Time.deltaTime;
        timeSinceLastCapture += Time.deltaTime;

        if (timeSinceLastCapture >= 1f/GhoostlingData.RATE) {
            data.frames.Add(CurrentFrame());
        }
    }

    private GhoostlingData.Frame CurrentFrame() {
        GhoostlingData.Frame f = new GhoostlingData.Frame();
        f.time = timeAlive;
        f.position = transform.position;
        f.eulerAngles = transform.eulerAngles;
        return f;
    }
}
