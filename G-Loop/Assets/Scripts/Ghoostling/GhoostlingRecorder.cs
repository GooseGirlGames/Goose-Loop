using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhoostlingRecorder : MonoBehaviour {
    public GameObject playerPrefab;
    public Transform ghoostlingHolder;
    public GhoostlingActionManager actionMan;
    private float timeAlive = 0;
    private float timeSinceLastCapture = 0;
    private GhoostlingData data = new GhoostlingData();
    private bool recording = true;
    private List<GhoostlingAction> actions = new List<GhoostlingAction>();
    void Awake() {
        
    }

    public void ExecuteAndRecordAction(GhoostlingAction a) {
        a.Trigger(actionMan);
        actions.Add(a);
    }

    void Update() {
        if (!recording) return;

        timeAlive += Time.deltaTime;
        timeSinceLastCapture += Time.deltaTime;

        if (timeSinceLastCapture >= 1f/GhoostlingData.RATE) {
            data.AddFrame(CurrentFrame());
            timeSinceLastCapture = 0;
        }

        if (Input.GetKeyDown(KeyCode.G)) {
            StopRecording();
            SpawnGhoostling();
        }

    }

    private GhoostlingData.Frame CurrentFrame() {
        GhoostlingData.Frame f = new GhoostlingData.Frame();
        f.time = timeAlive;
        f.position = transform.position;
        f.eulerAngles = transform.eulerAngles;
        if (actions.Count > 0) {
            f.actions = new List<GhoostlingAction>();
            f.actions.AddRange(actions);
            actions.Clear();
        }
        return f;
    }

    private void StopRecording() {
        recording = false;
    }
    public bool IsRecording() {
        return recording;
    }
    private void SpawnGhoostling() {
        GameObject g = GameObject.Instantiate(playerPrefab, ghoostlingHolder);
        g.GetComponent<GhoostlingRecorder>().enabled = false;
        g.GetComponentInChildren<Camera>().enabled = false;
        g.GetComponentInChildren<AudioListener>().enabled = false;
        g.GetComponent<CharacterController>().enabled = false;
        g.GetComponentInChildren<mouse_look>().enabled = false;
        g.GetComponent<movement>().enabled = false;
        g.GetComponentInChildren<MeshRenderer>().gameObject.layer = LayerMask.NameToLayer("Default");

        g.GetComponent<GhoostlingPlayer>().enabled = true;
        g.GetComponent<GhoostlingPlayer>().SetData(data);
    }
}
