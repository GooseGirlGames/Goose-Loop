using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class button_action : MonoBehaviour
{
    public Collider button;
    public GameObject destination;
    public List<UnityEvent> enterEvent;
    public List<UnityEvent> exitEvent;
    public void OnTriggerEnter(Collider button) {
        foreach(var action in enterEvent){
            action.Invoke();
        }
    }
    private void OnTriggerExit(Collider other) {
        foreach(var action in exitEvent){
            action.Invoke();
        }
    }
    // Update is called once per frame
}
