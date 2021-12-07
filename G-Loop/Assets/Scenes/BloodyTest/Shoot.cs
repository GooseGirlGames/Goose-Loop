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
            yield return new WaitForSeconds(5);
            reload = false;
            bullet_count = 1; 
    }

    void Update()
    {   if(bullet_count%(bullets+1) != 0){
            reload = false;
        }
        else{
            reload = true;
        }    
        if(reload){
            StartCoroutine(Reload());
        }
        else{
            if(Input.GetButtonDown("Fire1")){
                gun.SetActive(true);
                bullet_count++; 
                GameObject bullet = Instantiate(gun, spawn.transform.position, Quaternion.identity) as GameObject;
                bullet.GetComponent<Rigidbody>().AddForce(transform.forward *1000);
                Debug.Log(bullet_count);
            }
        }
        
    }
}
