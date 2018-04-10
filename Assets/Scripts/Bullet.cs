using UnityEngine.Networking;
using UnityEngine;

public class Bullet : NetworkBehaviour {

    private NetworkIdentity owner;
    [SerializeField] private int bulletDamage;
    [SerializeField] private GameObject bloodEffectPrefab;
    [SerializeField] private GameObject dustEffectPrefab;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float bulletLifeTime;
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
            if (Physics.Raycast(transform.position, transform.forward, out hit, bulletSpeed * Time.deltaTime))
            {
                HitDetection(hit);
            }
        }
        transform.Translate(Vector3.forward * bulletSpeed * Time.deltaTime);
    }

    // Tell the server to update the player that got hit's health.
    [Command]
    void CmdPlayerHit(int _damage, GameObject _player)
    {
        _player.GetComponent<PlayerHealth>().RpcTakeDamage(_damage);
    }

    // Tell the server to spawn a particleeffect on a position with a rotation.
    [Command]
    void CmdSpawnParticle(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        GameObject effect = Instantiate(prefab, position, rotation);
        NetworkServer.Spawn(effect);
        Destroy(effect, 0.5f);
    }

    // Function for 
    void HitDetection(RaycastHit _hit)
    {
        // Create a Quaternion for the creation of the partcle effect.
        particleRotation = Quaternion.FromToRotation(Vector3.up, _hit.normal);

        // If the bullet hits a player. Create blood effect on hit.position.
        if (_hit.collider.gameObject.CompareTag("Player"))
        {
            CmdSpawnParticle(bloodEffectPrefab, _hit.point, particleRotation);
            CmdPlayerHit(bulletDamage, _hit.collider.gameObject);
            Destroy(gameObject);
        }
        else // Otherwise play dust particles on hit.position. 
        {
            CmdSpawnParticle(dustEffectPrefab, _hit.point, particleRotation);
            Destroy(gameObject);
        }
    }

}
