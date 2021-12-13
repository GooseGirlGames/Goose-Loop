using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movable_box : MonoBehaviour
{
    public LayerMask FloatLayer;
    private GameObject body;
    private List<CharacterController> onPlatform = new List<CharacterController>();

    private void Start() {
        body = gameObject;
    }

    private void OnTriggerEnter(Collider other) {
        var cc = other.GetComponentInChildren<CharacterController>();
        if (cc) {
            onPlatform.Add(cc);
        }
    }

    private void OnTriggerExit(Collider other) {
        var cc = other.GetComponentInChildren<CharacterController>();
        if (cc) {
            onPlatform.Remove(cc);
        }
    }

    public void TryMove(Vector3 move) {
        bool okay = Physics.CheckSphere(body.transform.position + 3 * move, 0.1f, FloatLayer);
        if (okay) {
            body.transform.Translate(move);
            foreach (var cc in onPlatform) {
                cc.Move(move);
            }
        }
    }

    public void move_x(){
        TryMove(new Vector3(-0.1f, 0f, 0f));
    }
    public void move_x_back(){
        TryMove(new Vector3(0.1f, 0f, 0f));
    }
    public void move_z(){
        TryMove(new Vector3(0f, 0f, 0.1f));
    }
    public void move_z_back(){
        TryMove(new Vector3(0f, 0f, -0.1f));
    }
}
