using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PickUp : NetworkBehaviour {

    public int type = 0;

    public int index = 0;

    public GameObject bulletPrefab;

    public Sprite bulletImage;

    public GameObject inventory;

    public InventoryScript inventoryScript;

    private void Awake()
    {
        //varför fungerar inte get component eller någon verision av find
        inventory = GameObject.FindGameObjectWithTag("Inventory");
        inventoryScript = GameObject.FindGameObjectWithTag("Inventory").GetComponent<InventoryScript>();
    }


    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag);
        if (other.tag == "Player")
        {
            inventoryScript.CmdPick(type, bulletImage);
            Destroy(this.gameObject);
        }
    }
}
