using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GhoostlingRecorder : MonoBehaviour {
    public GameObject playerPrefab;
    public Transform ghoostlingHolder;
    public GhoostlingActionManager actionMan;
    public List<UnityEvent> resetEvents;
    private float timeAlive = 0;
    private float timeSinceLastCapture = 0;
    private GhoostlingData data = new GhoostlingData();
    private bool recording = true;
    private List<GhoostlingAction> actions = new List<GhoostlingAction>();
    public mouse_look cam;
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
        f.cameraPitch = cam.xRotation;
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
        gameObject.name += " (Recording)";

        GetComponentInChildren<CapsuleCollider>().enabled = true;
        GetComponent<GhoostlingRecorder>().enabled = false;
        GetComponentInChildren<Camera>().enabled = false;
        GetComponentInChildren<AudioListener>().enabled = false;
        GetComponent<CharacterController>().enabled = false;
        GetComponentInChildren<mouse_look>().acceptInput = false;
        GetComponent<movement>().enabled = false;
        GetComponentInChildren<SkinnedMeshRenderer>().gameObject.layer =
                LayerMask.NameToLayer("Default");

        foreach (var action in resetEvents){
            action.Invoke();
        }

        foreach (GhoostlingPlayer p in
                ghoostlingHolder.GetComponentsInChildren<GhoostlingPlayer>()) {
            p.Restart();
        }
        GetComponent<GhoostlingPlayer>().enabled = true;
        GetComponent<GhoostlingPlayer>().SetData(data);

        GameObject g = GameObject.Instantiate(playerPrefab, ghoostlingHolder);
        g.transform.position = ghoostlingHolder.position;
        g.transform.eulerAngles = ghoostlingHolder.eulerAngles;

        g.name = "Ghoostling #" + g.GetComponentInChildren<GhoostlingRecorder>().GetDataId();
        g.GetComponentInChildren<GhoostlingRecorder>().ghoostlingHolder = ghoostlingHolder;

        g.GetComponentInChildren<CapsuleCollider>().enabled = false;
        g.GetComponent<GhoostlingRecorder>().enabled = true;
        g.GetComponentInChildren<Camera>().enabled = true;
        g.GetComponentInChildren<AudioListener>().enabled = true;
        g.GetComponent<CharacterController>().enabled = true;
        g.GetComponentInChildren<mouse_look>().acceptInput = true;
        g.GetComponent<movement>().enabled = true;
        g.GetComponentInChildren<SkinnedMeshRenderer>().gameObject.layer =
                LayerMask.NameToLayer("Player");

        foreach (GhoostlingPlayer p in
                ghoostlingHolder.GetComponentsInChildren<GhoostlingPlayer>()) {
            p.Restart();
        }
    }
}
