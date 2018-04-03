using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// A simple shooting script that handles shooting. Is called from PlayerController.
// Add or change as you want.

public class PlayerShoot_Mattias : NetworkBehaviour
{
    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private Transform muzzle;
    [SerializeField]
    private float velocity;


    [Command]
    public void CmdFire ()
    {
        GameObject bullet = Instantiate (bulletPrefab, muzzle.position, muzzle.rotation);
        bullet.GetComponent<Rigidbody> ().velocity = bullet.transform.forward * velocity;

        NetworkServer.Spawn (bullet);

        Destroy (bullet, 5.0f);
    }
}
