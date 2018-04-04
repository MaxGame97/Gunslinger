using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour {

    public int type = 0;

    public int index = 0;

    public GameObject bulletPrefab;

    public Sprite bulletImage;

    private GameObject inventory;

    private InventoryScript inventoryScript;

    private void Start()
    {
        inventoryScript = GameObject.FindGameObjectWithTag("Inventory").GetComponent<InventoryScript>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            PickerUpper p = other.gameObject.GetComponent<PickerUpper>();
            if (p != null)
            {
                p.Pick(type,index);
                inventoryScript.Pick(type, bulletImage);
                Destroy(this.gameObject);
            }
        }
    }
}
