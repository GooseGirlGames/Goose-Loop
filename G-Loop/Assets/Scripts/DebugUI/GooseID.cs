using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GooseID : MonoBehaviour {
    public GooseController controller;
    public TMP_Text text;
    
    void Start() {
        text.text = "G" + controller.GetId();
    }


    private void OnGUI() {
        var lookTarget = Camera.main.transform.position;
        lookTarget.y = transform.position.y;
        text.gameObject.transform.LookAt(lookTarget);
        text.gameObject.transform.Rotate(Vector3.up, 180.0f);  // flip
    }
}
