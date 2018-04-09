using UnityEngine.Networking;
using UnityEngine;

public class Bullet : NetworkBehaviour {

    private NetworkIdentity owner;
    [SerializeField] private int bulletDamage;
    [SerializeField] private GameObject bloodEffectPrefab;
    [SerializeField] private GameObject dustEffectPrefab;
    private Quaternion particleRotation;
    
    public void SetOwner(NetworkIdentity id)
    {
        owner = id;
    }

    void Update()
    {
        Destroy(gameObject, 5f);
    }

    [Command]
    void CmdPlayerHit(int _damage, GameObject _player)
    {
        _player.GetComponent<PlayerHealth>().RpcTakeDamage(_damage);
    }

    [Command]
    void CmdSpawnParticle(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        GameObject effect = Instantiate(prefab, position, rotation);
        Destroy(effect, 0.5f);
        NetworkServer.Spawn(effect);
        
    }

    void OnCollisionEnter(Collision other)
    {
        if (!isClient)
            return;
        particleRotation = Quaternion.FromToRotation(transform.right, other.contacts[0].normal);
        if (other.gameObject.CompareTag("Player") )
        {
            CmdSpawnParticle(bloodEffectPrefab, transform.position, particleRotation);
            CmdPlayerHit(bulletDamage, other.gameObject);
            Destroy(gameObject);
          
        }
        else //Play hit particles at other.contacts[0].point? 
        {
            CmdSpawnParticle(dustEffectPrefab, transform.position, particleRotation);
            Destroy(gameObject);
        }
    }
}
