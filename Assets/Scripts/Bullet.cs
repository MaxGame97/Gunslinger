using UnityEngine.Networking;
using UnityEngine;

public class Bullet : NetworkBehaviour {

    [SyncVar] private GameObject owner;
    [SerializeField] private int bulletDamage;
    [SerializeField] private float bulletDestructionTimer;
    [SerializeField] private GameObject playerHitEffect;
    [SerializeField] private GameObject groundHitEffect;
    

    private void Start()
    {
        Destroy(gameObject, bulletDestructionTimer);   
    }

    [ClientRpc]
    public void RpcSetOwner(GameObject id)  // Set the owner of this object
    {
        owner = id;
    }

    [Command]   // Function run from client on server
    void CmdPlayerHit(int _damage, GameObject _player, GameObject shooter)
    {
        // Tell clients that _player takes _damage from shooter
        _player.GetComponent<PlayerHealth>().RpcTakeDamage(_damage, shooter);
    }

    [Command]   // Function run from client on server
    void CmdSpawnParticle(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        // Instantiate the effect on the server
        GameObject effect = Instantiate(prefab, position, rotation);

        // Set the destruction of the effect
        Destroy(effect, 0.5f);
        
        // Spawn the bullet on on all clients as well
        NetworkServer.Spawn(effect);
    }

    void OnCollisionEnter(Collision other)
    {
        if (!isClient)  //We only want to perfrom collision detection on the shooters client!
            return;

        // Get the desired rotation for the particles
        Quaternion particleRotation = Quaternion.FromToRotation(transform.right, other.contacts[0].normal);
        Vector3 collisionPoint = other.contacts[0].point;

        if (other.gameObject.CompareTag("Player"))  // If we hit a player 
        {
            CmdSpawnParticle(playerHitEffect, transform.position , particleRotation);   //Spawn player hit particles on 
            CmdPlayerHit(bulletDamage, other.gameObject, owner);
    
            Destroy(gameObject);
          
        }
        else // if we hit something besides a player
        {
            CmdSpawnParticle(groundHitEffect, transform.position, particleRotation);    //Spawn particles
            Destroy(gameObject);
        }
    }
}
