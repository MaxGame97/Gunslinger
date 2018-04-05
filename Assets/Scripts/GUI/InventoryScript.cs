using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class InventoryScript : MonoBehaviour {

    public Image clean;

    public int inventorySize = 3;

    List<Image> slots = new List<Image>();

	// Use this for initialization
	void Start () {
        Debug.Log("something");
        for (int i = 0; i < inventorySize; i++)
        {
            slots.Add(Instantiate(clean, new Vector3((i * 100) + 50, 50, 0), Quaternion.identity, this.gameObject.transform));
        }
        Debug.Log("something2");
    }

    
    public void CmdPick(int type, Sprite bulletImage)
    {
        if (type != 0)
        {
            slots[type-1].sprite = bulletImage;
        }
    }
}
