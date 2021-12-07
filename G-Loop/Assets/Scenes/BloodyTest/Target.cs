using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{

    public GameObject blood;
    // Start is called before the first frame update
    void Start()
    {

    }
    private void OnCollisionEnter(Collision other) {
        if(other.collider.tag == "Target"){
            GameObject splat = Instantiate(blood, other.gameObject.transform.position, Quaternion.identity) as GameObject;
            blood.GetComponent<ParticleSystem>().Play();
        }
        //blood.Stop();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
