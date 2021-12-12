using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GooseID : MonoBehaviour {
    public GooseController controller;
    public TMP_Text text;
    private DebugMenu debug;
    
    void Start() {
        text.text = "G" + controller.GetId();
        debug = DebugMenu.GetInstance();
    }


    private void OnGUI() {
        if (!Camera.main) return;
        
        var lookTarget = Camera.main.transform.position;
        lookTarget.y = transform.position.y;
        text.gameObject.transform.LookAt(lookTarget);
        text.gameObject.transform.Rotate(Vector3.up, 180.0f);  // flip
    }

    private void Update() {
        if (debug) {
            text.enabled = debug.IsVisible();
        } else {
            text.enabled = false;
        }
    }
}
