using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class button_push : MonoBehaviour
{
    private Animator button_animator;
    private Collider button;
    public bool active = false;
    
    
    // Start is called before the first frame update
    void Start(){
        button_animator = GetComponent<Animator>();
        button = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other) {
        active = true;
    }
    private void OnTriggerExit(Collider other) {
        active = false;
    }

    // Update is called once per frame
    void Update(){
        button_animator.SetBool("pressed", active);
    }
}
