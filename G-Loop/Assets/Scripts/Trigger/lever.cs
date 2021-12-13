using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class lever : MonoBehaviour
{
    public bool on = false;
    public List<UnityEvent> triggerEvents;
    private Animator lever_animator;
    public Collider activationArea;
    // Start is called before the first frame update
    void Start()
    {
        lever_animator = GetComponent<Animator>();
    }

    private void OnTriggerStay(Collider other) {
        var cc = other.GetComponentInChildren<GooseController>();
        if (cc) {
            if(cc.Interact.isInterakting()){
                on = !on;
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        lever_animator.SetBool("on", on);
        if(on){
            foreach (var item in triggerEvents)
            {
                item.Invoke();
            }
        }
    }
}
