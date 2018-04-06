using UnityEngine.Networking;
using UnityEngine;

public class Bullet : NetworkBehaviour {

    public NetworkInstanceId owner;
    [SerializeField] private int bulletDamage;
    
    public void SetOwner(NetworkInstanceId id)
    {
        owner = id;
    }

    [Command]
    void CmdPlayerHit(int _damage, GameObject _player)
    {
        _player.GetComponent<PlayerHealth>().RpcTakeDamage(_damage);
        Debug.Log("dealt " + _damage + " to " + _player);
    }

    void OnCollisionEnter(Collision other)
    {
        Debug.Log("Collided: " + other.gameObject.name);

        if (other.gameObject.CompareTag("Player"))
            CmdPlayerHit(bulletDamage, other.gameObject);

        //Play hit particles at other.contacts[0].point? 
        Destroy(this.gameObject);
    }
}
