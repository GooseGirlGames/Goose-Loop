using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prograkatzsprache : MonoBehaviour
{
    public Transform goose;
    public Animator a;
    public float thres = 5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float d = Vector3.Magnitude(goose.position - transform.position);
        a.SetBool("Dominate", d < thres);
    }
}
