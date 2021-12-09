using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GhoostlingRecorder : MonoBehaviour {
    public GameObject playerPrefab;
    public Transform ghoostlingHolder;
    public List<UnityEvent> resetEvents;  // TODO move to ghoostlingmanager
    private float timeAlive = 0;
    private float timeSinceLastCapture = 0;
    private GhoostlingData data = new GhoostlingData();
    private bool recording = true;
    public mouse_look cam;
    public GameObject viewModel;
    void Awake() {
    }

    void Update() {
        if (!recording) return;

        timeAlive += Time.deltaTime;
        timeSinceLastCapture += Time.deltaTime;

        /*
        if (timeSinceLastCapture >= 1f/GhoostlingData.RATE) {
            data.AddFrame(CurrentFrame());
            timeSinceLastCapture = 0;
        }
        */

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
        // TODO expand?
        return f;
    }

    private void StopRecording() {
        recording = false;
    }
    public bool IsRecording() {
        return recording;
    }
    public int GetDataId() {
        return -1; // TODO remove
    }
    private void SpawnGhoostling() {
        gameObject.name += " (Recording)";

        GetComponentInChildren<CapsuleCollider>().enabled = true;
        GetComponent<GhoostlingRecorder>().enabled = false;
        GetComponentInChildren<Camera>().enabled = false;
        GetComponentInChildren<AudioListener>().enabled = false;
        GetComponent<CharacterController>().enabled = false;
        GetComponentInChildren<mouse_look>().acceptInput = false;
        GetComponent<Movement>().enabled = false;
        GetComponentInChildren<SkinnedMeshRenderer>().gameObject.layer =
                LayerMask.NameToLayer("Default");
        viewModel.SetActive(false);

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
        g.GetComponent<Movement>().enabled = true;
        g.GetComponentInChildren<SkinnedMeshRenderer>().gameObject.layer =
                LayerMask.NameToLayer("Player");
        g.GetComponent<GhoostlingRecorder>().viewModel.SetActive(true);
        foreach (GhoostlingPlayer p in
                ghoostlingHolder.GetComponentsInChildren<GhoostlingPlayer>()) {
            p.Restart();
        }
    }
}
