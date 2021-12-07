using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    public GameObject gun;
    public GameObject spawn;
    public int bullets;
    private int bullet_count = 1;
    private bool reload = false;

    // Start is called before the first frame update
    void Start(){
        
    }
    IEnumerator Reload(){
            reload = true;
            yield return new WaitForSeconds(5);
            reload = false;
            bullet_count = 1; 
    }

    void Update()
    { 
        if(Input.GetButtonDown("Fire1") && !reload){
            GameObject bullet = Instantiate(gun, spawn.transform.position, Quaternion.identity) as GameObject;
            bullet.GetComponent<Rigidbody>().AddForce(transform.forward *1000);
            gun.SetActive(true);
            bullet_count++; 
            if(bullet_count%(bullets+1) == 0){
                StartCoroutine(Reload());
            }
            Debug.Log(bullet_count);
        }
    }
}
