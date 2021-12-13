using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pulley_down : MonoBehaviour
{
    // Start is called before the first frame update
    private Animator pully_animator;
    public Animator going_up;
    public int gooseCount;
    public bool used = false;
    
    void Start()
    {
        pully_animator = GetComponent<Animator>();
    }
    private void OnTriggerEnter(Collider other) {
        var cc = other.GetComponentInChildren<CharacterController>();
        if (cc) {
            //Debug.Log("Goose enter: " + other);
            gooseCount++;
        }
    }
    private void OnTriggerExit(Collider other) {
        var cc = other.GetComponentInChildren<CharacterController>();
        if (cc) {
            //Debug.Log("Goose enter: " + other);
            gooseCount--;
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        pully_animator.SetInteger("passangers", gooseCount);
        //Debug.Log("Goose on board:" + gooseCount);
        going_up.SetInteger("passangers", gooseCount);
    }
}
