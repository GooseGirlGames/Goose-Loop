using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pulley_down : MonoBehaviour
{
    // Start is called before the first frame update
    private Animator pully_animator;
    public int gooseCount;
    
    void Start()
    {
        pully_animator = GetComponent<Animator>();
    }
    private void OnCollisionEnter(Collision other) {
        gooseCount++;
    }
    private void OnCollisionExit(Collision other) {
        gooseCount--;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        pully_animator.SetInteger("passangers", gooseCount);
    }
}
