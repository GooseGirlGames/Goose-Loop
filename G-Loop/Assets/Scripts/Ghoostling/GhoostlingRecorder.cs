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
    public int GetDataId() {
        return data.id;
    }
    private void SpawnGhoostling() {
        GameObject g = GameObject.Instantiate(playerPrefab, ghoostlingHolder);
        g.transform.position = ghoostlingHolder.position;
        g.transform.eulerAngles = ghoostlingHolder.eulerAngles;

        g.name = "Ghoostling #" + g.GetComponentInChildren<GhoostlingRecorder>().GetDataId();
        g.GetComponentInChildren<GhoostlingRecorder>().ghoostlingHolder = ghoostlingHolder;

        gameObject.name += " (Recording)";

        GetComponent<GhoostlingRecorder>().enabled = false;
        GetComponentInChildren<Camera>().enabled = false;
        GetComponentInChildren<AudioListener>().enabled = false;
        GetComponent<CharacterController>().enabled = false;
        GetComponentInChildren<mouse_look>().enabled = false;
        GetComponent<movement>().enabled = false;
        GetComponentInChildren<SkinnedMeshRenderer>().gameObject.layer = LayerMask.NameToLayer("Default");

        foreach (GhoostlingPlayer p in ghoostlingHolder.GetComponentsInChildren<GhoostlingPlayer>()) {
            p.Restart();
        }
        GetComponent<GhoostlingPlayer>().enabled = true;
        GetComponent<GhoostlingPlayer>().SetData(data);
    }
}
