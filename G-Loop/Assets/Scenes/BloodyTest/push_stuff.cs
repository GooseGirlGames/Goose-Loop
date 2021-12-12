using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class push_stuff : MonoBehaviour
{

    private void OnCollisionEnter(Collision other) {
        other.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward *1000);
        Debug.Log("HIT");
    }
    // Update is called once per frame
    void Update() {
    }
}
