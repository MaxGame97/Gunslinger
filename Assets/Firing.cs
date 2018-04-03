using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class Firing : MonoBehaviour {

    public GameObject bulletPrefab;
    public GameObject muzzle;

    [SerializeField]
    private int ammoCount;
    [SerializeField]
    private float reloadTime;
    [SerializeField]
    private bool isReloading;
    [SerializeField]
    private int magSize = 6;


    void Start () {

        ammoCount = magSize;

        // Find the muzzle object on the player and log error if none is found.
        muzzle = GameObject.Find("Muzzle");
        if (muzzle == null)
        {
            Debug.LogError("No muzzle detected for player prefab.");
            this.enabled = false;
        }
        if (bulletPrefab == null)
        {
            Debug.LogError("No bulletPrefab detected for Firing Script.");
            this.enabled = false;
        }
    }

    void Update () {

        if (Input.GetButtonDown("Fire1") && ammoCount >= 1)
        {
            Shoot();
            ammoCount--;
        }


        // Reload revolver. 
        if (Input.GetButtonDown("Fire2"))
        {
            Reload();
        }
	}

    // Create the bullet object relative to the muzzle position 
    // and add velocity. Decrease ammoCount.
    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, muzzle.transform.position, transform.rotation);
        bullet.GetComponent<Rigidbody>().velocity = transform.forward*20;
    }

    // Reload the revolver.
    void Reload()
    {
        ammoCount = magSize;
    }
}
