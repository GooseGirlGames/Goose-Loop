using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class button_push : MonoBehaviour
{
    private Animator button_animator;
    private Collider button;
    public bool active = false;
    public List<UnityEvent> triggerEvents;

    
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

    void FixedUpdate(){
        button_animator.SetBool("pressed", active);
        if(active){
            foreach (var item in triggerEvents)
            {
                item.Invoke();
            }
        } 
    }
}
