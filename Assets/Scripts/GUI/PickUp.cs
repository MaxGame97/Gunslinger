using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PickUp : NetworkBehaviour {

    public int type = 0;

    public int index = 0;

    public GameObject bulletPrefab;

    public Sprite bulletImage;

    public InventoryScript inventoryScript;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            inventoryScript = other.gameObject.GetComponentInChildren<InventoryScript>();
            if (inventoryScript != null)
            {
                inventoryScript.CmdPick(type, bulletImage);
            }
            Destroy(this.gameObject);
        }
    }
}
