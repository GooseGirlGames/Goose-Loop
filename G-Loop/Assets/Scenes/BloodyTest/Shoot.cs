using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    public GameObject bulletPrefab;
    public GameObject spawn;
    //public GameObject reload_warning;
    
    //public GameObject bullet1;
    //public GameObject bullet2;
    //public GameObject bullet3;
    private GameObject[] bullet_image;


    private int bullets;
    private int bullet_count;
    private int reloadingUntilTick;
    private GhoostlingManager gman;
    public const int RELOAD_DURATION = 300;  // ticks
    public const int SHOOT_DELAY = 20;
    private List<GameObject> bulletsFired = new List<GameObject>();

    // Start is called before the first frame update
    void Start(){
        gman = GhoostlingManager.GetInstance();
        Goose_Reset();
    }

    public void Goose_Reset() {
        reloadingUntilTick = -1;
        bullets = 3;
        bullet_count = 1;
        foreach (var go in bulletsFired) {
            GameObject.DestroyImmediate(go);
        }
    }

    public void Reload(){
        BlockFiringFor(RELOAD_DURATION);
    }

    private void BlockFiringFor(int tick) {
        reloadingUntilTick = Mathf.Max(gman.GetCurrentTick() + tick, reloadingUntilTick);
    }

    private bool IsReloading() {
        return reloadingUntilTick > gman.GetCurrentTick();
    }

    public void ProcessInputs(GhoostlingData.UserInputs inputs) {

        if(inputs.fireButtonDown && !IsReloading()){
            BlockFiringFor(SHOOT_DELAY);
            GameObject bullet =
                    Instantiate(bulletPrefab, spawn.transform.position, spawn.transform.rotation)
                    as GameObject;
            bullet.transform.position = spawn.transform.position;
            bullet.GetComponent<Rigidbody>().AddForce(spawn.transform.forward *1000);

            bullet.SetActive(true);
            bulletsFired.Add(bullet);
            //bullet_image[bullet_count-1].SetActive(false);
            bullet_count++; 
            if(bullet_count%(bullets+1) == 0){
                Reload();
            }
        }
    }
}
