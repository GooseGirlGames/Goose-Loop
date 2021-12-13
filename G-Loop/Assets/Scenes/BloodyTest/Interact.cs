using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact : MonoBehaviour
{
    bool interacting;
    public bool isInterakting(){
        return interacting;
    }
    public void ProcessInputs(GhoostlingData.UserInputs inputs) {
        if(inputs.interactButtonDown){
            interacting = true;
        }
        if(inputs.interactButtonUp){
            interacting = false;
        }
    }
}
