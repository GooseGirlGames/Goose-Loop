using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ignore_bullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision other) {
        if (other.collider.gameObject.tag == "Bullet") {
            Physics.IgnoreCollision(other.gameObject.GetComponent<Collider>() , this.gameObject.GetComponent<Collider>() , true);
        }
    }
}
