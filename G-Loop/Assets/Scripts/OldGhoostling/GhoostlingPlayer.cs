using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhoostlingPlayer : MonoBehaviour {
    private GhoostlingData data = null;
    private bool active = false;
    private float timeAlive = 0;
    private SkinnedMeshRenderer mesh;
    private Vector3 initialScale;
    public const float FADE_IN_TIME = 4.0f;
    public mouse_look cam;

    public GhoostlingActionManager actionMan;
    // Start is called before the first frame update
    void Awake() {
        mesh = GetComponentInChildren<SkinnedMeshRenderer>();
        initialScale = transform.localScale;
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
    public void Restart() {
        timeAlive = 0;
    }

    // called once, as soon as data is available
    private void InitializeGhoostling() {
        // show model, enable interactions etc.
    }
    private void PlayGhoostlingFrame() {
        timeAlive += Time.deltaTime;

        if (timeAlive <= FADE_IN_TIME) {
            transform.localScale = initialScale * (timeAlive / FADE_IN_TIME);
        }

        GhoostlingData.Frame h = data.GetFrame(timeAlive);

        //Debug.Log("Time Alive: " + timeAlive + "; playing Frame at " + h.time);

        transform.position = h.position;
        transform.eulerAngles = h.eulerAngles;
        cam.xRotation = h.cameraPitch;

        if (h.actions != null) {
            foreach (GhoostlingAction action in h.actions) {
                action.Trigger(actionMan);
            }
        }
    }
}
