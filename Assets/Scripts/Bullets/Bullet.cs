using UnityEngine.Networking;
using UnityEngine;

public class Bullet : NetworkBehaviour {

    [SyncVar] private GameObject owner;
    [SerializeField] private int bulletDamage;
    [SerializeField] private GameObject bloodEffectPrefab;
    [SerializeField] private GameObject dustEffectPrefab;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float bulletLifeTime;
    [SerializeField] private LayerMask layerMask;
    private Quaternion particleRotation;

    [ClientRpc]
    public void RpcSetOwner(GameObject id)  // Set the owner of this object
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

            // First check if the bullet hits anything this frame. If it did, Detect what kind of object that was hit.
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
    void CmdPlayerHit(float _damageMultiplier, GameObject _player, GameObject _shooter)
    {
        // Make sure the damage is always atleast 1 when converting from Float to Integer
        float damage = _damageMultiplier * bulletDamage;

        // If damageMultiplier * bulletDamage is less than 1, set damage to 1.
        if (damage < 1f)
        {
            damage = 1f;
        }
        // Multiply the damage from the hitbox with the bullet's damage. Send damage to PlayerHealth script of the player who got shot.
        _player.GetComponent<PlayerHealth>().RpcTakeDamage((int)damage, _shooter);
    }

    // Tell the server to spawn a particleeffect on a position with a rotation. Remove particle object after x few seconds.
    [Command]
    void CmdSpawnParticle(string _prefab, Vector3 _position, Quaternion _rotation)
    {
        Debug.Log("Spawn particles");
        GameObject _go = Resources.Load("Particles/" + _prefab) as GameObject; 
        GameObject effect = Instantiate(_go, _position, _rotation);
        NetworkServer.Spawn(effect);
        Destroy(effect, 0.5f);
       
    }

    // Function for hit detection of players or other objects.
    void HitDetection(RaycastHit _hit)
    {
        string _particle = "";
        // Create a Quaternion for the creation of the partcle effect.
        particleRotation = Quaternion.FromToRotation(Vector3.up, _hit.normal);

        // If the bullet hits a player's hitbox. Create blood effect on hit.position.
        if (_hit.collider.CompareTag("Hitbox"))
        {
            // If the hitbox is set to isHead then log headshot in console.
            if (_hit.collider.GetComponent<PlayerHitbox>().IsHead)
            {
                    // Add headshot specific things here:
            }
            // Damage from bullet is multiplied from the hitbox on player hit.
            CmdPlayerHit(_hit.collider.GetComponent<PlayerHitbox>().DamageMultiplier, _hit.collider.transform.root.gameObject, owner);
            _particle = "FX_BloodSplatter";
            // Create Particle Effects and set its rotation.
            //    CmdSpawnParticle(_particle, _hit.point, particleRotation);
        }
        else 
        {
            _particle = "FX_DirtSplatter";
        }
        // Create Particle Effects and set its rotation.
        CmdSpawnParticle(_particle, _hit.point, particleRotation);
        Destroy(gameObject);
    }

}
