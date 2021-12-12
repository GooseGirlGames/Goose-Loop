using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movable_box : MonoBehaviour
{
    public LayerMask FloatLayer;
    private GameObject body;
    private bool allowed_to_move = true;
    // Start is called before the first frame update

    private void Start() {
        body = gameObject;
    }

    private void zoneCheck(){
        if(Physics.CheckSphere(body.transform.position, 0.1f , FloatLayer)){
            allowed_to_move = true;
        }
        else{
            allowed_to_move = false;
        }
    }
    private void Update() {
        zoneCheck();
    }
    public void move_x(){
        Debug.Log("in zone: " + allowed_to_move);
        if(allowed_to_move){
            body.transform.Translate(0.1f, 0f, 0f);
        }
        else{
        body.transform.Translate(-0.1f, 0f, 0f);
        }
    }
    public void move_x_back(){
        Debug.Log("in zone: " + allowed_to_move);
        if(allowed_to_move){
            body.transform.Translate(-0.1f, 0f, 0f);
        }
        else{
        body.transform.Translate(0.1f, 0f, 0f);
        }
    }
    public void move_z(){
        Debug.Log("in zone: " + allowed_to_move);
        if(allowed_to_move){
            body.transform.Translate(0f, 0f, 0.1f);
        }
        else{
        body.transform.Translate(0f, 0f, -0.1f);
        }
    }
    public void move_z_back(){
        Debug.Log("in zone: " + allowed_to_move);
        if(allowed_to_move){
            body.transform.Translate(0f, 0f, -0.1f);
        }
        else{
        body.transform.Translate(0f, 0f, 0.1f);
        }
    }
}
