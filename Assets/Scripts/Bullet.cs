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

    void OnCollisionEnter(Collision other)
    {
        particleRotation = Quaternion.FromToRotation(transform.right, other.contacts[0].normal);
         
        if (other.gameObject.CompareTag("Player"))
        {
            GameObject effect = Instantiate(bloodEffectPrefab, transform.position, particleRotation);
            NetworkServer.Spawn(effect);
            CmdPlayerHit(bulletDamage, other.gameObject);
            Destroy(gameObject);
            Destroy(effect, 0.5f);
        }
        else //Play hit particles at other.contacts[0].point? 
        {
            GameObject effect = Instantiate(dustEffectPrefab, transform.position, particleRotation);
            NetworkServer.Spawn(effect);
            Destroy(gameObject);
            Destroy(effect, 0.5f);
        }
    }
}
