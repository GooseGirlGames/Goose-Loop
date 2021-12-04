using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhoostlingPlayer : MonoBehaviour {
    private GhoostlingData data = null;
    private bool active = false;
    private float timeAlive = 0;
    // Start is called before the first frame update
    void Start() {
        // hide model, disable interactions, etc.
    }

    void Update() {
        if (data == null) {
            return;
        } else if (!active) {
            InitializeGhoostling();
        }

        PlayGhoostlingFrame();
    }

    public void SetData(GhoostlingData d) {
        data = d;
    }

    // called once, as soon as data is available
    private void InitializeGhoostling() {
        // show model, enable interactions etc.
    }
    private void PlayGhoostlingFrame() {
        timeAlive += Time.deltaTime;

        GhoostlingData.Frame h = data.GetFrame(timeAlive);

        //Debug.Log("Time Alive: " + timeAlive + "; playing Frame at " + h.time);

        transform.position = h.position;
        transform.eulerAngles = h.eulerAngles;
    }
}
