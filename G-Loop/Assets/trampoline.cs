using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trampoline : MonoBehaviour
{
    public float bounciness = 10;
    private Collider trampolin;
    private void Start() {
        trampolin = GetComponent<Collider>();
    }
    private void OnTriggerEnter(Collider other) {        
        GooseController goose = other.GetComponent<GooseController>();
        Debug.Log("trampoline hit");
        if(goose == null){
            return;
        }
        goose.Movement.yeet(bounciness);
    }
}
