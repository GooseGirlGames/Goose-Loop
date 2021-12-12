using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movable_box : MonoBehaviour
{
    private Collider[] moveZone;
    public GameObject zone_holder;
    private Rigidbody body;
    private bool allowed_to_move = true;
    // Start is called before the first frame update

    private void Start() {
        moveZone = zone_holder.GetComponents<Collider>();
        body = GetComponent<Rigidbody>();
    }
    public void move_x(){
        if(allowed_to_move){
            Debug.Log("is moving");
            body.transform.Translate(0.1f, 0f, 0f);
        }
    }
    public void move_x_back(){

    }
    public void move_z(){
        
    }
    public void move_z_back(){
        
    }
}
