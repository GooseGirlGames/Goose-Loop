using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class push_stuff : MonoBehaviour
{
    private void OnCollisionEnter(Collision other) {
        if(other.collider.tag == "Target" || other.collider.tag == "Duck"){
            other.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward *1000);
        }
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
