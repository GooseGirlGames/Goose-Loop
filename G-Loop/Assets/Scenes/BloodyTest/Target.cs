using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{

    public ParticleSystem blood;
    // Start is called before the first frame update
    void Start()
    {
        blood.Stop();
    }
    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Target"){
            blood.transform.position = other.transform.position;
            blood.Play();
        }
        blood.Stop();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
