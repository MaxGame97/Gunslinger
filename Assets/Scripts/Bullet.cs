using UnityEngine.Networking;
using UnityEngine;

public class Bullet : NetworkBehaviour {

    private NetworkIdentity owner;
    [SerializeField] private int bulletDamage;
    [SerializeField] private GameObject bloodEffectPrefab;
    [SerializeField] private GameObject dustEffectPrefab;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float bulletLifeTime;
    [SerializeField] private LayerMask layerMask;
    private Quaternion particleRotation;


    public void SetOwner(NetworkIdentity id)
    {
        owner = id;
    }

    void Start()
    {
        // Destroy bullet after x seconds. 
        Destroy(gameObject, bulletLifeTime);
    }

    // Updates every frame.
    void Update()
    {
        // Let the clients update the bullets instead of server.
        if (hasAuthority)
        {
            RaycastHit hit;

            // First check if the bullet hits anything this frame.
            if (Physics.Raycast(transform.position, transform.forward, out hit, bulletSpeed * Time.deltaTime, layerMask))
            {
                HitDetection(hit);
            }
        }

        // Then move the bullet object after checking for hits.
        transform.Translate(Vector3.forward * bulletSpeed * Time.deltaTime);
    }

    // Command to the server to update the player that got hit's health. 
    [Command]
    void CmdPlayerHit(float _damageMultiplier, GameObject _player)
    {
        // Multiply the damage from the hitbox with the bullet's damage. Send damage to PlayerHealth script of the player who got shot.
        _player.GetComponentInParent<PlayerHealth>().RpcTakeDamage((int)(_damageMultiplier * bulletDamage));
    }

    // Tell the server to spawn a particleeffect on a position with a rotation. Remove particle object after a few seconds.
    [Command]
    void CmdSpawnParticle(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        GameObject effect = Instantiate(prefab, position, rotation);
        NetworkServer.Spawn(effect);
        Destroy(effect, 0.5f);
    }

    // Function for hit detection of players or other objects.
    void HitDetection(RaycastHit _hit)
    {
        Debug.Log(_hit.collider.gameObject.name);
        
        // Create a Quaternion for the creation of the partcle effect.
        particleRotation = Quaternion.FromToRotation(Vector3.up, _hit.normal);

        // If the bullet hits a player's hitbox. Create blood effect on hit.position.
        if (_hit.collider.CompareTag("Hitbox"))
        {
            // Create Particle Effects and set its rotation.
            CmdSpawnParticle(bloodEffectPrefab, _hit.point, particleRotation);
            

            // Damage from bullet is multiplied from the hitbox on player hit.
            CmdPlayerHit(_hit.collider.GetComponent<PlayerHitbox>().DamageMultiplier, _hit.collider.gameObject);

            // Destroy to remove particle object from the scene.
            Destroy(gameObject);
        }
        else 
        {
            // Create Particle Effects and set its rotation.
            CmdSpawnParticle(dustEffectPrefab, _hit.point, particleRotation);
            
            // Destroy to remove particle object from the scene.
            Destroy(gameObject);
        }
    }

}
