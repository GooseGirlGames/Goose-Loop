using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    public GameObject gun;
    public GameObject spawn;
    public GameObject reload_warning;
    
    public GameObject bullet1;
    public GameObject bullet2;
    public GameObject bullet3;

    private GameObject[] bullet_image;
    public int bullets;
    private int bullet_count = 1;
    private bool reload = false;

    // Start is called before the first frame update
    void Start(){
        bullet_image = new GameObject[3];
        bullet_image[0] = bullet1;
        bullet_image[1] = bullet2;
        bullet_image[2] = bullet3;
    }

    IEnumerator Reload(){
            reload = true;
            yield return new WaitForSeconds(0.5f);
            reload_warning.SetActive(true);
            yield return new WaitForSeconds(5);
            reload_warning.SetActive(false);
            reload = false;
            bullet_count = 1; 
            for(int i = 0; i < bullet_image.Length; i++){
                bullet_image[i].SetActive(true);
            }
            
    }

    void Update()
    { 
        if(Input.GetButtonDown("Fire1") && !reload){
            GameObject bullet = Instantiate(gun, spawn.transform.position, Quaternion.identity) as GameObject;
            bullet.GetComponent<Rigidbody>().AddForce(transform.forward *1000);
            gun.SetActive(true);
            bullet_image[bullet_count-1].SetActive(false);
            bullet_count++; 
            if(bullet_count%(bullets+1) == 0){
                StartCoroutine(Reload());
            }
            Debug.Log(bullet_count);
        }
    }
}
